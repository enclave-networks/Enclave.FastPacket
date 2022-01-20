using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using Enclave.FastPacket.Generator.ValueProviders;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Enclave.FastPacket.Generator
{

    internal class PacketPropertyFactory
    {
        private readonly GeneratorExecutionContext _ctxt;
        private readonly PacketFieldAttribute _defaultFieldAttribute = new PacketFieldAttribute();

        private readonly Dictionary<INamedTypeSymbol, (ISizeProvider Size, IValueProvider Value)> _supportedNumericTypes;

        private readonly Dictionary<INamedTypeSymbol, CustomProviderResult> _customProviderCache;
        private readonly IValueProvider _remainingSpanValueProvider;

        public PacketPropertyFactory(GeneratorExecutionContext ctxt)
        {
            _ctxt = ctxt;
            _customProviderCache = new Dictionary<INamedTypeSymbol, CustomProviderResult>(SymbolEqualityComparer.Default);

            // Get the type ReadOnlySpan<T>
            // `1 because ReadOnlySpan<T> has 1 generic parameter
            INamedTypeSymbol? readonlySpanT = ctxt.Compilation.GetTypeByMetadataName("System.ReadOnlySpan`1");

            if (readonlySpanT is null)
            {
                throw new InvalidOperationException("Cannot handle absence of System.ReadOnlySpan<T>");
            }

            // Construct ReadOnlySpan<byte> from ReadOnlySpan<T>
            ReadOnlySpanByteType = readonlySpanT.Construct(ctxt.Compilation.GetSpecialType(SpecialType.System_Byte));

            INamedTypeSymbol? spanT = ctxt.Compilation.GetTypeByMetadataName("System.Span`1");

            if (spanT is null)
            {
                throw new InvalidOperationException("Cannot handle absence of System.Span<T>");
            }

            SpanByteType = spanT.Construct(ctxt.Compilation.GetSpecialType(SpecialType.System_Byte));

            _remainingSpanValueProvider = new SpanRestOfDataValueProvider(SpanByteType);

            _supportedNumericTypes = new Dictionary<INamedTypeSymbol, (ISizeProvider Size, IValueProvider Value)>(SymbolEqualityComparer.Default);

            INamedTypeSymbol GetSpecialType(SpecialType specialType) => ctxt.Compilation.GetSpecialType(specialType);

            void AddPrimitiveType(SpecialType type, IValueProvider? customProvider = null)
            {
                var symbol = GetSpecialType(type);
                _supportedNumericTypes.Add(symbol, (new SizeOfSizeProvider(symbol), customProvider ?? new BinaryPrimitivesValueProvider(symbol)));
            }

            AddPrimitiveType(SpecialType.System_UInt16);
            AddPrimitiveType(SpecialType.System_UInt32);
            AddPrimitiveType(SpecialType.System_UInt64);
            AddPrimitiveType(SpecialType.System_Int16);
            AddPrimitiveType(SpecialType.System_Int32);
            AddPrimitiveType(SpecialType.System_Int64);
            AddPrimitiveType(SpecialType.System_Byte, new SingleByteValueProvider(GetSpecialType(SpecialType.System_Byte)));
        }

        public INamedTypeSymbol ReadOnlySpanByteType { get; }

        public INamedTypeSymbol SpanByteType { get; }

        private struct PacketFieldOptions
        {
            public int? Size { get; set; }

            public int? Position { get; set; }

            public string? PositionFunction { get; set; }

            public INamedTypeSymbol? EnumBackingType { get; set; }
        }

        public bool TryCreate(IPropertySymbol propSymbol, IPacketProperty? previousProperty, bool isLast, out IPacketProperty? packetProperty)
        {
            // Look for the attribute.
            var attributeSymbol = propSymbol.GetAttributes().FirstOrDefault(x =>
                x.AttributeClass is INamedTypeSymbol symbol &&
                x.AttributeClass is not IErrorTypeSymbol &&
                symbol.ToDisplayString() == "Enclave.FastPacket.Generator.PacketFieldAttribute");

            IPositionProvider? positionProvider = null;
            ISizeProvider? sizeProvider = null;
            PacketFieldOptions options = default;

            if (attributeSymbol is AttributeData attrData)
            {
                // Now we can determine the list of named properties on the attribute.
                foreach (var namedArg in attrData.NamedArguments)
                {
                    var argValue = namedArg.Value;

                    if (argValue.Kind == TypedConstantKind.Error || argValue.Value is null)
                    {
                        continue;
                    }

                    if (namedArg.Key == nameof(PacketFieldAttribute.Position))
                    {
                        options.Position = (int)argValue.Value;
                    }
                    else if (namedArg.Key == nameof(PacketFieldAttribute.Size))
                    {
                        options.Size = (int)argValue.Value;
                    }
                    else if (namedArg.Key == nameof(PacketFieldAttribute.PositionFunction))
                    {
                        options.PositionFunction = argValue.Value as string;
                    }
                    else if (namedArg.Key == nameof(PacketFieldAttribute.EnumBackingType))
                    {
                        options.EnumBackingType = argValue.Value as INamedTypeSymbol;
                    }
                }

                var syntaxRef = attributeSymbol.ApplicationSyntaxReference?.SyntaxTree.GetLocation(attributeSymbol.ApplicationSyntaxReference.Span);

                if (options.PositionFunction is string && TryGetPositionMethod(propSymbol.ContainingType, options.PositionFunction, syntaxRef, out var positionMethod))
                {
                    if (options.Position.HasValue)
                    {
                        positionProvider = new FunctionPositionExplicitDefaultProvider(positionMethod!, options.Position.Value);
                    }
                    else
                    {
                        positionProvider = new FunctionPositionAutomaticDefaultProvider(positionMethod!, previousProperty);
                    }
                }
                else if (options.Position.HasValue)
                {
                    positionProvider = new ConstantPositionProvider(options.Position.Value);
                }
            }

            if (positionProvider is null)
            {
                // Fall back to automatic position determination by default.
                positionProvider = new AutomaticPositionProvider(previousProperty);
            }

            if (options.Size.HasValue)
            {
                sizeProvider = new ExplicitSizeProvider(options.Size.Value);
            }

            IValueProvider? valueProvider = null;

            // If the type symbol is something that actually exists...
            if (propSymbol.Type is INamedTypeSymbol propType && propSymbol.Type is not IErrorTypeSymbol)
            {
                // Basic provider.
                if (_supportedNumericTypes.TryGetValue(propType, out var providers))
                {
                    valueProvider = providers.Value;
                    sizeProvider = providers.Size;
                }
                else if (propType.EnumUnderlyingType is not null)
                {
                    var customEnumBackingType = options.EnumBackingType;

                    var underlyingType = propType.EnumUnderlyingType;

                    if (customEnumBackingType is INamedTypeSymbol)
                    {
                        underlyingType = customEnumBackingType;
                    }

                    if (_supportedNumericTypes.TryGetValue(underlyingType, out var underlyingProviders))
                    {
                        valueProvider = new CastingValueProvider(propType, underlyingProviders.Value);

                        // Only use the enums' size if we haven't specified a specific size.
                        sizeProvider = underlyingProviders.Size;
                    }
                    else
                    {
                        _ctxt.ReportDiagnostic(Diagnostic.Create(Diagnostics.EnumUnderlyingTypeInvalid, propSymbol.Locations.First(), propType.Name, underlyingType.Name));
                    }
                }
                else if (propType.Equals(ReadOnlySpanByteType, SymbolEqualityComparer.Default) ||
                         propType.Equals(SpanByteType, SymbolEqualityComparer.Default))
                {
                    if (isLast)
                    {
                        valueProvider = _remainingSpanValueProvider;
                        sizeProvider = UnknownSizeProvider.Instance;
                    }
                    else if (options.Size.HasValue)
                    {
                        sizeProvider = new ExplicitSizeProvider(options.Size.Value);
                        valueProvider = new SpanKnownSizeValueProvider(SpanByteType, sizeProvider);
                    }
                    else
                    {
                        _ctxt.ReportDiagnostic(Diagnostic.Create(Diagnostics.SpanInMiddleOfPacketMustHaveSize, propSymbol.Locations.First(), propType.Name));
                    }
                }
                else
                {
                    var custom = GetCustomValueProvider(propType, propSymbol);

                    if (custom.Value is null)
                    {
                        if (sizeProvider is not null)
                        {
                            valueProvider = new CustomTypeValueProvider(propType, sizeProvider);
                        }
                        else
                        {
                            _ctxt.ReportDiagnostic(Diagnostic.Create(Diagnostics.CustomFieldTypeMustProvideLength, propSymbol.Locations.First(), propType.Name));
                        }
                    }
                    else
                    {
                        valueProvider = custom.Value;
                        sizeProvider = custom.Size;
                    }
                }
            }

            var docCommentXml = propSymbol.GetDocumentationCommentXml();

            IEnumerable<string> docComments;

            if (docCommentXml is string)
            {
                var splitComments = docCommentXml.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

                var justComments = splitComments.Skip(1).Take(splitComments.Length - 3).ToList();

                if (justComments.Count > 0)
                {
                    var firstTrimmed = justComments[0].TrimStart();
                    var trimSize = justComments[0].Length - firstTrimmed.Length;

                    justComments[0] = firstTrimmed;

                    for (int idx = 1; idx < justComments.Count; idx++)
                    {
                        justComments[idx] = justComments[idx].Substring(trimSize);
                    }
                }

                docComments = justComments;
            }
            else
            {
                docComments = Array.Empty<string>();
            }

            if (positionProvider is not null && valueProvider is not null && sizeProvider is not null)
            {
                packetProperty = new PacketProperty(propSymbol.Name, positionProvider, sizeProvider, valueProvider, docComments);
                return true;
            }

            packetProperty = null;
            return false;
        }

        private (IValueProvider? Value, ISizeProvider? Size) GetCustomValueProvider(INamedTypeSymbol type, IPropertySymbol propSymbol)
        {
            var diagnostics = ImmutableList<DiagnosticDescriptor>.Empty;

            if (_customProviderCache.TryGetValue(type, out var cachedEntry))
            {
                diagnostics = cachedEntry.Diagnostics;
            }
            else
            {
                // Validate the type.
                var constructor = type.Constructors.FirstOrDefault(m => !m.IsStatic && m.DeclaredAccessibility == Accessibility.Public && MethodParametersMatch(m, ReadOnlySpanByteType));

                if (constructor is null)
                {
                    diagnostics = diagnostics.Add(Diagnostics.CustomFieldTypeNoConstructor);
                }

                var copyTo = type.GetMembers("CopyTo").FirstOrDefault(m =>
                    m is IMethodSymbol method &&
                    !method.IsStatic &&
                    method.DeclaredAccessibility == Accessibility.Public &&
                    MethodParametersMatch(method, SpanByteType));

                if (copyTo is null)
                {
                    diagnostics = diagnostics.Add(Diagnostics.CustomFieldTypeNoCopyTo);
                }

                ISizeProvider? sizeProvider = null;

                ISymbol? GetSizeConstantMember(INamedTypeSymbol type)
                {
                    return type.GetMembers("Size").FirstOrDefault(m =>
                                        m is IFieldSymbol field &&
                                        field.IsConst &&
                                        field.DeclaredAccessibility == Accessibility.Public &&
                                        field.Type.Equals(_ctxt.Compilation.GetSpecialType(SpecialType.System_Int32), SymbolEqualityComparer.Default));
                }

                ISymbol? GetSizeMethodMember(INamedTypeSymbol type)
                {
                    return type.GetMembers("GetSize").FirstOrDefault(m =>
                                        m is IMethodSymbol method &&
                                        method.IsStatic &&
                                        method.DeclaredAccessibility == Accessibility.Public &&
                                        MethodParametersMatch(method, ReadOnlySpanByteType));
                }

                if (GetSizeConstantMember(type) is IFieldSymbol)
                {
                    // No size constant, but that may be OK.
                    sizeProvider = new CustomTypeConstantSizeProvider(type);
                }
                else if (GetSizeMethodMember(type) is IMethodSymbol)
                {
                    sizeProvider = new CustomTypeMethodSizeProvider(type);
                }

                if (sizeProvider is not null)
                {
                    cachedEntry = new CustomProviderResult(new CustomTypeValueProvider(type, sizeProvider), sizeProvider, diagnostics);
                }
                else
                {
                    // Add a cached entry with no value provider, because the size must be provided externally.
                    cachedEntry = new CustomProviderResult(null, null, diagnostics);
                }

                _customProviderCache.Add(type, cachedEntry);
            }

            foreach (var diag in cachedEntry.Diagnostics)
            {
                _ctxt.ReportDiagnostic(Diagnostic.Create(diag, propSymbol.Locations.First(), type.Name));
            }

            return (cachedEntry.Value, cachedEntry.Size);
        }

        private bool TryGetPositionMethod(INamedTypeSymbol containingType, string declaredName, Location? diagnosticLocation, out IMethodSymbol? positionMethod)
        {
            var foundMembers = containingType.GetMembers(declaredName);
            DiagnosticDescriptor? diagnosticMessage = Diagnostics.PositionFunctionIsNotFound;
            positionMethod = null;

            foreach (var member in foundMembers)
            {
                if (member is IMethodSymbol methodSymbol)
                {
                    if (!methodSymbol.IsStatic || methodSymbol.DeclaredAccessibility != Accessibility.Public)
                    {
                        diagnosticMessage = Diagnostics.PositionFunctionIsNotPublicStatic;
                        continue;
                    }

                    var methodArgs = methodSymbol.Parameters;
                    diagnosticMessage = Diagnostics.PositionFunctionUnexpectedSignature;

                    if (methodArgs.Length == 2)
                    {
                        // Should be 2 arguments.
                        var spanArg = methodArgs[0];
                        var positionArg = methodArgs[1];

                        if (spanArg.Type is INamedTypeSymbol spanTypeSymbol &&
                            SymbolEqualityComparer.Default.Equals(spanTypeSymbol, ReadOnlySpanByteType) &&
                            positionArg.Type is INamedTypeSymbol posTypeSymbol &&
                            SymbolEqualityComparer.Default.Equals(posTypeSymbol, _ctxt.Compilation.GetSpecialType(SpecialType.System_Int32)) &&
                            methodSymbol.ReturnType is INamedTypeSymbol returnSymbol &&
                            SymbolEqualityComparer.Default.Equals(returnSymbol, _ctxt.Compilation.GetSpecialType(SpecialType.System_Int32)))
                        {
                            // Looks good. Choose this one.
                            // No errors.
                            diagnosticMessage = null;
                            positionMethod = methodSymbol;
                        }
                    }
                }
            }

            if (diagnosticMessage is DiagnosticDescriptor)
            {
                _ctxt.ReportDiagnostic(Diagnostic.Create(diagnosticMessage, diagnosticLocation, declaredName));
                return false;
            }

            return true;
        }

        private bool MethodParametersMatch(IMethodSymbol method, params INamedTypeSymbol[] expected)
        {
            if (method.Parameters.Length != expected.Length)
            {
                return false;
            }

            for (int idx = 0; idx < expected.Length; idx++)
            {
                var arg = method.Parameters[idx];
                var expectedArg = expected[idx];

                // If the types do not match, then fail.
                if (arg.Type is not INamedTypeSymbol spanTypeSymbol ||
                    !SymbolEqualityComparer.Default.Equals(spanTypeSymbol, expectedArg))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
