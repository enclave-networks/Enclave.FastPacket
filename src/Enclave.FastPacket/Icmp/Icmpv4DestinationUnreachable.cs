using System;
using Enclave.FastPacket.Generator;

namespace Enclave.FastPacket.Icmp;

/// <summary>
/// Defines the possible destination unreachable codes.
/// </summary>
public enum DestinationUnreachableCode : byte
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    NetworkUnreachable = 0,
    HostUnreachable = 1,
    ProtocolUnreachable = 2,
    PortUnreachable = 3,
    DatagramTooBig = 4,
    SourceRouteFailed = 5,
    DestinationNetworkUnknown = 6,
    DestinationHostUnknown = 7,
    SourceHostIsolated = 8,
    DestinationNetworkAdminProhibited = 9,
    DestinationHostAdminProhibited = 10,
    NetworkUnreachableTypeOfService = 11,
    HostUnreachableTypeOfService = 12,
    CommunicationAdminProhibited = 13,
    HostPrecedenceViolation = 14,
    PrecedenceCutoff = 15,
#pragma warning restore CS1591
}

internal ref struct Icmpv4DestinationUnreachableDefinition
{
    /// <summary>
    /// Indicates the type of the ICMP packet.
    /// </summary>
    public Icmpv4Types Type { get; set; }

    /// <summary>
    /// The destination-unreachable code.
    /// </summary>
    public DestinationUnreachableCode Code { get; set; }

    /// <summary>
    /// The checksum.
    /// </summary>
    public ushort Checksum { get; set; }

    private ushort Unused { get; set; }

    /// <summary>
    /// The MTU of the next hop.
    /// </summary>
    public ushort NextHopMtu { get; set; }

    /// <summary>
    /// The failed IP header and datagram.
    /// </summary>
    public ReadOnlySpan<byte> IpHeaderAndDatagram { get; set; }
}

/// <summary>
/// Provides a read-write decoder for an ICMP Destination Unreachable packet.
/// </summary>
[PacketImplementation(typeof(Icmpv4DestinationUnreachableDefinition))]
public readonly ref partial struct Icmpv4DestinationUnreachableSpan
{
}

/// <summary>
/// Provides a read-only decoder for an ICMP Destination Unreachable packet.
/// </summary>
[PacketImplementation(typeof(Icmpv4DestinationUnreachableDefinition), IsReadOnly = true)]
public readonly ref partial struct ReadOnlyIcmpv4DestinationUnreachableSpan
{
    /// <summary>
    /// Convert to a readonly representation.
    /// </summary>
    public static implicit operator ReadOnlyIcmpv4DestinationUnreachableSpan(Icmpv4DestinationUnreachableSpan s)
        => new ReadOnlyIcmpv4DestinationUnreachableSpan(s.GetRawData());
}