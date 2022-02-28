using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using Enclave.FastPacket.Generator.PositionProviders;
using Enclave.FastPacket.Generator.SizeProviders;
using Enclave.FastPacket.Generator.ValueProviders;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FlowAnalysis;

namespace Enclave.FastPacket.Generator;

internal class PacketPropertyFactory
{
    private readonly GeneratorExecutionContext _ctxt;

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

    private INamedTypeSymbol GetSpecialType(SpecialType specialType)
        => _ctxt.Compilation.GetSpecialType(specialType);

    public INamedTypeSymbol ReadOnlySpanByteType { get; }

    public INamedTypeSymbol SpanByteType { get; }

    public bool TryCreateUnion(INamedTypeSymbol definitionType, INamedTypeSymbol unionType, IPacketProperty? previousProperty, out IPacketProperty? virtualUnionProperty)
    {
        Location defLocation = unionType.Locations.First();
        Location configurationLocation = defLocation;

        var options = GetPacketFieldOptions(unionType, ref configurationLocation);

        var (positionProvider, sizeProvider) = GetDefaultSizeAndPositionProvider(definitionType, previousProperty, configurationLocation, options);

        if (sizeProvider is null)
        {
            _ctxt.ReportDiagnostic(Diagnostic.Create(Diagnostics.UnionsShouldHaveDeclaredSize, configurationLocation, unionType.Name));

            virtualUnionProperty = null;
            return false;
        }

        IEnumerable<string> docComments = GetDocComments(unionType);

        virtualUnionProperty = new VirtualUnionProperty(unionType.Name, positionProvider, sizeProvider, docComments);
        return true;
    }

    public bool TryCreate(INamedTypeSymbol definitionType, IPropertySymbol propSymbol, IPacketProperty? previousProperty, bool isLast, out IPacketProperty? packetProperty)
    {
        Location propLocation = propSymbol.Locations.First();
        Location configurationLocation = propLocation;

        var options = GetPacketFieldOptions(propSymbol, ref configurationLocation);

        var (positionProvider, sizeProvider) = GetDefaultSizeAndPositionProvider(definitionType, previousProperty, configurationLocation, options);

        IValueProvider? valueProvider = null;

        // Default to not supporting a bitmask unless we actually do.
        bool bitMaskNotSupported = options.Bitmask.HasValue;

        // If the type symbol is something that actually exists...
        if (propSymbol.Type is INamedTypeSymbol propType && propSymbol.Type is not IErrorTypeSymbol)
        {
            var readType = propType;

            if (options.Bitmask.HasValue)
            {
                // The actual 'thing' we use is going to be based on the position of the last bits of the
                // bitmask.
                var lastBit = 63 - BitmaskHelpers.LeadingZeroCount(options.Bitmask.Value);

                if (lastBit < 8)
                {
                    readType = GetSpecialType(SpecialType.System_Byte);
                }
                else if (lastBit < 16)
                {
                    readType = GetSpecialType(SpecialType.System_UInt16);
                }
                else if (lastBit < 32)
                {
                    readType = GetSpecialType(SpecialType.System_UInt32);
                }
                else if (lastBit < 64)
                {
                    readType = GetSpecialType(SpecialType.System_UInt64);
                }
                else
                {
                    // Diagnostic?
                }
            }

            // Basic provider.
            if (_supportedNumericTypes.TryGetValue(readType, out var providers))
            {
                valueProvider = providers.Value;

                sizeProvider ??= providers.Size;

                if (options.Bitmask.HasValue)
                {
                    valueProvider = new BitmaskWrapperValueProvider(options.Bitmask.Value, valueProvider);
                }

                if (!SymbolEqualityComparer.Default.Equals(readType, propType))
                {
                    // Is boolean?
                    if (SymbolEqualityComparer.Default.Equals(propType, GetSpecialType(SpecialType.System_Boolean)))
                    {
                        valueProvider = new BoolFromNumberValueProvider(propType, valueProvider);
                    }
                    else
                    {
                        // Cast it.
                        valueProvider = new CastingValueProvider(propType, valueProvider);
                    }
                }
            }
            else if (propType.EnumUnderlyingType is not null)
            {
                if (!options.Bitmask.HasValue)
                {
                    readType = propType.EnumUnderlyingType;

                    if (options.EnumBackingType is INamedTypeSymbol)
                    {
                        readType = options.EnumBackingType;
                    }
                }

                if (_supportedNumericTypes.TryGetValue(readType, out var underlyingProviders))
                {
                    // Only use the enums' size if we haven't specified a specific size.
                    sizeProvider ??= underlyingProviders.Size;

                    var enumValueProvider = underlyingProviders.Value;

                    if (options.Bitmask.HasValue)
                    {
                        enumValueProvider = new BitmaskWrapperValueProvider(options.Bitmask.Value, enumValueProvider);
                    }

                    valueProvider = new CastingValueProvider(propType, enumValueProvider);
                }
                else
                {
                    _ctxt.ReportDiagnostic(Diagnostic.Create(Diagnostics.EnumUnderlyingTypeInvalid, propSymbol.Locations.First(), propType.Name, readType.Name));
                }
            }
            else if (propType.Equals(ReadOnlySpanByteType, SymbolEqualityComparer.Default) ||
                     propType.Equals(SpanByteType, SymbolEqualityComparer.Default))
            {
                if (sizeProvider is null)
                {
                    if (isLast)
                    {
                        valueProvider = _remainingSpanValueProvider;
                        sizeProvider ??= SpanRemainingLengthSizeProvider.Instance;
                    }
                    else
                    {
                        _ctxt.ReportDiagnostic(Diagnostic.Create(Diagnostics.SpanInMiddleOfPacketMustHaveSize, propSymbol.Locations.First(), propType.Name));
                    }
                }
                else
                {
                    valueProvider = new SpanKnownSizeValueProvider(SpanByteType, sizeProvider);
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
                    sizeProvider ??= custom.Size;
                }
            }
        }

        IEnumerable<string> docComments = GetDocComments(propSymbol);

        if (positionProvider is not null && valueProvider is not null && sizeProvider is not null)
        {
            packetProperty = new PacketProperty(propSymbol.Name, propSymbol.DeclaredAccessibility, options, positionProvider, sizeProvider, valueProvider, docComments);
            return true;
        }

        packetProperty = null;
        return false;
    }

    private (IPositionProvider Position, ISizeProvider? Size) GetDefaultSizeAndPositionProvider(INamedTypeSymbol definitionType, IPacketProperty? previousProperty, Location configurationLocation, PacketFieldOptions options)
    {
        IPositionProvider? positionProvider = null;
        ISizeProvider? sizeProvider = null;

        if (options.PositionFunction is string && TryGetPositionMethod(definitionType, options.PositionFunction, configurationLocation, out var positionMethod))
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
            positionProvider = new ExplicitPositionProvider(options.Position.Value);
        }

        if (positionProvider is null)
        {
            // Fall back to automatic position determination by default.
            positionProvider = new AutomaticPositionProvider(previousProperty);
        }

        if (options.SizeFunction is string && TryGetSizeMethodProvider(definitionType, options.SizeFunction, configurationLocation, out var funcSizeProvider))
        {
            sizeProvider = funcSizeProvider;
        }
        else if (options.Size.HasValue)
        {
            sizeProvider = new ExplicitSizeProvider(options.Size.Value);
        }

        return (positionProvider, sizeProvider);
    }

    private static IEnumerable<string> GetDocComments(ISymbol symbol)
    {
        var docCommentXml = symbol.GetDocumentationCommentXml();

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

        return docComments;
    }

    private static PacketFieldOptions GetPacketFieldOptions(ISymbol owningSymbol, ref Location configurationLocation)
    {
        var options = new PacketFieldOptions();

        var packetFieldAttr = owningSymbol.GetAttributes().FirstOrDefault(x =>
            x.AttributeClass is INamedTypeSymbol symbol &&
            x.AttributeClass is not IErrorTypeSymbol &&
            symbol.ToDisplayString() == "Enclave.FastPacket.Generator.PacketFieldAttribute");

        if (packetFieldAttr is AttributeData attrData)
        {
            // Now we can determine the list of named properties on the attribute.
            foreach (var namedArg in attrData.NamedArguments)
            {
                var argValue = namedArg.Value;

                if (argValue.Kind == TypedConstantKind.Error || argValue.Value is null)
                {
                    continue;
                }

                switch (namedArg.Key)
                {
                    case nameof(PacketFieldAttribute.Position):
                        options.Position = (int)argValue.Value;
                        break;
                    case nameof(PacketFieldAttribute.Size):
                        options.Size = (int)argValue.Value;
                        break;
                    case nameof(PacketFieldAttribute.PositionFunction):
                        options.PositionFunction = argValue.Value as string;
                        break;
                    case nameof(PacketFieldAttribute.EnumBackingType):
                        options.EnumBackingType = argValue.Value as INamedTypeSymbol;
                        break;
                    case nameof(PacketFieldAttribute.Bitmask):
                        options.Bitmask = (ulong)argValue.Value;
                        break;
                    case nameof(PacketFieldAttribute.SizeFunction):
                        options.SizeFunction = argValue.Value as string;
                        break;
                }
            }

            var foundLocation = packetFieldAttr.ApplicationSyntaxReference?.SyntaxTree.GetLocation(packetFieldAttr.ApplicationSyntaxReference.Span);

            if (foundLocation is not null)
            {
                configurationLocation = foundLocation;
            }
        }

        var packetFieldBitsAttr = owningSymbol.GetAttributes().FirstOrDefault(x =>
            x.AttributeClass is INamedTypeSymbol symbol &&
            x.AttributeClass is not IErrorTypeSymbol &&
            symbol.ToDisplayString() == "Enclave.FastPacket.Generator.PacketFieldBitsAttribute");

        if (packetFieldBitsAttr is AttributeData)
        {
            static bool TryGetIntArg(TypedConstant arg, out int value)
            {
                if (arg.Kind != TypedConstantKind.Error && arg.Value is uint found)
                {
                    value = (int)found;
                    return true;
                }

                value = 0;
                return false;
            }

            var constructArgs = packetFieldBitsAttr.ConstructorArguments;

            if (constructArgs.Length > 0)
            {
                if (TryGetIntArg(constructArgs[0], out var firstBit))
                {
                    var lastBit = firstBit;

                    // If we've either got only 1 constructor arg, or we've got a second "last" bit.
                    if (constructArgs.Length == 1 || TryGetIntArg(packetFieldBitsAttr.ConstructorArguments[1], out lastBit))
                    {
                        if (lastBit >= firstBit)
                        {
                            // Intentional int arithmetic, only care about the whole bytes.
                            var numberOfWholeBytes = lastBit / 8;

                            var lastBitInMask = (8 * (numberOfWholeBytes + 1)) - 1;

                            // Generate the bitmask for MSB0 designation.
                            // This is when we invert the bit order.
                            ulong bitMask = 0;

                            for (int bitPos = lastBitInMask - firstBit; bitPos >= lastBitInMask - lastBit; bitPos--)
                            {
                                bitMask |= 1UL << bitPos;
                            }

                            options.Bitmask = bitMask;
                        }
                        else
                        {
                            // Raise a diagnostic.
                        }
                    }
                }
            }
        }

        return options;
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

                if (MethodParametersMatch(methodSymbol, ReadOnlySpanByteType, _ctxt.Compilation.GetSpecialType(SpecialType.System_Int32)) &&
                    MethodReturnTypeMatches(methodSymbol, _ctxt.Compilation.GetSpecialType(SpecialType.System_Int32)))
                {
                    // Looks good. Choose this one.
                    // No errors.
                    diagnosticMessage = null;
                    positionMethod = methodSymbol;
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

    private bool TryGetSizeMethodProvider(INamedTypeSymbol containingType, string declaredName, Location? diagnosticLocation, out ISizeProvider? sizeProvider)
    {
        var foundMembers = containingType.GetMembers(declaredName);
        DiagnosticDescriptor? diagnosticMessage = Diagnostics.SizeFunctionIsNotFound;
        sizeProvider = null;

        foreach (var member in foundMembers)
        {
            if (member is IMethodSymbol methodSymbol)
            {
                if (!methodSymbol.IsStatic || methodSymbol.DeclaredAccessibility != Accessibility.Public)
                {
                    diagnosticMessage = Diagnostics.SizeFunctionIsNotPublicStatic;
                    continue;
                }

                diagnosticMessage = Diagnostics.SizeFunctionUnexpectedSignature;

                // Size methods can optionally take the position of the field as an int if they need to, but it's optional.
                // Not much point computing and passing that position if it's not required.
                if ((MethodParametersMatch(methodSymbol, ReadOnlySpanByteType, _ctxt.Compilation.GetSpecialType(SpecialType.System_Int32)) ||
                    MethodParametersMatch(methodSymbol, ReadOnlySpanByteType)) &&
                    MethodReturnTypeMatches(methodSymbol, _ctxt.Compilation.GetSpecialType(SpecialType.System_Int32)))
                {
                    // Looks good. Choose this one.
                    // No errors.
                    diagnosticMessage = null;
                    sizeProvider = new FunctionSizeProvider(methodSymbol);
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

    private static bool MethodParametersMatch(IMethodSymbol method, params INamedTypeSymbol[] expected)
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

    private static bool MethodReturnTypeMatches(IMethodSymbol method, INamedTypeSymbol returnType)
    {
        if (method.ReturnsVoid)
        {
            return false;
        }

        return SymbolEqualityComparer.Default.Equals(returnType, method.ReturnType);
    }
}
