using System;
using Enclave.FastPacket.Generator;

namespace Enclave.FastPacket.Icmp;

internal ref struct Icmpv4SourceQuenchDefinition
{
    public Icmpv4Types Type { get; set; }

    public byte Code { get; set; }

    public ushort Checksum { get; set; }

    public uint Unused { get; set; }

    public ReadOnlySpan<byte> IpHeaderAndDatagram { get; set; }
}

[PacketImplementation(typeof(Icmpv4SourceQuenchDefinition))]
public readonly ref partial struct Icmpv4SourceQuenchSpan
{
}

[PacketImplementation(typeof(Icmpv4SourceQuenchDefinition), IsReadOnly = true)]
public readonly ref partial struct Icmpv4SourceQuenchReadOnlySpan
{
}