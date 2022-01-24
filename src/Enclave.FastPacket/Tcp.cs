using System;
using Enclave.FastPacket.Generator;

namespace Enclave.FastPacket;

[Flags]
public enum TcpFlags
{
    Fin = 1,
    Syn = 1 << 1,
    Rst = 1 << 2,
    Psh = 1 << 3,
    Ack = 1 << 4,
    Urg = 1 << 5,
    Ece = 1 << 6,
    Cwr = 1 << 7,
    Ns = 1 << 8,
}

internal ref struct TcpPacketDefinition
{
    public ushort SourcePort { get; set; }

    public ushort DestinationPort { get; set; }

    public uint SequenceNumber { get; set; }

    public uint AckNumber { get; set; }

    [PacketField(Size = sizeof(ushort))]
    public struct UFlags
    {
        [PacketFieldBits(0, 3)]
        public byte DataOffset { get; set; }

        [PacketFieldBits(4, 15)]
        public TcpFlags Flags { get; set; }
    }

    public ushort WindowSize { get; set; }

    public ushort Checksum { get; set; }

    public ushort UrgentPointer { get; set; }

    [PacketField(SizeFunction = nameof(GetOptionSize))]
    public ReadOnlySpan<byte> Options { get; set; }

    public ReadOnlySpan<byte> Payload { get; set; }

    public static int GetOptionSize(ReadOnlySpan<byte> packet)
    {
        return (new TcpPacketReadOnlySpan(packet).DataOffset - 5) * 32;
    }
}

[PacketImplementation(typeof(TcpPacketDefinition))]
public readonly ref partial struct TcpPacketSpan
{
}

[PacketImplementation(typeof(TcpPacketDefinition), IsReadOnly = true)]
public readonly ref partial struct TcpPacketReadOnlySpan
{
}
