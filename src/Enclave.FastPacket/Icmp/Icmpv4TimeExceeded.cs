using System;
using Enclave.FastPacket.Generator;

namespace Enclave.FastPacket.Icmp;

public enum Icmpv4TimeExceededCodes : byte
{
    TimeToLiveExceeded = 0,
    FragmentReassemblyTimeExceeded = 1,
}

internal ref struct Icmpv4TimeExceededDefinition
{
    public Icmpv4Types Type { get; set; }

    public Icmpv4TimeExceededCodes Code { get; set; }

    public ushort Checksum { get; set; }

    public uint Unused { get; set; }

    public ReadOnlySpan<byte> IpHeaderAndDatagram { get; set; }
}

[PacketImplementation(typeof(Icmpv4TimeExceededDefinition))]
public readonly ref partial struct Icmpv4TimeExceededSpan
{
}

[PacketImplementation(typeof(Icmpv4TimeExceededDefinition), IsReadOnly = true)]
public readonly ref partial struct ReadOnlyIcmpv4TimeExceededSpan
{
}