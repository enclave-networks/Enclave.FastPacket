using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Enclave.FastPacket.Generator
{
    [Generator]
    public class PacketParserGenerator : ISourceGenerator
    {
        private TemplatedParserBuilder? _writableRefStruct;
        private TemplatedParserBuilder? _readonlyRefStruct;

        public PacketParserGenerator()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (s, e) =>
            {
                if (e.Name.StartsWith("Scriban", StringComparison.InvariantCulture))
                {
                    return Assembly.LoadFile(
                        Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                            @".nuget\packages\scriban\5.0.0\lib\netstandard2.0\Scriban.dll"));
                }

                return null;
            };
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

                if (!trackSeenTypes.Add(structSymbol))
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
            var members = defType.GetMembers().OfType<IPropertySymbol>().ToImmutableList();
            var propertySet = ImmutableList<IPacketProperty>.Empty;

            IPacketProperty? lastProp = null;

            for (int idx = 0; idx < members.Count; idx++)
            {
                ISymbol? member = members[idx];

                if (member is IPropertySymbol prop &&
                    packetFieldFactory.TryCreate(prop, lastProp, idx == members.Count - 1, out var createdProp))
                {
                    // Add this to render.
                    propertySet = propertySet.Add(createdProp!);

                    lastProp = createdProp;
                }
            }

            return new PacketParserDefinition(propertySet);
        }

        private bool ValidateType(GeneratorExecutionContext context, GenerationOptions options, StructDeclarationSyntax owner, INamedTypeSymbol symbol)
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

                // Whenever we see an implementation attribute, we define 
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
}
