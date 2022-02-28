using Enclave.FastPacket.Generator;

namespace Enclave.FastPacket;

internal ref struct Ipv6FragmentExtension
{
    [PacketField(EnumBackingType = typeof(byte))]
    public IpProtocol NextHeader { get; set; }

    private byte Reserved { get; set; }

    [PacketField(Size = sizeof(ushort))]
    public struct OffsetUnion
    {
        [PacketFieldBits(0, 12)]
        public ushort FragmentOffset { get; set; }

        [PacketFieldBits(15)]
        public bool MoreFragments { get; set; }
    }

    public uint Identification { get; set; }
}

[PacketImplementation(typeof(Ipv6FragmentExtension), IsReadOnly = true)]
public readonly ref partial struct ReadOnlyIpv6FragmentExtensionSpan
{
}
