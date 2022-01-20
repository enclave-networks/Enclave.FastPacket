using System;
using System.Buffers.Binary;

namespace Enclave.FastPacket;

public static class PacketWriter
{
    public static void WriteHardwareAddress(Span<byte> buffer, int start, HardwareAddress address)
        => address.CopyTo(buffer.Slice(start));

    public static void WriteUInt16(Span<byte> buffer, int start, ushort value)
        => BinaryPrimitives.WriteUInt16BigEndian(buffer.Slice(start, sizeof(ushort)), value);
}
