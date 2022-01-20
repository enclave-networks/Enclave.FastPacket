using System;
using System.Net.Sockets;

namespace Enclave.FastPacket;

public readonly ref struct Ipv6PacketSpan
{
    private const int ProtocolPosition = 6;
    private const int SourcePosition = 8;
    private const int DestinationPosition = 24;
    private const int DataPosition = 40;

    private readonly Span<byte> _span;

    public Ipv6PacketSpan(Span<byte> span)
    {
        if (span.IsEmpty)
        {
            throw new FastPacketException("Cannot create Ipv6 packet", span);
        }

        _span = span;
    }

    public ProtocolType Protocol =>
        _span.Length > ProtocolPosition ? (ProtocolType)_span[ProtocolPosition]
        : throw new FastPacketException("Insufficient bytes to read protocol", _span);

    public ValueIpAddress Source => ValueIpAddress.CreateIpv6(_span.Slice(SourcePosition));

    public ValueIpAddress Destination => ValueIpAddress.CreateIpv6(_span.Slice(DestinationPosition));

    public Span<byte> Payload => _span.Slice(DataPosition);
}
