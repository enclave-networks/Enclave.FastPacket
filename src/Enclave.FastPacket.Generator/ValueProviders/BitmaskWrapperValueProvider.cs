using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Enclave.FastPacket.Generator.SizeProviders;
using Microsoft.CodeAnalysis;

namespace Enclave.FastPacket.Generator.ValueProviders;

internal class BitmaskWrapperValueProvider : IValueProvider
{
    private readonly ulong _bitmask;
    private readonly IValueProvider _wrappedProvider;

    public BitmaskWrapperValueProvider(ulong bitmask, IValueProvider wrappedProvider)
    {
        _bitmask = bitmask;
        _wrappedProvider = wrappedProvider;
    }

    public INamedTypeSymbol TypeSymbol => _wrappedProvider.TypeSymbol;

    public bool CanSet => _wrappedProvider.CanSet;

    public string TypeReferenceName => _wrappedProvider.TypeReferenceName;

    public string GetPropGetExpression(string spanName, string positionExpression)
    {
        var maskOffset = BitmaskHelpers.TrailingZeroCount(_bitmask);

        if (maskOffset == 0)
        {
            // We read the underlying value from the wrapped provider, then we AND it with the bitmask, and shift right the value
            // to the 'start' of the mask as it would be entered in an attribute. This gives the most 'natural' expectation
            // of the returned value.
            return $"({_wrappedProvider.TypeReferenceName})({_wrappedProvider.GetPropGetExpression(spanName, positionExpression)}" +
                   $" & 0x{_bitmask:X}u)";
        }
        else
        {
            // We read the underlying value from the wrapped provider, then we AND it with the bitmask, and shift right the value
            // to the 'start' of the mask as it would be entered in an attribute. This gives the most 'natural' expectation
            // of the returned value.
            return $"({_wrappedProvider.TypeReferenceName})(({_wrappedProvider.GetPropGetExpression(spanName, positionExpression)}" +
                   $" & 0x{_bitmask:X}u) >> {maskOffset})";
        }
    }

    public string GetPropSetExpression(string spanName, string positionExpression, string valueExpression)
    {
        var maskOffset = BitmaskHelpers.TrailingZeroCount(_bitmask);

        var existingValueExpr = _wrappedProvider.GetPropGetExpression(spanName, positionExpression);

        // Shift left to push the value into the correct position.
        // AND with the bitmask again to make sure we can only overwrite bits
        // our bitmask allows.
        var valueExpr = maskOffset == 0 ?
            $"({valueExpression} & 0x{_bitmask:X}u)" :
            $"(({valueExpression} << {maskOffset}) & 0x{_bitmask:X}u)";

        // Now we combine with the existing value (after masking out the existing bits for the section of the value we use).
        valueExpr = $"({_wrappedProvider.TypeReferenceName})({valueExpr} | ({_wrappedProvider.TypeReferenceName})({existingValueExpr} & ~0x{_bitmask:X}u))";

        // And finally we write it back.
        return _wrappedProvider.GetPropSetExpression(spanName, positionExpression, valueExpr);
    }
}
