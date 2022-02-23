using Enclave.FastPacket.Generator;

namespace Enclave.FastPacket.Icmp;

internal struct Icmpv4TimestampDefinition
{
    /// <summary>
    /// Indicates the type of the ICMP packet.
    /// </summary>
    public Icmpv4Types Type { get; set; }

    /// <summary>
    /// The code.
    /// </summary>
    public byte Code { get; set; }

    /// <summary>
    /// The checksum.
    /// </summary>
    public ushort Checksum { get; set; }

    /// <summary>
    /// The identifier.
    /// </summary>
    public ushort Identifier { get; set; }

    /// <summary>
    /// The sequence number.
    /// </summary>
    public ushort SequenceNumber { get; set; }

    /// <summary>
    /// The originating timestamp.
    /// </summary>
    public uint OrginateTimeStamp { get; set; }

    /// <summary>
    /// The receive timestamp.
    /// </summary>
    public uint ReceiveTimeStamp { get; set; }

    /// <summary>
    /// The transmit timestamp.
    /// </summary>
    public uint TransmitTimeStamp { get; set; }
}

/// <summary>
/// Provides a read-write decoder for an ICMP Timestamp packet.
/// </summary>
[PacketImplementation(typeof(Icmpv4TimestampDefinition))]
public readonly ref partial struct Icmpv4TimestampSpan
{
}

/// <summary>
/// Provides a read-only decoder for an ICMP Timestamp packet.
/// </summary>
[PacketImplementation(typeof(Icmpv4TimestampDefinition), IsReadOnly = true)]
public readonly ref partial struct ReadOnlyIcmpv4TimestampSpan
{
    /// <summary>
    /// Convert to a readonly representation.
    /// </summary>
    public static implicit operator ReadOnlyIcmpv4TimestampSpan(Icmpv4TimestampSpan s)
        => new ReadOnlyIcmpv4TimestampSpan(s.GetRawData());
}