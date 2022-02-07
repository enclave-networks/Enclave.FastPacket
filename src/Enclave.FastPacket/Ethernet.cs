using System;
using System.Buffers.Binary;
using Enclave.FastPacket.Generator;

namespace Enclave.FastPacket;

internal ref struct EthernetPacketDefinition
{
    /// <summary>
    /// The destination hardware (MAC) address.
    /// </summary>
    public HardwareAddress Destination { get; set; }

    /// <summary>
    /// The source hardware (MAC) address.
    /// </summary>
    public HardwareAddress Source { get; set; }

    /// <summary>
    /// The EtherType field.
    /// </summary>
    public EthernetType Type { get; set; }

    /// <summary>
    /// The Ethernet Payload.
    /// </summary>
    public ReadOnlySpan<byte> Payload { get; set; }
}

[PacketImplementation(typeof(EthernetPacketDefinition))]
public readonly ref partial struct EthernetPacketSpan
{
}

[PacketImplementation(typeof(EthernetPacketDefinition), IsReadOnly = true)]
public readonly ref partial struct ReadOnlyEthernetPacketSpan
{

    public static implicit operator ReadOnlyEthernetPacketSpan(EthernetPacketSpan s) => new ReadOnlyEthernetPacketSpan(s.GetRawData());
}

public readonly struct EthernetPacket
{
    private readonly Memory<byte> _buffer;

    public EthernetPacket(Memory<byte> buffer)
    {
        _buffer = buffer;
    }

    public EthernetPacketSpan Span => new EthernetPacketSpan(_buffer.Span);
}

public readonly struct ReadOnlyEthernetPacket
{
    private readonly ReadOnlyMemory<byte> _buffer;

    public ReadOnlyEthernetPacket(ReadOnlyMemory<byte> buffer)
    {
        _buffer = buffer;
    }

    public ReadOnlyEthernetPacketSpan Span => new ReadOnlyEthernetPacketSpan(_buffer.Span);
}