using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Enclave.FastPacket.Generator.SizeProviders;
using Microsoft.CodeAnalysis;

namespace Enclave.FastPacket.Generator.ValueProviders;

internal static class BitMaskHelpers
{
    private static ReadOnlySpan<byte> TrailingZeroCountDeBruijn => new byte[32]
    {
            00, 01, 28, 02, 29, 14, 24, 03,
            30, 22, 20, 15, 25, 17, 04, 08,
            31, 27, 13, 23, 21, 19, 16, 07,
            26, 12, 18, 06, 11, 05, 10, 09,
    };

    private static ReadOnlySpan<byte> Log2DeBruijn => new byte[32]
    {
            00, 09, 01, 10, 13, 21, 02, 29,
            11, 14, 16, 18, 22, 25, 03, 30,
            08, 12, 20, 28, 15, 17, 24, 07,
            19, 27, 23, 06, 26, 05, 04, 31,
    };

    public static int TrailingZeroCount(ulong value)
    {
        uint lo = (uint)value;

        if (lo == 0)
        {
            return 32 + TrailingZeroCount((uint)(value >> 32));
        }

        return TrailingZeroCount(lo);
    }

    public static int TrailingZeroCount(uint value)
    {
        // Unguarded fallback contract is 0->0, BSF contract is 0->undefined
        if (value == 0)
        {
            return 32;
        }

        // uint.MaxValue >> 27 is always in range [0 - 31] so we use Unsafe.AddByteOffset to avoid bounds check
        return Unsafe.AddByteOffset(
            // Using deBruijn sequence, k=2, n=5 (2^5=32) : 0b_0000_0111_0111_1100_1011_0101_0011_0001u
            ref MemoryMarshal.GetReference(TrailingZeroCountDeBruijn),
            // uint|long -> IntPtr cast on 32-bit platforms does expensive overflow checks not needed here
            (IntPtr)(int)(((value & (uint)-(int)value) * 0x077CB531u) >> 27)); // Multi-cast mitigates redundant conv.u8
    }

    public static int LeadingZeroCount(ulong value)
    {
        uint hi = (uint)(value >> 32);

        if (hi == 0)
        {
            return 32 + LeadingZeroCount((uint)value);
        }

        return LeadingZeroCount(hi);
    }

    public static int LeadingZeroCount(uint value)
    {
        // Unguarded fallback contract is 0->31, BSR contract is 0->undefined
        if (value == 0)
        {
            return 32;
        }

        return 31 ^ Log2(value);
    }

    /// <summary>
    /// Returns the integer (floor) log of the specified value, base 2.
    /// Note that by convention, input value 0 returns 0 since Log(0) is undefined.
    /// Does not directly use any hardware intrinsics, nor does it incur branching.
    /// </summary>
    /// <param name="value">The value.</param>
    private static int Log2(uint value)
    {
        // No AggressiveInlining due to large method size
        // Has conventional contract 0->0 (Log(0) is undefined)

        // Fill trailing zeros with ones, eg 00010010 becomes 00011111
        value |= value >> 01;
        value |= value >> 02;
        value |= value >> 04;
        value |= value >> 08;
        value |= value >> 16;

        // uint.MaxValue >> 27 is always in range [0 - 31] so we use Unsafe.AddByteOffset to avoid bounds check
        return Unsafe.AddByteOffset(
            // Using deBruijn sequence, k=2, n=5 (2^5=32) : 0b_0000_0111_1100_0100_1010_1100_1101_1101u
            ref MemoryMarshal.GetReference(Log2DeBruijn),
            // uint|long -> IntPtr cast on 32-bit platforms does expensive overflow checks not needed here
            (IntPtr)(int)((value * 0x07C4ACDDu) >> 27));
    }

}

internal class BitmaskWrapperValueProvider : IValueProvider
{
    private readonly ulong _bitmask;
    private readonly IValueProvider _wrappedProvider;
    private readonly ISizeProvider _sizeProvider;

    private static ReadOnlySpan<byte> TrailingZeroCountDeBruijn => new byte[32]
    {
            00, 01, 28, 02, 29, 14, 24, 03,
            30, 22, 20, 15, 25, 17, 04, 08,
            31, 27, 13, 23, 21, 19, 16, 07,
            26, 12, 18, 06, 11, 05, 10, 09,
    };

    private static ReadOnlySpan<byte> Log2DeBruijn => new byte[32]
    {
            00, 09, 01, 10, 13, 21, 02, 29,
            11, 14, 16, 18, 22, 25, 03, 30,
            08, 12, 20, 28, 15, 17, 24, 07,
            19, 27, 23, 06, 26, 05, 04, 31,
    };

    public BitmaskWrapperValueProvider(ulong bitmask, IValueProvider wrappedProvider, ISizeProvider sizeProvider)
    {
        _bitmask = bitmask;
        _wrappedProvider = wrappedProvider;
        _sizeProvider = sizeProvider;
    }

    public INamedTypeSymbol TypeSymbol => _wrappedProvider.TypeSymbol;

    public bool CanSet => _wrappedProvider.CanSet;

    public string TypeReferenceName => _wrappedProvider.TypeReferenceName;

    public string GetPropGetExpression(string spanName, string positionExpression)
    {
        var maskOffset = TrailingZeroCount(_bitmask);

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
        var maskOffset = TrailingZeroCount(_bitmask);

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

    private static int TrailingZeroCount(ulong value)
    {
        uint lo = (uint)value;

        if (lo == 0)
        {
            return 32 + TrailingZeroCount((uint)(value >> 32));
        }

        return TrailingZeroCount(lo);
    }

    private static int TrailingZeroCount(uint value)
    {
        // Unguarded fallback contract is 0->0, BSF contract is 0->undefined
        if (value == 0)
        {
            return 32;
        }

        // uint.MaxValue >> 27 is always in range [0 - 31] so we use Unsafe.AddByteOffset to avoid bounds check
        return Unsafe.AddByteOffset(
            // Using deBruijn sequence, k=2, n=5 (2^5=32) : 0b_0000_0111_0111_1100_1011_0101_0011_0001u
            ref MemoryMarshal.GetReference(TrailingZeroCountDeBruijn),
            // uint|long -> IntPtr cast on 32-bit platforms does expensive overflow checks not needed here
            (IntPtr)(int)(((value & (uint)-(int)value) * 0x077CB531u) >> 27)); // Multi-cast mitigates redundant conv.u8
    }
}
