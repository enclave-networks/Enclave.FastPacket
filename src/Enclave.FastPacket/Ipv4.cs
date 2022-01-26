using System;
using System.Net.Sockets;
using Enclave.FastPacket.Generator;

namespace Enclave.FastPacket;

[Flags]
public enum FragmentFlags
{
    Reserved = 0x00,
    DontFragment = 0x01,
    MoreFragments = 0x02,
}

internal ref struct Ipv4Definition
{
    [PacketField(Size = sizeof(byte))]
    private struct U1
    {
        [PacketFieldBits(0, 3)]
        public byte Version { get; set; }

        [PacketFieldBits(4, 7)]
        public byte IHL { get; set; }
    }

    public byte Dscp { get; set; }

    public ushort TotalLength { get; set; }

    public ushort Identification { get; set; }

    [PacketField(Size = sizeof(ushort))]
    private struct U3
    {
        [PacketFieldBits(0, 2)]
        public FragmentFlags FragmentFlags { get; set; }

        [PacketFieldBits(3, 15)]
        public ushort FragmentValue { get; set; }
    }

    public byte Ttl { get; set; }

    [PacketField(EnumBackingType = typeof(byte))]
    public ProtocolType Protocol { get; set; }

    public ushort HeaderChecksum { get; set; }

    [PacketField(Size = ValueIpAddress.Ipv4Length)]
    public ValueIpAddress Source { get; set; }

    [PacketField(Size = ValueIpAddress.Ipv4Length)]
    public ValueIpAddress Destination { get; set; }

    [PacketField(SizeFunction = nameof(GetOptionsSize))]
    public ReadOnlySpan<byte> Options { get; set; }

    public ReadOnlySpan<byte> Payload { get; set; }

    public static int GetOptionsSize(ReadOnlySpan<byte> span)
    {
        // IHL field is the header size in 32-bit words. Options is 0-length if
        // IHL is 5.
        return (new Ipv4PacketReadOnlySpan(span).IHL - 5) * 4;
    }
}

[PacketImplementation(typeof(Ipv4Definition), IsReadOnly = true)]
public readonly ref partial struct Ipv4PacketReadOnlySpan
{
}

[PacketImplementation(typeof(Ipv4Definition))]
public readonly ref partial struct Ipv4PacketSpan
{
}
