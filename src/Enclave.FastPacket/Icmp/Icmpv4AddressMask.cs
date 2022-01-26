using Enclave.FastPacket.Generator;

namespace Enclave.FastPacket.Icmp;

internal struct Icmpv4AddressMaskDefinition
{
    public Icmpv4Types Type { get; set; }

    public byte Code { get; set; }

    public ushort Checksum { get; set; }

    public ushort Identifier { get; set; }

    public ushort SequenceNumber { get; set; }

    public uint AddressMask { get; set; }
}

[PacketImplementation(typeof(Icmpv4AddressMaskDefinition))]
public readonly ref partial struct Icmpv4AddressMaskSpan
{
}

[PacketImplementation(typeof(Icmpv4AddressMaskDefinition), IsReadOnly = true)]
public readonly ref partial struct Icmpv4AddressMaskReadOnlySpan
{
}