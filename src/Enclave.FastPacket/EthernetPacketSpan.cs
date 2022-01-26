using System;
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
public readonly ref partial struct EthernetPacketReadOnlySpan
{
}