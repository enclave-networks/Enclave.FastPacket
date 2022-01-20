using System;
using System.Runtime.CompilerServices;

namespace Enclave.FastPacket;

public enum Icmpv4Types : byte
{
    EchoReply = 0,
    DestinationUnreachable = 3,
    SourceQuench = 4,
    RedirectMessage = 5,
    AlternateHostAddress = 6,
    EchoRequest = 8,
    RouterAdvertisement = 9,
    RouterSolicitation = 10,
    TimeExceeded = 11,
    BadIpHeader = 12,
    Timestamp = 13,
    TimestampReply = 14,
    InformationRequest = 15,
    InformationReply = 16,
    AddressMaskRequest = 17,
    AddressMaskReply = 18,
    Traceroute = 30,
    ExtendedEchoRequest = 42,
    ExtendedEchoReply = 43,
}

public readonly ref struct Icmpv4PacketSpan
{
    private const int TypePosition = 0;
    private const int CodePosition = 1;

    private readonly Span<byte> _span;

    public Icmpv4PacketSpan(Span<byte> span)
    {
        _span = span;
    }

    public Icmpv4Types Type => _span.Length > TypePosition ? (Icmpv4Types)_span[TypePosition] : throw new FastPacketException("Insufficient bytes to read type", _span);

    public byte Code => _span.Length > CodePosition ? _span[CodePosition] : throw new FastPacketException("Insufficient bytes to read code", _span);
}
