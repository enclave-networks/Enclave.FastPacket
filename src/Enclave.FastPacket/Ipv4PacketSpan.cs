using System;
using System.Net.Sockets;
using Enclave.FastPacket.Generator;

namespace Enclave.FastPacket;

public ref struct Ipv4Definition
{
    public byte VersionIhl { get; set; }

    public byte Dscp { get; set; }

    public ushort TotalLength { get; set; }

    public ushort Identification { get; set; }

    public ushort FragmentValue { get; set; }

    public byte Ttl { get; set; }

    [PacketField(EnumBackingType = typeof(byte))]
    public ProtocolType Protocol { get; set; }

    public ushort HeaderChecksum { get; set; }

    [PacketField(Size = 4)]
    public ValueIpAddress Source { get; set; }

    [PacketField(Size = 4)]
    public ValueIpAddress Destination { get; set; }

    public ReadOnlySpan<byte> Payload { get; set; }
}

[PacketImplementation(typeof(Ipv4Definition))]
public readonly ref partial struct Ipv4PacketSpan
{
    //private const int ProtocolPosition = 9;
    //private const int SourcePosition = 12;
    //private const int DestinationPosition = 16;
    //private const byte IpVersion = 4;
    //private const ushort LengthFieldPosition = 2;
    //private const int TimeToLivePosition = 8;

    //private const byte IpHeaderLengthMultiplier = 4;
    //private const byte MinHeaderSize = 5;

    //private readonly Span<byte> _span;

    //public Ipv4PacketSpan(Span<byte> span)
    //{
    //    if (span.IsEmpty)
    //    {
    //        throw new FastPacketException("Cannot create Ipv4 packet", span);
    //    }

    //    _span = span;
    //}

    //public static int GetLengthIncludingPayload(int payloadSize)
    //{
    //    return (MinHeaderSize * IpHeaderLengthMultiplier) + payloadSize;
    //}

    //public static Ipv4PacketSpan CreateBlank(Span<byte> target, ushort payloadSize)
    //{
    //    ushort headerSize = MinHeaderSize * IpHeaderLengthMultiplier;

    //    byte firstByte = IpVersion << 4 | MinHeaderSize;

    //    target[0] = firstByte;

    //    BinaryPrimitives.WriteUInt16BigEndian(target.Slice(LengthFieldPosition), (ushort)(headerSize + payloadSize));

    //    target[TimeToLivePosition] = 1;

    //    return new Ipv4PacketSpan(target);
    //}

    //public ProtocolType Protocol
    //{
    //    get => _span.Length > ProtocolPosition ? (ProtocolType)_span[ProtocolPosition]
    //           : throw new FastPacketException("Insufficient bytes to read protocol", _span);
    //    set => _span[ProtocolPosition] = (byte)value;
    //}

    //public int TotalLength => _span.Length;

    //public ValueIpAddress Source
    //{
    //    get => ValueIpAddress.CreateIpv4(_span.Slice(SourcePosition));
    //    set => value.CopyTo(_span.Slice(SourcePosition));
    //}

    //public ValueIpAddress Destination
    //{
    //    get => ValueIpAddress.CreateIpv4(_span.Slice(DestinationPosition));
    //    set => value.CopyTo(_span.Slice(DestinationPosition));
    //}

    //public Span<byte> Payload => GetDataSpanWithHeaderSize(_span);

    //public void CopyTo(Span<byte> destination)
    //{
    //    _span.CopyTo(destination);
    //}

    //private static Span<byte> GetDataSpanWithHeaderSize(Span<byte> span)
    //{
    //    // Read out the IHL bits.
    //    var firstByte = span[0];

    //    // Shift to get the IHL.
    //    var headerSize = firstByte >> 4;

    //    if (headerSize < MinHeaderSize)
    //    {
    //        headerSize = MinHeaderSize;
    //    }

    //    // Data position is the header size.
    //    var dataPos = headerSize * IpHeaderLengthMultiplier;

    //    return span.Slice(dataPos);
    //}
}
