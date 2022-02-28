using System;
using Enclave.FastPacket.Generator;

namespace Enclave.FastPacket;

internal ref struct Ipv6HopByHopAndDestinationExtension
{
    [PacketField(EnumBackingType = typeof(byte))]
    public IpProtocol NextHeader { get; set; }

    public byte HeaderExtensionLength { get; set; }

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

[PacketImplementation(typeof(Ipv6HopByHopAndDestinationExtension), IsReadOnly = true)]
public readonly ref partial struct ReadOnlyIpv6HopByHopAndDestinationExtensionSpan
{
}
