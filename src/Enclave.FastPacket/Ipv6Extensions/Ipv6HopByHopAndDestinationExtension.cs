using System;
using Enclave.FastPacket.Generator;

namespace Enclave.FastPacket;

internal ref struct Ipv6HopByHopAndDestinationExtension
{
    /// <summary>
    /// The next header after this extension.
    /// </summary>
    [PacketField(EnumBackingType = typeof(byte))]
    public IpProtocol NextHeader { get; set; }

    /// <summary>
    /// The length of this extension header, excluding the first 8 bytes.
    /// </summary>
    public byte HeaderExtensionLength { get; set; }

    /// <summary>
    /// The options and padding included in this header.
    /// </summary>
    [PacketField(SizeFunction = nameof(GetOptionsSize))]
    public ReadOnlySpan<byte> OptionsAndPadding { get; set; }

    public static int GetOptionsSize(ReadOnlySpan<byte> buffer)
    {
        // HeaderExtensionLength excludes the first 8 bytes.
        var totalSize = new ReadOnlyIpv6HopByHopAndDestinationExtensionSpan(buffer).HeaderExtensionLength + 8;

        // Options data is the total size minus the minimum known size.
        return totalSize - ReadOnlyIpv6HopByHopAndDestinationExtensionSpan.MinimumSize;
    }
}

/// <summary>
/// A read-write decoder for an IPv6 hop-by-hop or destination extension header.
/// </summary>
[PacketImplementation(typeof(Ipv6HopByHopAndDestinationExtension), IsReadOnly = true)]
public readonly ref partial struct ReadOnlyIpv6HopByHopAndDestinationExtensionSpan
{
}
