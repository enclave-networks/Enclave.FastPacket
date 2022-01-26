using Enclave.FastPacket.Generator;

namespace Enclave.FastPacket.Icmp;

internal struct Icmpv4TimestampDefinition
{
    public Icmpv4Types Type { get; set; }

    public byte Code { get; set; }

    public ushort Checksum { get; set; }

    public ushort Identifier { get; set; }

    public ushort SequenceNumber { get; set; }

    public uint OrginateTimeStamp { get; set; }

    public uint ReceiveTimeStamp { get; set; }

    public uint TransmitTimeStamp { get; set; }
}

[PacketImplementation(typeof(Icmpv4TimestampDefinition))]
public readonly ref partial struct Icmpv4TimestampSpan
{
}

[PacketImplementation(typeof(Icmpv4TimestampDefinition), IsReadOnly = true)]
public readonly ref partial struct Icmpv4TimestampReadOnlySpan
{
}