using System;
using Enclave.FastPacket.Generator;
using Enclave.FastPacket.Ipv6Extensions;

namespace Enclave.FastPacket;

internal ref struct Ipv6Definition
{
    [PacketField(Size = sizeof(uint))]
    private struct VersionClassAndFlow
    {
        /// <summary>
        /// The IP version (6).
        /// </summary>
        [PacketFieldBits(0, 3)]
        public byte Version { get; set; }

        /// <summary>
        /// The traffic class.
        /// </summary>
        [PacketFieldBits(4, 11)]
        public byte TrafficClass { get; set; }

        /// <summary>
        /// The flow label, identifying a flow of packets between source and destination.
        /// </summary>
        [PacketFieldBits(12, 31)]
        public uint FlowLabel { get; set; }
    }

    /// <summary>
    /// The size of the payload in bytes.
    /// </summary>
    public ushort PayloadLength { get; set; }

    /// <summary>
    /// The 'next header' value.
    /// </summary>
    [PacketField(EnumBackingType = typeof(byte))]
    public IpProtocol NextHeader { get; set; }

    /// <summary>
    /// The hop limit, decremented by every forwarding node.
    /// </summary>
    public byte HopLimit { get; set; }

    /// <summary>
    /// The source IPv6 address.
    /// </summary>
    [PacketField(Size = ValueIpAddress.Ipv6Length)]
    public ValueIpAddress Source { get; set; }

    /// <summary>
    /// The destination IPv6 address.
    /// </summary>
    [PacketField(Size = ValueIpAddress.Ipv6Length)]
    public ValueIpAddress Destination { get; set; }

    /// <summary>
    /// The payload.
    /// </summary>
    public ReadOnlySpan<byte> Payload { get; set; }
}

/// <summary>
/// A readonly decoder for an IPv6 packet.
/// </summary>
[PacketImplementation(typeof(Ipv6Definition))]
public readonly ref partial struct Ipv6PacketSpan
{
    /// <summary>
    /// Access a visitor for the IPv6 Extensions.
    /// </summary>
    public Ipv6ExtensionVisitor Extensions => new Ipv6ExtensionVisitor(this);
}

/// <summary>
/// A read-write decoder for an IPv6 packet.
/// </summary>
[PacketImplementation(typeof(Ipv6Definition), IsReadOnly = true)]
public readonly ref partial struct ReadOnlyIpv6PacketSpan
{
    /// <summary>
    /// Access a visitor for the IPv6 Extensions.
    /// </summary>
    public Ipv6ExtensionVisitor Extensions => new Ipv6ExtensionVisitor(this);

    /// <summary>
    /// Convert to a readonly representation.
    /// </summary>
    public static implicit operator ReadOnlyIpv6PacketSpan(Ipv6PacketSpan s) => new ReadOnlyIpv6PacketSpan(s.GetRawData());
}