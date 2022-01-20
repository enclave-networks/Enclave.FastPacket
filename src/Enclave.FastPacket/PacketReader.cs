using System;
using System.Buffers.Binary;

namespace Enclave.FastPacket;

public static class PacketReader
{
    public static HardwareAddress ReadHardwareAddress(ReadOnlySpan<byte> buffer, int start)
        => new HardwareAddress(buffer.Slice(start));

    public static ushort ReadUInt16(ReadOnlySpan<byte> buffer, int start)
        => BinaryPrimitives.ReadUInt16BigEndian(buffer.Slice(start, sizeof(ushort)));
}
