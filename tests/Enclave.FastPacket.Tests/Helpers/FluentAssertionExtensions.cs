using FluentAssertions;
using FluentAssertions.Collections;
using System;

namespace Enclave.FastPacket.Tests;

internal static class FluentAssertionExtensions
{
    public static GenericCollectionAssertions<byte> Should(this ReadOnlySpan<byte> span)
    {
        return span.ToArray().Should();
    }

    public static GenericCollectionAssertions<byte> Should(this Span<byte> span)
    {
        return span.ToArray().Should();
    }
}
