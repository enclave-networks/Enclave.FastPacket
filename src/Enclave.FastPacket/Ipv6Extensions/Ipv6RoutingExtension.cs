using System;
using Enclave.FastPacket.Generator;

namespace Enclave.FastPacket;

internal ref struct Ipv6RoutingExtension
{
    [PacketField(EnumBackingType = typeof(byte))]
    public IpProtocol NextHeader { get; set; }

    public byte HeaderExtensionLength { get; set; }

    public byte RoutingType { get; set; }

    public byte SegmentsLeft { get; set; }

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

[PacketImplementation(typeof(Ipv6RoutingExtension), IsReadOnly = true)]
public readonly ref partial struct ReadOnlyIpv6RoutingExtensionSpan
{
}
