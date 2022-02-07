using System;
using Enclave.FastPacket.Generator;

namespace Enclave.FastPacket.Icmp;

public enum Icmpv4RedirectCodes : byte
{
    RedirectForNetwork = 0,
    RedirectForHost = 1,
    RedirectForTypeOfServiceAndNetwork = 2,
    RedirectForTypeOfServiceAndHost = 3,
}

internal ref struct Icmpv4RedirectDefinition
{
    public Icmpv4Types Type { get; set; }

    public Icmpv4RedirectCodes Code { get; set; }

    public ushort Checksum { get; set; }

    [PacketField(Size = ValueIpAddress.Ipv4Length)]
    public ValueIpAddress IpAddress { get; set; }

    public ReadOnlySpan<byte> IpHeaderAndDatagram { get; set; }
}

[PacketImplementation(typeof(Icmpv4RedirectDefinition))]
public readonly ref partial struct Icmpv4RedirectSpan
{
}

[PacketImplementation(typeof(Icmpv4RedirectDefinition), IsReadOnly = true)]
public readonly ref partial struct ReadOnlyIcmpv4RedirectSpan
{
}