using Enclave.FastPacket.Generator;

namespace Enclave.FastPacket.Icmp;

internal struct Icmpv4AddressMaskDefinition
{
    /// <summary>
    /// Indicates the type of the ICMP packet.
    /// </summary>
    public Icmpv4Types Type { get; set; }

    /// <summary>
    /// The ICMP Code.
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
    /// The address mask.
    /// </summary>
    public uint AddressMask { get; set; }
}

/// <summary>
/// Provides a read-write decoder for an ICMP Address Mask packet.
/// </summary>
[PacketImplementation(typeof(Icmpv4AddressMaskDefinition))]
public readonly ref partial struct Icmpv4AddressMaskSpan
{
}

/// <summary>
/// Provides a read-only decoder for an ICMP Address Mask packet.
/// </summary>
[PacketImplementation(typeof(Icmpv4AddressMaskDefinition), IsReadOnly = true)]
public readonly ref partial struct ReadOnlyIcmpv4AddressMaskSpan
{
    /// <summary>
    /// Convert to a readonly representation.
    /// </summary>
    public static implicit operator ReadOnlyIcmpv4AddressMaskSpan(Icmpv4AddressMaskSpan s)
        => new ReadOnlyIcmpv4AddressMaskSpan(s.GetRawData());
}