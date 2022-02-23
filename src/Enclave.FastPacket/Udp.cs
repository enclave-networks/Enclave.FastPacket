using System;
using Enclave.FastPacket.Generator;

namespace Enclave.FastPacket;

internal ref struct UdpPacketDefinition
{
    /// <summary>
    /// The source port.
    /// </summary>
    public ushort SourcePort { get; set; }

    /// <summary>
    /// The destination port.
    /// </summary>
    public ushort DestinationPort { get; set; }

    /// <summary>
    /// The length in bytes of the header and data.
    /// </summary>
    public ushort Length { get; set; }

    /// <summary>
    /// The checksum.
    /// </summary>
    public ushort Checksum { get; set; }

    /// <summary>
    /// The payload.
    /// </summary>
    public ReadOnlySpan<byte> Payload { get; set; }
}

/// <summary>
/// A read-write decoder for a UDP packet.
/// </summary>
[PacketImplementation(typeof(UdpPacketDefinition))]
public readonly ref partial struct UdpPacketSpan
{
}

/// <summary>
/// A read-only decoder for a UDP packet.
/// </summary>
[PacketImplementation(typeof(UdpPacketDefinition), IsReadOnly = true)]
public readonly ref partial struct ReadOnlyUdpPacketSpan
{
    /// <summary>
    /// Convert to a readonly representation.
    /// </summary>
    public static implicit operator ReadOnlyUdpPacketSpan(UdpPacketSpan s) => new ReadOnlyUdpPacketSpan(s.GetRawData());
}