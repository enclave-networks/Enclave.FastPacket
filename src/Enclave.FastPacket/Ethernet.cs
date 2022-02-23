using System;
using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
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

/// <summary>
/// A read-write decoder for an Ethernet packet.
/// </summary>
[PacketImplementation(typeof(EthernetPacketDefinition))]
public readonly ref partial struct EthernetPacketSpan
{
}

/// <summary>
/// A read-only decoder for an Ethernet packet.
/// </summary>
[PacketImplementation(typeof(EthernetPacketDefinition), IsReadOnly = true)]
public readonly ref partial struct ReadOnlyEthernetPacketSpan
{
    /// <summary>
    /// Convert to a readonly representation.
    /// </summary>
    public static implicit operator ReadOnlyEthernetPacketSpan(EthernetPacketSpan s) => new ReadOnlyEthernetPacketSpan(s.GetRawData());
}

/// <summary>
/// A non-ref holding struct for an Ethernet packet. Fields are accessed by accessing the <see cref="Span"/> property.
/// </summary>
[SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "Holding types should not be compared")]
public readonly struct EthernetPacket
{
    private readonly Memory<byte> _buffer;

    /// <summary>
    /// Initializes a new instance of the <see cref="EthernetPacket"/> struct.
    /// </summary>
    public EthernetPacket(Memory<byte> buffer)
    {
        _buffer = buffer;
    }

    /// <summary>
    /// Get the ref struct span that allows direct access to the fields in the packet.
    /// </summary>
    public EthernetPacketSpan Span => new EthernetPacketSpan(_buffer.Span);
}

/// <summary>
/// A non-ref holding struct for a read-only Ethernet packet. Fields are accessed by accessing the <see cref="Span"/> property.
/// </summary>
[SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "Holding types should not be compared")]
public readonly struct ReadOnlyEthernetPacket
{
    private readonly ReadOnlyMemory<byte> _buffer;

    /// <summary>
    /// Initializes a new instance of the <see cref="EthernetPacket"/> struct.
    /// </summary>
    public ReadOnlyEthernetPacket(ReadOnlyMemory<byte> buffer)
    {
        _buffer = buffer;
    }

    /// <summary>
    /// Get the ref struct span that allows direct access to the fields in the packet.
    /// </summary>
    public ReadOnlyEthernetPacketSpan Span => new ReadOnlyEthernetPacketSpan(_buffer.Span);
}