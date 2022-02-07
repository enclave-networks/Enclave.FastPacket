using System;
using System.Diagnostics.CodeAnalysis;
using Enclave.FastPacket.Generator;
using Enclave.FastPacket.Icmp;

namespace Enclave.FastPacket;

public enum Icmpv4Types : byte
{
    EchoReply = 0,
    DestinationUnreachable = 3,
    SourceQuench = 4,
    RedirectMessage = 5,
    AlternateHostAddress = 6,
    EchoRequest = 8,
    RouterAdvertisement = 9,
    RouterSolicitation = 10,
    TimeExceeded = 11,
    BadIpHeader = 12,
    Timestamp = 13,
    TimestampReply = 14,
    InformationRequest = 15,
    InformationReply = 16,
    AddressMaskRequest = 17,
    AddressMaskReply = 18,
    Traceroute = 30,
    ExtendedEchoRequest = 42,
    ExtendedEchoReply = 43,
}

internal ref struct Icmpv4Definition
{
    public Icmpv4Types Type { get; set; }

    public byte Code { get; set; }

    public ushort Checksum { get; set; }

    [PacketField(Size = 4)]
    public ReadOnlySpan<byte> RestOfHeader { get; set; }

    public ReadOnlySpan<byte> Data { get; set; }
}

[PacketImplementation(typeof(Icmpv4Definition))]
public readonly ref partial struct Icmpv4PacketSpan
{
}

[PacketImplementation(typeof(Icmpv4Definition), IsReadOnly = true)]
public readonly ref partial struct ReadOnlyIcmpv4PacketSpan
{
    public static implicit operator ReadOnlyIcmpv4PacketSpan(Icmpv4PacketSpan s) => new ReadOnlyIcmpv4PacketSpan(s.GetRawData());
}

public static class Icmpv4Extensions
{
    public static Icmpv4SourceQuenchSpan AsSourceQuench(this Icmpv4PacketSpan icmp)
    {
        if (icmp.Type != Icmpv4Types.SourceQuench)
        {
            ThrowException($"Expected {Icmpv4Types.SourceQuench}, but found {icmp.Type}", icmp);
        }

        return new Icmpv4SourceQuenchSpan(icmp.GetRawData());
    }

    public static ReadOnlyIcmpv4SourceQuenchSpan AsSourceQuench(this ReadOnlyIcmpv4PacketSpan icmp)
    {
        if (icmp.Type != Icmpv4Types.SourceQuench)
        {
            ThrowException($"Expected {Icmpv4Types.SourceQuench}, but found {icmp.Type}", icmp);
        }

        return new ReadOnlyIcmpv4SourceQuenchSpan(icmp.GetRawData());
    }

    public static Icmpv4RedirectSpan AsRedirect(this Icmpv4PacketSpan icmp)
    {
        if (icmp.Type != Icmpv4Types.RedirectMessage)
        {
            ThrowException($"Expected {Icmpv4Types.RedirectMessage}, but found {icmp.Type}", icmp);
        }

        return new Icmpv4RedirectSpan(icmp.GetRawData());
    }

    public static ReadOnlyIcmpv4RedirectSpan AsRedirect(this ReadOnlyIcmpv4PacketSpan icmp)
    {
        if (icmp.Type != Icmpv4Types.RedirectMessage)
        {
            ThrowException($"Expected {Icmpv4Types.RedirectMessage}, but found {icmp.Type}", icmp);
        }

        return new ReadOnlyIcmpv4RedirectSpan(icmp.GetRawData());
    }

    public static Icmpv4TimeExceededSpan AsTimeExceeded(this Icmpv4PacketSpan icmp)
    {
        if (icmp.Type != Icmpv4Types.TimeExceeded)
        {
            ThrowException($"Expected {Icmpv4Types.TimeExceeded}, but found {icmp.Type}", icmp);
        }

        return new Icmpv4TimeExceededSpan(icmp.GetRawData());
    }

    public static ReadOnlyIcmpv4TimeExceededSpan AsTimeExceeded(this ReadOnlyIcmpv4PacketSpan icmp)
    {
        if (icmp.Type != Icmpv4Types.TimeExceeded)
        {
            ThrowException($"Expected {Icmpv4Types.TimeExceeded}, but found {icmp.Type}", icmp);
        }

        return new ReadOnlyIcmpv4TimeExceededSpan(icmp.GetRawData());
    }

    public static Icmpv4TimestampSpan AsTimestamp(this Icmpv4PacketSpan icmp)
    {
        if (icmp.Type != Icmpv4Types.Timestamp && icmp.Type != Icmpv4Types.TimestampReply)
        {
            ThrowException($"Expected {Icmpv4Types.Timestamp} or {Icmpv4Types.TimestampReply}, but found {icmp.Type}", icmp);
        }

        return new Icmpv4TimestampSpan(icmp.GetRawData());
    }

    public static ReadOnlyIcmpv4TimestampSpan AsTimestamp(this ReadOnlyIcmpv4PacketSpan icmp)
    {
        if (icmp.Type != Icmpv4Types.Timestamp && icmp.Type != Icmpv4Types.TimestampReply)
        {
            ThrowException($"Expected {Icmpv4Types.Timestamp} or {Icmpv4Types.TimestampReply}, but found {icmp.Type}", icmp);
        }

        return new ReadOnlyIcmpv4TimestampSpan(icmp.GetRawData());
    }

    public static Icmpv4TimestampSpan AsDestinationUnreachable(this Icmpv4PacketSpan icmp)
    {
        if (icmp.Type != Icmpv4Types.Timestamp && icmp.Type != Icmpv4Types.TimestampReply)
        {
            ThrowException($"Expected {Icmpv4Types.Timestamp} or {Icmpv4Types.TimestampReply}, but found {icmp.Type}", icmp);
        }

        return new Icmpv4TimestampSpan(icmp.GetRawData());
    }

    public static ReadOnlyIcmpv4TimestampSpan AsDestinationUnreachable(this ReadOnlyIcmpv4PacketSpan icmp)
    {
        if (icmp.Type != Icmpv4Types.Timestamp && icmp.Type != Icmpv4Types.TimestampReply)
        {
            ThrowException($"Expected {Icmpv4Types.Timestamp} or {Icmpv4Types.TimestampReply}, but found {icmp.Type}", icmp);
        }

        return new ReadOnlyIcmpv4TimestampSpan(icmp.GetRawData());
    }

    [DoesNotReturn]
    private static void ThrowException(string message, ReadOnlyIcmpv4PacketSpan packet)
    {
        throw new FastPacketException(message, packet.GetRawData());
    }
}