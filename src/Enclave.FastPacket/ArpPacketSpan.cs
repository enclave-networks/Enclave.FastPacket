using Enclave.FastPacket.Generator;

namespace Enclave.FastPacket;

internal struct ArpPacketDefinition
{
    public ushort HardwareType { get; set; }

    public ushort ProtocolType { get; set; }

    public byte HardwareAddressLength { get; set; }

    public byte ProtocolAddressLength { get; set; }

    public ArpOperation Operation { get; set; }

    public HardwareAddress SenderHardwareAddress { get; set; }

    [PacketField(Size = 4)]
    public ValueIpAddress SenderProtocolAddress { get; set; }

    public HardwareAddress TargetHardwareAddress { get; set; }

    [PacketField(Size = 4)]
    public ValueIpAddress TargetProtocolAddress { get; set; }
}

[PacketImplementation(typeof(ArpPacketDefinition))]
public readonly ref partial struct ArpPacketSpan
{
}

[PacketImplementation(typeof(ArpPacketDefinition), IsReadOnly = true)]
public readonly ref partial struct ArpPacketReadOnlySpan
{
}
