using System;
using System.Buffers.Binary;
using Enclave.FastPacket.Generator;

namespace Enclave.FastPacket;

internal ref struct UdpPacketDefinition
{
    public ushort SourcePort { get; set; }

    public ushort DestinationPort { get; set; }

    public ushort Length { get; set; }

    public ushort Checksum { get; set; }

    public ReadOnlySpan<byte> Payload { get; set; }
}

[PacketImplementation(typeof(UdpPacketDefinition))]
public readonly ref partial struct UdpPacketSpan
{
}

[PacketImplementation(typeof(UdpPacketDefinition), IsReadOnly = true)]
public readonly ref partial struct UdpPacketReadOnlySpan
{
}