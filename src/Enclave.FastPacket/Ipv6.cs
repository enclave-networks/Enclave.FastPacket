using System;
using System.Net.Sockets;
using Enclave.FastPacket.Generator;

namespace Enclave.FastPacket;

internal ref struct Ipv6Definition
{
    [PacketField(Size = sizeof(uint))]
    struct VersionClassAndFlow
    {
        [PacketFieldBits(0, 3)]
        public byte Version { get; set; }

        [PacketFieldBits(4, 11)]
        public byte TrafficClass { get; set; }

        [PacketFieldBits(12, 31)]
        public uint FlowLabel { get; set; }
    }

    public ushort PayloadLength { get; set; }

    [PacketField(EnumBackingType = typeof(byte))]
    public ProtocolType NextHeader { get; set; }

    public byte HopLimit { get; set; }

    [PacketField(Size = ValueIpAddress.Ipv6Length)]
    public ValueIpAddress Source { get; set; }

    [PacketField(Size = ValueIpAddress.Ipv6Length)]
    public ValueIpAddress Destination { get; set; }

    public ReadOnlySpan<byte> Payload { get; set; }
}

[PacketImplementation(typeof(Ipv6Definition))]
public readonly ref partial struct Ipv6PacketSpan
{
}

[PacketImplementation(typeof(Ipv6Definition), IsReadOnly = true)]
public readonly ref partial struct Ipv6PacketReadOnlySpan
{
}