using Enclave.FastPacket.Generator;

namespace Enclave.FastPacket;

internal struct ArpPacketDefinition
{
    /// <summary>
    /// Specifes the network link protocol type.
    /// </summary>
    public ushort HardwareType { get; set; }

    /// <summary>
    /// Specifies the IP protocol for which the ARP request is intended. For IPv4 packets, this will be 0x800.
    /// </summary>
    public ushort ProtocolType { get; set; }

    /// <summary>
    /// The length of the hardware address segment (will be 6).
    /// </summary>
    public byte HardwareAddressLength { get; set; }

    /// <summary>
    /// The length of the hardware address segment (will be 4 for IPv4 requests).
    /// </summary>
    public byte ProtocolAddressLength { get; set; }

    /// <summary>
    /// Specifies the arp operation (request or reply).
    /// </summary>
    public ArpOperation Operation { get; set; }

    /// <summary>
    /// The hardware address of the sender.
    /// </summary>
    public HardwareAddress SenderHardwareAddress { get; set; }

    /// <summary>
    /// The protocol address of the sender.
    /// </summary>
    [PacketField(Size = 4)]
    public ValueIpAddress SenderProtocolAddress { get; set; }

    /// <summary>
    /// The hardware address of the target (blank for ARP requests).
    /// </summary>
    public HardwareAddress TargetHardwareAddress { get; set; }

    /// <summary>
    /// The protocol address of the target.
    /// </summary>
    [PacketField(Size = 4)]
    public ValueIpAddress TargetProtocolAddress { get; set; }
}

/// <summary>
/// A read-write decoder for an ARP packet.
/// </summary>
[PacketImplementation(typeof(ArpPacketDefinition))]
public readonly ref partial struct ArpPacketSpan
{
}

/// <summary>
/// A read-only decoder for an ARP packet.
/// </summary>
[PacketImplementation(typeof(ArpPacketDefinition), IsReadOnly = true)]
public readonly ref partial struct ReadOnlyArpPacketSpan
{
    /// <summary>
    /// Convert to a readonly representation.
    /// </summary>
    public static implicit operator ReadOnlyArpPacketSpan(ArpPacketSpan s) => new ReadOnlyArpPacketSpan(s.GetRawData());
}
