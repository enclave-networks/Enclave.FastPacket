using System;
using Enclave.FastPacket.Generator;

namespace Enclave.FastPacket.Icmp;

internal ref struct Icmpv4SourceQuenchDefinition
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

    private uint Unused { get; set; }

    /// <summary>
    /// The failed IP header and datagram.
    /// </summary>
    public ReadOnlySpan<byte> IpHeaderAndDatagram { get; set; }
}

/// <summary>
/// Provides a read-write decoder for an ICMP Source Quench packet.
/// </summary>
[PacketImplementation(typeof(Icmpv4SourceQuenchDefinition))]
public readonly ref partial struct Icmpv4SourceQuenchSpan
{
}

/// <summary>
/// Provides a read-only decoder for an ICMP Source Quench packet.
/// </summary>
[PacketImplementation(typeof(Icmpv4SourceQuenchDefinition), IsReadOnly = true)]
public readonly ref partial struct ReadOnlyIcmpv4SourceQuenchSpan
{
    /// <summary>
    /// Convert to a readonly representation.
    /// </summary>
    public static implicit operator ReadOnlyIcmpv4SourceQuenchSpan(Icmpv4SourceQuenchSpan s)
        => new ReadOnlyIcmpv4SourceQuenchSpan(s.GetRawData());
}