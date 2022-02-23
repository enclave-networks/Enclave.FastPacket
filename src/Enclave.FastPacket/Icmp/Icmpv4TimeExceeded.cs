using System;
using Enclave.FastPacket.Generator;

namespace Enclave.FastPacket.Icmp;

/// <summary>
/// Time exceeded codes.
/// </summary>
public enum Icmpv4TimeExceededCodes : byte
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    TimeToLiveExceeded = 0,
    FragmentReassemblyTimeExceeded = 1,
#pragma warning restore CS1591
}

internal ref struct Icmpv4TimeExceededDefinition
{
    /// <summary>
    /// Indicates the type of the ICMP packet.
    /// </summary>
    public Icmpv4Types Type { get; set; }

    /// <summary>
    /// The time-exceeded code.
    /// </summary>
    public Icmpv4TimeExceededCodes Code { get; set; }

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
/// Provides a read-write decoder for an ICMP Time Exceeded packet.
/// </summary>
[PacketImplementation(typeof(Icmpv4TimeExceededDefinition))]
public readonly ref partial struct Icmpv4TimeExceededSpan
{
}

/// <summary>
/// Provides a read-only decoder for an ICMP Time Exceeded packet.
/// </summary>
[PacketImplementation(typeof(Icmpv4TimeExceededDefinition), IsReadOnly = true)]
public readonly ref partial struct ReadOnlyIcmpv4TimeExceededSpan
{
    /// <summary>
    /// Convert to a readonly representation.
    /// </summary>
    public static implicit operator ReadOnlyIcmpv4TimeExceededSpan(Icmpv4TimeExceededSpan s)
        => new ReadOnlyIcmpv4TimeExceededSpan(s.GetRawData());
}