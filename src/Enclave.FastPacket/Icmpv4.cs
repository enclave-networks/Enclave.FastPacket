using System;
using System.Diagnostics.CodeAnalysis;
using Enclave.FastPacket.Generator;
using Enclave.FastPacket.Icmp;

namespace Enclave.FastPacket;

/// <summary>
/// Defines the possible Icmpv4 types.
/// </summary>
public enum Icmpv4Types : byte
{
    /// <summary>
    /// EchoReply.
    /// </summary>
    EchoReply = 0,

    /// <summary>
    /// Destination unreachable response.
    /// </summary>
    DestinationUnreachable = 3,

    /// <summary>
    /// Source Quench.
    /// </summary>
    SourceQuench = 4,

    /// <summary>
    /// Redirect.
    /// </summary>
    RedirectMessage = 5,

    /// <summary>
    /// AlternateHostAddress.
    /// </summary>
    AlternateHostAddress = 6,

    /// <summary>
    /// EchoRequest.
    /// </summary>
    EchoRequest = 8,

    /// <summary>
    /// RouterAdvertisement.
    /// </summary>
    RouterAdvertisement = 9,

    /// <summary>
    /// RouteSolicitation.
    /// </summary>
    RouterSolicitation = 10,

    /// <summary>
    /// TimeExceeded.
    /// </summary>
    TimeExceeded = 11,

    /// <summary>
    /// BadIpHeader.
    /// </summary>
    BadIpHeader = 12,

    /// <summary>
    /// Timestamp.
    /// </summary>
    Timestamp = 13,

    /// <summary>
    /// TimestampReply.
    /// </summary>
    TimestampReply = 14,

    /// <summary>
    /// InformationRequest.
    /// </summary>
    InformationRequest = 15,

    /// <summary>
    /// InformationReply.
    /// </summary>
    InformationReply = 16,

    /// <summary>
    /// AddressMaskRequest.
    /// </summary>
    AddressMaskRequest = 17,

    /// <summary>
    /// AddressMaskReply.
    /// </summary>
    AddressMaskReply = 18,

    /// <summary>
    /// Traceroute.
    /// </summary>
    Traceroute = 30,

    /// <summary>
    /// ExtendedEchoRequest.
    /// </summary>
    ExtendedEchoRequest = 42,

    /// <summary>
    /// ExtendedEchoReply.
    /// </summary>
    ExtendedEchoReply = 43,
}

internal ref struct Icmpv4Definition
{
    /// <summary>
    /// Indicates the type of the ICMP packet.
    /// </summary>
    public Icmpv4Types Type { get; set; }

    /// <summary>
    /// Defines the ICMP Code value.
    /// </summary>
    public byte Code { get; set; }

    /// <summary>
    /// The ICMP packet checksum.
    /// </summary>
    public ushort Checksum { get; set; }

    /// <summary>
    /// The rest of the header. Use the As* methods to get the typed packet.
    /// </summary>
    [PacketField(Size = 4)]
    public ReadOnlySpan<byte> RestOfHeader { get; set; }

    /// <summary>
    /// The 'Data' portion of the ICMP header.
    /// </summary>
    public ReadOnlySpan<byte> Data { get; set; }
}

/// <summary>
/// Provides a read-write decoder for an ICMP packet.
/// </summary>
[PacketImplementation(typeof(Icmpv4Definition))]
public readonly ref partial struct Icmpv4PacketSpan
{
}

/// <summary>
/// Provides a read-only decoder for an ICMP packet.
/// </summary>
[PacketImplementation(typeof(Icmpv4Definition), IsReadOnly = true)]
public readonly ref partial struct ReadOnlyIcmpv4PacketSpan
{
    /// <summary>
    /// Convert to a readonly representation.
    /// </summary>
    public static implicit operator ReadOnlyIcmpv4PacketSpan(Icmpv4PacketSpan s) => new ReadOnlyIcmpv4PacketSpan(s.GetRawData());
}

/// <summary>
/// Extension methods for converting.
/// </summary>
public static class Icmpv4Extensions
{
    /// <summary>
    /// Gets the typed ICMP decoder for the <see cref="Icmpv4Types.SourceQuench"/> type.
    /// </summary>
    public static Icmpv4SourceQuenchSpan AsSourceQuench(this Icmpv4PacketSpan icmp)
    {
        if (icmp.Type != Icmpv4Types.SourceQuench)
        {
            ThrowException($"Expected {Icmpv4Types.SourceQuench}, but found {icmp.Type}", icmp);
        }

        return new Icmpv4SourceQuenchSpan(icmp.GetRawData());
    }

    /// <summary>
    /// Gets the typed read-only ICMP decoder for the <see cref="Icmpv4Types.SourceQuench"/> type.
    /// </summary>
    public static ReadOnlyIcmpv4SourceQuenchSpan AsSourceQuench(this ReadOnlyIcmpv4PacketSpan icmp)
    {
        if (icmp.Type != Icmpv4Types.SourceQuench)
        {
            ThrowException($"Expected {Icmpv4Types.SourceQuench}, but found {icmp.Type}", icmp);
        }

        return new ReadOnlyIcmpv4SourceQuenchSpan(icmp.GetRawData());
    }

    /// <summary>
    /// Gets the typed ICMP decoder for the <see cref="Icmpv4Types.RedirectMessage"/> type.
    /// </summary>
    public static Icmpv4RedirectSpan AsRedirect(this Icmpv4PacketSpan icmp)
    {
        if (icmp.Type != Icmpv4Types.RedirectMessage)
        {
            ThrowException($"Expected {Icmpv4Types.RedirectMessage}, but found {icmp.Type}", icmp);
        }

        return new Icmpv4RedirectSpan(icmp.GetRawData());
    }

    /// <summary>
    /// Gets the typed read-only ICMP decoder for the <see cref="Icmpv4Types.RedirectMessage"/> type.
    /// </summary>
    public static ReadOnlyIcmpv4RedirectSpan AsRedirect(this ReadOnlyIcmpv4PacketSpan icmp)
    {
        if (icmp.Type != Icmpv4Types.RedirectMessage)
        {
            ThrowException($"Expected {Icmpv4Types.RedirectMessage}, but found {icmp.Type}", icmp);
        }

        return new ReadOnlyIcmpv4RedirectSpan(icmp.GetRawData());
    }

    /// <summary>
    /// Gets the typed ICMP decoder for the <see cref="Icmpv4Types.TimeExceeded"/> type.
    /// </summary>
    public static Icmpv4TimeExceededSpan AsTimeExceeded(this Icmpv4PacketSpan icmp)
    {
        if (icmp.Type != Icmpv4Types.TimeExceeded)
        {
            ThrowException($"Expected {Icmpv4Types.TimeExceeded}, but found {icmp.Type}", icmp);
        }

        return new Icmpv4TimeExceededSpan(icmp.GetRawData());
    }

    /// <summary>
    /// Gets the typed read-only ICMP decoder for the <see cref="Icmpv4Types.TimeExceeded"/> type.
    /// </summary>
    public static ReadOnlyIcmpv4TimeExceededSpan AsTimeExceeded(this ReadOnlyIcmpv4PacketSpan icmp)
    {
        if (icmp.Type != Icmpv4Types.TimeExceeded)
        {
            ThrowException($"Expected {Icmpv4Types.TimeExceeded}, but found {icmp.Type}", icmp);
        }

        return new ReadOnlyIcmpv4TimeExceededSpan(icmp.GetRawData());
    }

    /// <summary>
    /// Gets the typed ICMP decoder for the <see cref="Icmpv4Types.Timestamp"/> or <see cref="Icmpv4Types.TimestampReply"/> type.
    /// </summary>
    public static Icmpv4TimestampSpan AsTimestamp(this Icmpv4PacketSpan icmp)
    {
        if (icmp.Type != Icmpv4Types.Timestamp && icmp.Type != Icmpv4Types.TimestampReply)
        {
            ThrowException($"Expected {Icmpv4Types.Timestamp} or {Icmpv4Types.TimestampReply}, but found {icmp.Type}", icmp);
        }

        return new Icmpv4TimestampSpan(icmp.GetRawData());
    }

    /// <summary>
    /// Gets the typed read-only ICMP decoder for the <see cref="Icmpv4Types.Timestamp"/> or <see cref="Icmpv4Types.TimestampReply"/> type.
    /// </summary>
    public static ReadOnlyIcmpv4TimestampSpan AsTimestamp(this ReadOnlyIcmpv4PacketSpan icmp)
    {
        if (icmp.Type != Icmpv4Types.Timestamp && icmp.Type != Icmpv4Types.TimestampReply)
        {
            ThrowException($"Expected {Icmpv4Types.Timestamp} or {Icmpv4Types.TimestampReply}, but found {icmp.Type}", icmp);
        }

        return new ReadOnlyIcmpv4TimestampSpan(icmp.GetRawData());
    }

    /// <summary>
    /// Gets the typed ICMP decoder for the <see cref="Icmpv4Types.DestinationUnreachable"/> type.
    /// </summary>
    public static Icmpv4DestinationUnreachableSpan AsDestinationUnreachable(this Icmpv4PacketSpan icmp)
    {
        if (icmp.Type != Icmpv4Types.DestinationUnreachable)
        {
            ThrowException($"Expected {Icmpv4Types.DestinationUnreachable}, but found {icmp.Type}", icmp);
        }

        return new Icmpv4DestinationUnreachableSpan(icmp.GetRawData());
    }

    /// <summary>
    /// Gets the typed read-only ICMP decoder for the <see cref="Icmpv4Types.DestinationUnreachable"/> type.
    /// </summary>
    public static ReadOnlyIcmpv4DestinationUnreachableSpan AsDestinationUnreachable(this ReadOnlyIcmpv4PacketSpan icmp)
    {
        if (icmp.Type != Icmpv4Types.DestinationUnreachable)
        {
            ThrowException($"Expected {Icmpv4Types.DestinationUnreachable}, but found {icmp.Type}", icmp);
        }

        return new ReadOnlyIcmpv4DestinationUnreachableSpan(icmp.GetRawData());
    }

    [DoesNotReturn]
    private static void ThrowException(string message, ReadOnlyIcmpv4PacketSpan packet)
    {
        throw new FastPacketException(message, packet.GetRawData());
    }
}