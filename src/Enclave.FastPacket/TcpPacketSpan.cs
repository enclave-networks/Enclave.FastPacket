using System;
using System.Buffers.Binary;

namespace Enclave.FastPacket;

public readonly ref struct TcpPacketSpan
{
    private const int DataOffsetPosition = 12;
    private const int DestinationPortPosition = 2;

    private const int TcpHeaderLengthMultiplier = 4;
    private const int MinHeaderSize = 5;

    private readonly Span<byte> _span;

    public TcpPacketSpan(Span<byte> span)
    {
        _span = span;
    }

    public ushort SourcePort => BinaryPrimitives.TryReadUInt16BigEndian(_span, out var readValue) ? readValue
        : throw new FastPacketException("Insufficient bytes to read source port", _span);

    public ushort DestinationPort
    {
        get
        {
            if (_span.Length < DestinationPortPosition + 2)
            {
                throw new FastPacketException("Insufficient bytes to read destination port", _span);
            }

            var slice = _span.Slice(DestinationPortPosition);

            return BinaryPrimitives.ReadUInt16BigEndian(slice);
        }
    }

    public Span<byte> Payload => GetDataSpanWithHeaderSize(_span);

    private static Span<byte> GetDataSpanWithHeaderSize(Span<byte> span)
    {
        // Read out the IHL bits.
        var offsetByte = span[DataOffsetPosition];

        // Shift to get the IHL.
        var headerSize = offsetByte >> 4;

        if (headerSize < MinHeaderSize)
        {
            headerSize = MinHeaderSize;
        }

        // Data position is the header size.
        var dataPos = headerSize * TcpHeaderLengthMultiplier;

        return span.Slice(dataPos);
    }
}
