using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Enclave.FastPacket.Generator.PositionProviders;
using Enclave.FastPacket.Generator.SizeProviders;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Enclave.FastPacket.Generator;

[Generator]
public class PacketParserGenerator : ISourceGenerator
{
    private TemplatedParserBuilder? _writableRefStruct;
    private TemplatedParserBuilder? _readonlyRefStruct;

    public PacketParserGenerator()
    {
        // When we actually ship this as a nuget package, we definitely don't want to
        // include this workaround required for building everything in one solution,
        // in the released nuget. So we #if it out, based on a build property set
        // in GH Actions.
#if !REMOVE_LOCAL_REF_WORKAROUND
        AppDomain.CurrentDomain.AssemblyResolve += (s, e) =>
        {
            if (e.Name.StartsWith("Scriban", StringComparison.InvariantCulture))
            {
                try
                {
                    return Assembly.LoadFile(
                        Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                            @".nuget\packages\scriban\5.0.0\lib\netstandard2.0\Scriban.dll"));
                }
                catch
                {
                        // Best not to propagate an exception out of the resolver.
                        return null;
                }
            }

            return null;
        };
#endif
    }

    public void Execute(GeneratorExecutionContext context)
    {
        var rx = (SyntaxReceiver)context.SyntaxContextReceiver!;

#pragma warning disable RS1024 // Compare symbols correctly (https://github.com/dotnet/roslyn-analyzers/issues/4469)
        var trackSeenTypes = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
#pragma warning restore RS1024 // Compare symbols correctly

        var packetFieldFactory = new PacketPropertyFactory(context);

        var cachedPacketDefinitions = new Dictionary<INamedTypeSymbol, PacketParserDefinition>(SymbolEqualityComparer.Default);

        // Go through each entry and add the appropriate types.
        foreach (var instance in rx.ImplementationTypes)
        {
            var node = instance.Owner;
            var attr = instance.Attribute;

            var semanticModel = context.Compilation.GetSemanticModel(node.SyntaxTree);

            var structSymbol = semanticModel.GetDeclaredSymbol(node);

            if (structSymbol is null || !trackSeenTypes.Add(structSymbol))
            {
                // Seen already - duplicate attribute usage warning is going to show up.
                continue;
            }

            var attrData = structSymbol.GetAttributes().FirstOrDefault(
                x => x.ApplicationSyntaxReference?.Span == attr.Span);

            // First, validate.
            if (attrData is AttributeData &&
                GenerationOptions.TryGetFromAttribute(attrData, out var options) &&
                ValidateType(context, options, node, structSymbol))
            {
                // - Look at the definition, and build a picture of the set of properties defined.
                // - Also, see if we have seen it already (cache the computed definition from the type).
                // - As we visit the definition, make sure we validate each property, and consult it's type to know
                //   what to do with it. The type of property basically dictates how we implement it.
                // - Need to consider how we might allow 'custom' behaviour, including the ability to 'skip' bytes,
                //   provide a custom position computation for a field, etc.
                var defType = options.DefinitionType;

                try
                {
                    if (!cachedPacketDefinitions.TryGetValue(defType, out var parserDefinition))
                    {
                        parserDefinition = CreatePacketDefinition(packetFieldFactory, defType);

                        cachedPacketDefinitions.Add(defType, parserDefinition);
                    }

                    IParserBuilder builder;

                    if (options.IsReadOnly)
                    {
                        builder = _readonlyRefStruct ??= new("ReadOnlyRefStruct");
                    }
                    else
                    {
                        builder = _writableRefStruct ??= new("WriteableRefStruct");
                    }

                    var generated = builder.Generate(parserDefinition, options, structSymbol, packetFieldFactory);

                    context.AddSource(structSymbol.GetFullyQualifiedGeneratedFileName(), generated);
                }
                catch (Exception ex)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Diagnostics.InternalError, attr.GetLocation(), structSymbol.Name, ex.ToString()));
                }
            }
        }
    }

    private static PacketParserDefinition CreatePacketDefinition(PacketPropertyFactory packetFieldFactory, INamedTypeSymbol defType)
    {
        var propertySymbolSet = new List<(IPropertySymbol Prop, INamedTypeSymbol? UnionType)>();

        var members = defType.GetMembers();

        // In the first pass, we need to drill down into any union structs to extract associated properties.
        foreach (var member in members)
        {
            if (member is IPropertySymbol prop)
            {
                if (!prop.IsStatic)
                {
                    propertySymbolSet.Add((prop, null));
                }
            }
            else if (member is INamedTypeSymbol childTypeSymbol && member is not IErrorTypeSymbol)
            {
                // May be a union, need to drill in.
                foreach (var childMember in childTypeSymbol.GetMembers())
                {
                    if (childMember is IPropertySymbol childProp && !childProp.IsStatic)
                    {
                        propertySymbolSet.Add((childProp, childTypeSymbol));
                    }
                }
            }
        }

        var propertySet = new List<IPacketProperty>();

        IPacketProperty? lastProp = null;
        INamedTypeSymbol? currentUnion = null;
        IPacketProperty? currentUnionProperty = null;

        var listMinSize = new List<string>();

        void UpdateMinimumSizeEntries(IPacketProperty? createdProp)
        {
            if (createdProp!.PositionProvider is IConstantPositionProvider constantPosition)
            {
                // Fixed position.
                listMinSize.Clear();

                listMinSize.Add(constantPosition.GetConstantPositionExpression());
            }

            if (createdProp.SizeProvider is IConstantSizeProvider constantSize)
            {
                listMinSize.Add(constantSize.GetConstantSizeExpression());
            }
        }

        for (int idx = 0; idx < propertySymbolSet.Count; idx++)
        {
            var (propSymbol, unionSymbol) = propertySymbolSet[idx];

            if (SymbolEqualityComparer.Default.Equals(currentUnion, unionSymbol))
            {
                if (currentUnion is not null && currentUnionProperty is null)
                {
                    // Apparently had a problem creating the union, so ignore this property as well.
                    continue;
                }
            }
            else
            {
                // If the union has changed (null -> union, union -> new union, union -> null),
                // set the last property to the representation of that union.
                if (currentUnionProperty is not null)
                {
                    lastProp = currentUnionProperty;

                    UpdateMinimumSizeEntries(currentUnionProperty);
                }

                currentUnion = unionSymbol;

                if (unionSymbol is null)
                {
                    currentUnionProperty = null;
                }
                else
                {
                    if (!packetFieldFactory.TryCreateUnion(defType, unionSymbol, lastProp, out currentUnionProperty))
                    {
                        // If we couldn't create the union, ignore the property as well.
                        continue;
                    }
                }
            }

            if (packetFieldFactory.TryCreate(defType, propSymbol, lastProp, idx == propertySymbolSet.Count - 1, out var createdProp))
            {
                // Add this to render.
                propertySet.Add(createdProp!);

                // If we're in a union, we don't move the last property on, so the start position stays the same
                // for the next property.
                if (currentUnionProperty is null)
                {
                    lastProp = createdProp;

                    UpdateMinimumSizeEntries(createdProp);
                }
            }
        }

        string minSizeExpression = "0";

        if (listMinSize.Count > 0)
        {
            minSizeExpression = string.Join(" + ", listMinSize);
        }

        return new PacketParserDefinition(propertySet, minSizeExpression);
    }

    private static bool ValidateType(GeneratorExecutionContext context, GenerationOptions options, StructDeclarationSyntax owner, INamedTypeSymbol symbol)
    {
        if (!owner.Modifiers.Any(SyntaxKind.PartialKeyword))
        {
            context.ReportDiagnostic(Diagnostic.Create(Diagnostics.TypeIsNotPartial, owner.Identifier.GetLocation(), symbol.Name));

            return false;
        }

        return true;
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForPostInitialization(pi =>
        {
            pi.AddSource("FastPacket_PacketImplementationAttribute", GetTemplateContent("PacketImplementationAttribute"));
            pi.AddSource("FastPacket_PacketFieldAttribute", GetTemplateContent("PacketFieldAttribute"));
        });
        context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
    }

    private class SyntaxReceiver : ISyntaxContextReceiver
    {
        public List<(AttributeSyntax Attribute, StructDeclarationSyntax Owner)> ImplementationTypes { get; } = new();

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            var node = context.Node;

            if (context.Node is AttributeSyntax attrib &&
                context.SemanticModel.GetTypeInfo(attrib).Type?.ToDisplayString() == "Enclave.FastPacket.Generator.PacketImplementationAttribute")
            {
                var owningStruct = attrib.FirstAncestorOrSelf<StructDeclarationSyntax>();

                if (owningStruct is object)
                {
                    ImplementationTypes.Add((attrib, owningStruct));
                }
            }
        }
    }

    public static string GetTemplateContent(string templateName)
    {
        using var implSource = new StreamReader(
            typeof(PacketParserGenerator).Assembly.GetManifestResourceStream(
                $"Enclave.FastPacket.Generator.FileTemplates.{templateName}.cs"));

        return implSource.ReadToEnd();
    }
}

public class UnionOptions
{
    public INamedTypeSymbol? SizeOfType { get; set; }
}
