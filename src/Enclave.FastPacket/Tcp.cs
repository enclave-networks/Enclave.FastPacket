using System;
using Enclave.FastPacket.Generator;

namespace Enclave.FastPacket;

/// <summary>
/// Defines the possible TCP flags.
/// </summary>
[Flags]
public enum TcpFlags
{
    /// <summary>
    /// The finished (FIN) flag, which indicates whether the sender has finished sending.
    /// </summary>
    Fin = 1,

    /// <summary>
    /// The synchronise (SYN) flag, which indicates the sequence numbers should be synchronized
    /// between the sender and receiver to initiate a connection.
    /// </summary>
    Syn = 1 << 1,

    /// <summary>
    /// The reset (RST) flag, which indicates the session should be reset between
    /// the sender and the receiver.
    /// </summary>
    Rst = 1 << 2,

    /// <summary>
    /// The push (PSH) flag, which indicates the receiver should pass the data to the application as soon as possible.
    /// </summary>
    Psh = 1 << 3,

    /// <summary>
    /// The acknowledgment (ACK) flag, which indicates if the Acknowledgment Number is valid.
    /// </summary>
    Ack = 1 << 4,

    /// <summary>
    /// The urgent (URG) flag, which indicates if the 'urgent pointer' field is meaningful.
    /// </summary>
    Urg = 1 << 5,

    /// <summary>
    /// ECE Flag.
    /// </summary>
    Ece = 1 << 6,

    /// <summary>
    /// CWR Flag.
    /// </summary>
    Cwr = 1 << 7,

    /// <summary>
    /// NS Flag.
    /// </summary>
    Ns = 1 << 8,
}

internal ref struct TcpPacketDefinition
{
    /// <summary>
    /// The source port number.
    /// </summary>
    public ushort SourcePort { get; set; }

    /// <summary>
    /// The destination port number.
    /// </summary>
    public ushort DestinationPort { get; set; }

    /// <summary>
    /// The sequence number.
    /// </summary>
    public uint SequenceNumber { get; set; }

    /// <summary>
    /// The next sequence number the sender is expecting.
    /// </summary>
    public uint AckNumber { get; set; }

    [PacketField(Size = sizeof(ushort))]
    public struct UFlags
    {
        /// <summary>
        /// The size of the TCP header in 32-bit words. (min 5).
        /// </summary>
        [PacketFieldBits(0, 3)]
        public byte DataOffset { get; set; }

        /// <summary>
        /// The TCP flags.
        /// </summary>
        [PacketFieldBits(4, 15)]
        public TcpFlags Flags { get; set; }
    }

    /// <summary>
    /// The size of the receive window.
    /// </summary>
    public ushort WindowSize { get; set; }

    /// <summary>
    /// The TCP header checksum.
    /// </summary>
    public ushort Checksum { get; set; }

    /// <summary>
    /// If the urgent flag is set, this is an offset from the sequence number indicating the last urgent byte.
    /// </summary>
    public ushort UrgentPointer { get; set; }

    /// <summary>
    /// The options block.
    /// </summary>
    [PacketField(SizeFunction = nameof(GetOptionSize))]
    public ReadOnlySpan<byte> Options { get; set; }

    /// <summary>
    /// The payload.
    /// </summary>
    public ReadOnlySpan<byte> Payload { get; set; }

    public static int GetOptionSize(ReadOnlySpan<byte> packet)
    {
        return (new ReadOnlyTcpPacketSpan(packet).DataOffset - 5) * 4;
    }
}

/// <summary>
/// A readonly decoder for a TCP packet.
/// </summary>
[PacketImplementation(typeof(TcpPacketDefinition))]
public readonly ref partial struct TcpPacketSpan
{
    /// <summary>
    /// Get the required size of a buffer that would hold a TCP packet with the given payload.
    /// </summary>
    public static int GetRequiredPacketSize(int payloadSize)
    {
        return MinimumSize + payloadSize;
    }
}

/// <summary>
/// A read-write decoder for a TCP packet.
/// </summary>
[PacketImplementation(typeof(TcpPacketDefinition), IsReadOnly = true)]
public readonly ref partial struct ReadOnlyTcpPacketSpan
{
    /// <summary>
    /// Convert to a readonly representation.
    /// </summary>
    public static implicit operator ReadOnlyTcpPacketSpan(TcpPacketSpan s) => new ReadOnlyTcpPacketSpan(s.GetRawData());
}
