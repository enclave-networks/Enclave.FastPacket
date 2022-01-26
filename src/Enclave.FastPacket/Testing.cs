using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Enclave.FastPacket.Generator;

namespace Enclave.FastPacket;

internal ref struct MyPacketDefinition
{
    public byte Field1 { get; set; }

    public byte Field2 { get; set; }

    public ushort Field3 { get; set; }

    public ReadOnlySpan<byte> RestOfData { get; set; }
}

[PacketImplementation(typeof(MyPacketDefinition))]
public readonly ref partial struct MyPacketImplementation
{
}