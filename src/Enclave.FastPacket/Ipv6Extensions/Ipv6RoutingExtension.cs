using System;
using Enclave.FastPacket.Generator;

namespace Enclave.FastPacket;

internal ref struct Ipv6RoutingExtension
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
    /// The routing type.
    /// </summary>
    public byte RoutingType { get; set; }

    /// <summary>
    /// The remaining segments.
    /// </summary>
    public byte SegmentsLeft { get; set; }

    /// <summary>
    /// Additional type data.
    /// </summary>
    [PacketField(SizeFunction = nameof(GetTypeDataSize))]
    public ReadOnlySpan<byte> TypeData { get; set; }

    public static int GetTypeDataSize(ReadOnlySpan<byte> buffer)
    {
        // HeaderExtensionLength excludes the first 8 bytes.
        var totalSize = new ReadOnlyIpv6RoutingExtensionSpan(buffer).HeaderExtensionLength + 8;

        // Type data size is the total size minus the known preceding segments.
        return totalSize - ReadOnlyIpv6RoutingExtensionSpan.MinimumSize;
    }
}

/// <summary>
/// A read-write decoder for an IPv6 routing extension header.
/// </summary>
[PacketImplementation(typeof(Ipv6RoutingExtension), IsReadOnly = true)]
public readonly ref partial struct ReadOnlyIpv6RoutingExtensionSpan
{
}
