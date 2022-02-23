using System;
using Enclave.FastPacket.Generator;

namespace Enclave.FastPacket.Icmp;

/// <summary>
/// Redirect Codes.
/// </summary>
public enum Icmpv4RedirectCodes : byte
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    RedirectForNetwork = 0,
    RedirectForHost = 1,
    RedirectForTypeOfServiceAndNetwork = 2,
    RedirectForTypeOfServiceAndHost = 3,
#pragma warning restore CS1591
}

internal ref struct Icmpv4RedirectDefinition
{
    /// <summary>
    /// Indicates the type of the ICMP packet.
    /// </summary>
    public Icmpv4Types Type { get; set; }

    /// <summary>
    /// The redirect code.
    /// </summary>
    public Icmpv4RedirectCodes Code { get; set; }

    /// <summary>
    /// The checksum.
    /// </summary>
    public ushort Checksum { get; set; }

    /// <summary>
    /// The IPv4 address to redirect to.
    /// </summary>
    [PacketField(Size = ValueIpAddress.Ipv4Length)]
    public ValueIpAddress IpAddress { get; set; }

    /// <summary>
    /// The original IP Header and datagram.
    /// </summary>
    public ReadOnlySpan<byte> IpHeaderAndDatagram { get; set; }
}

/// <summary>
/// Provides a read-write decoder for an ICMP Redirect packet.
/// </summary>
[PacketImplementation(typeof(Icmpv4RedirectDefinition))]
public readonly ref partial struct Icmpv4RedirectSpan
{
}

/// <summary>
/// Provides a read-only decoder for an ICMP Redirect packet.
/// </summary>
[PacketImplementation(typeof(Icmpv4RedirectDefinition), IsReadOnly = true)]
public readonly ref partial struct ReadOnlyIcmpv4RedirectSpan
{
    /// <summary>
    /// Convert to a readonly representation.
    /// </summary>
    public static implicit operator ReadOnlyIcmpv4RedirectSpan(Icmpv4RedirectSpan s)
        => new ReadOnlyIcmpv4RedirectSpan(s.GetRawData());
}