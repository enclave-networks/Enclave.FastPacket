using System;
using Enclave.FastPacket.Generator;

namespace Enclave.FastPacket.Icmp;

public enum DestinationUnreachableCode : byte
{
    NetworkUnreachable = 0,
    HostUnreachable = 1,
    ProtocolUnreachable = 2,
    PortUnreachable = 3,
    DatagramTooBig = 4,
    SourceRouteFailed = 5,
    DestinationNetworkUnknown = 6,
    DestinationHostUnknown = 7,
    SourceHostIsolated = 8,
    DestinationNetworkAdminProhibited = 9,
    DestinationHostAdminProhibited = 10,
    NetworkUnreachableTypeOfService = 11,
    HostUnreachableTypeOfService = 12,
    CommunicationAdminProhibited = 13,
    HostPrecedenceViolation = 14,
    PrecedenceCutoff = 15,
}

internal ref struct Icmpv4DestinationUnreachableDefinition
{
    public Icmpv4Types Type { get; set; }

    public DestinationUnreachableCode Code { get; set; }

    public ushort Checksum { get; set; }

    public ushort Unused { get; set; }

    public ushort NextHopMtu { get; set; }

    public ReadOnlySpan<byte> IpHeaderAndDatagram { get; set; }
}

[PacketImplementation(typeof(Icmpv4DestinationUnreachableDefinition))]
public readonly ref partial struct Icmpv4DestinationUnreachableSpan
{
}

[PacketImplementation(typeof(Icmpv4DestinationUnreachableDefinition), IsReadOnly = true)]
public readonly ref partial struct Icmpv4DestinationUnreachableReadOnlySpan
{
}