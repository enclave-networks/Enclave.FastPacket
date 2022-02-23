using System;
using System.Diagnostics.CodeAnalysis;

namespace Enclave.FastPacket;

/// <summary>
/// Holding type for an IP Packet that may be IPv4 or IPv6.
/// </summary>
[SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "Comparing two packets directly not needed")]
public readonly struct IpPacket
{
    private readonly Memory<byte> _buffer;

    /// <summary>
    /// Create a new <see cref="IpPacket"/> from a buffer containing either an IPv4 or IPv6 packet.
    /// </summary>
    public IpPacket(Memory<byte> buffer)
    {
        _buffer = buffer;

        if (buffer.Length < Ipv4PacketSpan.MinimumSize)
        {
            throw new FastPacketException("Buffer is too small to hold a valid IP packet", buffer.Span);
        }

        IsIpv6 = new Ipv4PacketSpan(_buffer.Span).Version == 6;
    }

    /// <summary>
    /// Is this packet IPv6.
    /// </summary>
    public bool IsIpv6 { get; }

    /// <summary>
    /// Is this packet IPv4.
    /// </summary>
    public bool IsIpv4 => !IsIpv6;

    /// <summary>
    /// Access the IP Packet span for IPv4.
    /// </summary>
    public Ipv4PacketSpan Ipv4Span => IsIpv4 ?
        new Ipv4PacketSpan(_buffer.Span) :
        throw new FastPacketException("IP packet is not IPv4", _buffer.Span);

    /// <summary>
    /// Access the IP Packet span for IPv6.
    /// </summary>
    public Ipv6PacketSpan Ipv6Span => IsIpv6 ?
        new Ipv6PacketSpan(_buffer.Span) :
        throw new FastPacketException("IP packet is not IPv6", _buffer.Span);
}

/// <summary>
/// Holding type for a read-only IP Packet that may be IPv4 or IPv6.
/// </summary>
[SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "Comparing two packets directly not needed")]
public readonly struct ReadOnlyIpPacket
{
    private readonly ReadOnlyMemory<byte> _buffer;

    /// <summary>
    /// Create a new <see cref="ReadOnlyIpPacket"/> from a buffer containing either an IPv4 or IPv6 packet.
    /// </summary>
    public ReadOnlyIpPacket(ReadOnlyMemory<byte> buffer)
    {
        _buffer = buffer;

        if (buffer.Length < Ipv4PacketSpan.MinimumSize)
        {
            throw new FastPacketException("Buffer is too small to hold a valid IP packet", buffer.Span);
        }

        IsIpv6 = new ReadOnlyIpv4PacketSpan(_buffer.Span).Version == 6;
    }

    /// <summary>
    /// Is this packet IPv6.
    /// </summary>
    public bool IsIpv6 { get; }

    /// <summary>
    /// Is this packet IPv4.
    /// </summary>
    public bool IsIpv4 => !IsIpv6;

    /// <summary>
    /// Access the IP Packet span for IPv4.
    /// </summary>
    public ReadOnlyIpv4PacketSpan Ipv4Span => IsIpv4 ?
        new ReadOnlyIpv4PacketSpan(_buffer.Span) :
        throw new FastPacketException("IP packet is not IPv4", _buffer.Span);

    /// <summary>
    /// Access the IP Packet span for IPv6.
    /// </summary>
    public ReadOnlyIpv6PacketSpan Ipv6Span => IsIpv6 ?
        new ReadOnlyIpv6PacketSpan(_buffer.Span) :
        throw new FastPacketException("IP packet is not IPv6", _buffer.Span);
}