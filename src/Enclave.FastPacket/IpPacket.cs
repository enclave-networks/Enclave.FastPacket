using System;

namespace Enclave.FastPacket;

public readonly struct IpPacket
{
    private readonly Memory<byte> _buffer;

    public IpPacket(Memory<byte> buffer)
    {
        _buffer = buffer;

        if (buffer.Length < Ipv4PacketSpan.MinimumSize)
        {
            throw new FastPacketException("Buffer is too small to hold a valid IP packet", buffer.Span);
        }

        IsIpv6 = new Ipv4PacketSpan(_buffer.Span).Version == 6;
    }

    public bool IsIpv6 { get; }

    public bool IsIpv4 => !IsIpv6;

    public Ipv4PacketSpan Ipv4Span => IsIpv4 ?
        new Ipv4PacketSpan(_buffer.Span) :
        throw new FastPacketException("IP packet is not IPv4", _buffer.Span);

    public Ipv6PacketSpan Ipv6Span => IsIpv6 ?
        new Ipv6PacketSpan(_buffer.Span) :
        throw new FastPacketException("IP packet is not IPv6", _buffer.Span);
}

public readonly struct ReadOnlyIpPacket
{
    private readonly ReadOnlyMemory<byte> _buffer;

    public ReadOnlyIpPacket(ReadOnlyMemory<byte> buffer)
    {
        _buffer = buffer;

        if (buffer.Length < Ipv4PacketSpan.MinimumSize)
        {
            throw new FastPacketException("Buffer is too small to hold a valid IP packet", buffer.Span);
        }

        IsIpv6 = new ReadOnlyIpv4PacketSpan(_buffer.Span).Version == 6;
    }

    public bool IsIpv6 { get; }

    public bool IsIpv4 => !IsIpv6;

    public ReadOnlyIpv4PacketSpan Ipv4Span => IsIpv4 ?
        new ReadOnlyIpv4PacketSpan(_buffer.Span) :
        throw new FastPacketException("IP packet is not IPv4", _buffer.Span);

    public ReadOnlyIpv6PacketSpan Ipv6Span => IsIpv6 ?
        new ReadOnlyIpv6PacketSpan(_buffer.Span) :
        throw new FastPacketException("IP packet is not IPv6", _buffer.Span);
}