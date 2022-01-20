using System;
using System.Buffers.Binary;

namespace Enclave.FastPacket;

public readonly struct ArpPacket
{
    //private readonly Memory<byte> _memory;

    //public ArpPacket(Memory<byte> memory)
    //{
    //    _memory = memory;
    //}

    //public ArpPacketSpan Span => new ArpPacketSpan(_memory.Span);
}

public readonly struct ReadOnlyArpPacket
{
    //private readonly ReadOnlyMemory<byte> _memory;

    //public ReadOnlyArpPacket(ReadOnlyMemory<byte> memory)
    //{
    //    _memory = memory;
    //}

    //public ReadOnlyArpPacketSpan Span => new ReadOnlyArpPacketSpan(_memory.Span);
}

public readonly ref struct ArpPacketSpan
{
    //private const int HardwareTypePosition = 0;
    //private const int ProtocolTypePosition = HardwareTypePosition + sizeof(ushort);
    //private const int HardwareAddressLengthPosition = ProtocolTypePosition + sizeof(ushort);
    //private const int ProtocolAddressLengthPosition = HardwareAddressLengthPosition + sizeof(byte);
    //private const int OperationPosition = ProtocolAddressLengthPosition + sizeof(byte);
    //private const int SenderHardwareAddressPosition = OperationPosition + sizeof(byte);
    //private const int SenderProtocolAddressPosition = SenderHardwareAddressPosition + HardwareAddress.Length;
    //private const int TargetHardwareAddressPosition = SenderProtocolAddressPosition + ValueIpAddress.Ipv4Length;
    //private const int TargetProtocolAddressPosition = TargetHardwareAddressPosition + HardwareAddress.Length;

    //private readonly Span<byte> _span;

    //public ArpPacketSpan(Span<byte> span)
    //{
    //    _span = span;
    //}

    //public ushort HardwareType
    //{
    //    get => PacketReader.ReadUInt16(_span, HardwareTypePosition);
    //    set => PacketWriter.WriteUInt16(_span, HardwareTypePosition, value);
    //}

    //public EthernetType ProtocolType => (EthernetType)PacketReader.ReadUInt16(_span, ProtocolTypePosition);

    //public byte HardwareAddressLength => _span[HardwareAddressLengthPosition];

    //public byte ProtocolAddressLength => _span[ProtocolAddressLengthPosition];

    //public ArpOperation Operation => (ArpOperation)PacketReader.ReadUInt16(_span, OperationPosition);

    //public void OverwriteOperation(ArpOperation operation)
    //{
    //    BinaryPrimitives.WriteUInt16BigEndian(_span.Slice(OperationPosition), (ushort)operation);
    //}

    //public HardwareAddress SenderHardwareAddress => new HardwareAddress(_span.Slice(SenderHwAddressPosition));

    //public void OverwriteSenderHardwareAddress(in HardwareAddress newSender) => newSender.CopyTo(_span.Slice(SenderHwAddressPosition));

    //public ValueIpAddress SenderProtocolAddress => ValueIpAddress.CreateIpv4(_span.Slice(SenderProtocolAddressPosition));

    //public void OverwriteSenderProtocolAddress(in ValueIpAddress newAddress) => newAddress.CopyTo(_span.Slice(SenderProtocolAddressPosition));

    //public HardwareAddress TargetHardwareAddress => new HardwareAddress(_span.Slice(TargetHwAddressPosition));

    //public void OverwriteTargetHardwareAddress(in HardwareAddress newSender) => newSender.CopyTo(_span.Slice(TargetHwAddressPosition));

    //public ValueIpAddress TargetProtocolAddress => ValueIpAddress.CreateIpv4(_span.Slice(TargetProtocolAddressPosition));

    //public void OverwriteTargetProtocolAddress(in ValueIpAddress newAddress) => newAddress.CopyTo(_span.Slice(TargetProtocolAddressPosition));

    //public override string ToString()
    //{
    //    return $"{Operation} > {TargetHardwareAddress} : {TargetProtocolAddress}";
    //}
}
