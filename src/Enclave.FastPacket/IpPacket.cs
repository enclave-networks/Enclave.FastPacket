using System;

namespace Enclave.FastPacket;

public readonly struct IpPacket : IEquatable<IpPacket>
{
    private readonly Memory<byte> _packetData;

    public IpPacket(Memory<byte> packetData)
    {
        _packetData = packetData;
    }

    public Ipv4PacketSpan Ipv4Span => new Ipv4PacketSpan(_packetData.Span);

    public Ipv6PacketSpan Ipv6Span => new Ipv6PacketSpan(_packetData.Span);

    public override bool Equals(object? obj)
    {
        return obj is IpPacket packet && Equals(packet);
    }

    public bool Equals(IpPacket other)
    {
        return _packetData.Equals(other._packetData);
    }

    public override int GetHashCode()
    {
        return _packetData.GetHashCode();
    }

    public static bool operator ==(IpPacket left, IpPacket right) => left.Equals(right);

    public static bool operator !=(IpPacket left, IpPacket right) => !(left == right);
}
