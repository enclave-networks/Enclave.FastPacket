using System;
using System.Net.Sockets;
using System.Text;
using Enclave.FastPacket.Generator;

namespace Enclave.FastPacket;

/// <summary>
/// Possible IPv4 fragmentation flags.
/// </summary>
[Flags]
public enum FragmentFlags
{
    /// <summary>
    /// None
    /// </summary>
    None = 0, // When bit shifted, gives [000][0 0000 0000 0000] in binary

    /// <summary>
    /// More fragments (MF).
    /// </summary>
    MoreFragments = 1, // When bit shifted, gives [001][0 0000 0000 0000] in binary

    /// <summary>
    /// Don't fragment (DF).
    /// </summary>
    DontFragment = 2, // When bit shifted, gives [010][0 0000 0000 0000] in binary

    /// <summary>
    /// Reserved, do not use.
    /// </summary>
    Reserved = 4, // When bit shifted, gives [100][0 0000 0000 0000] in binary
}

internal ref struct Ipv4Definition
{
    [PacketField(Size = sizeof(byte))]
    private struct U1
    {
        /// <summary>
        /// The Version field indicates the format of the internet header.
        /// For IPv4 this will always have the value 4.
        /// </summary>
        [PacketFieldBits(0, 3)]
        public byte Version { get; set; }

        /// <summary>
        /// Internet Header Length is the length of the internet header in 32
        /// bit words, and thus points to the beginning of the data.Note that
        /// the minimum value for a correct header is 5.
        /// </summary>
        [PacketFieldBits(4, 7)]
        public byte IHL { get; set; }
    }

    /// <summary>
    /// Differentiated services field.
    /// </summary>
    /// <remarks>See https://en.wikipedia.org/wiki/Differentiated_services.</remarks>
    public byte Dscp { get; set; }

    /// <summary>
    /// Defines the entire packet size in bytes, including header and data.
    /// </summary>
    /// <remarks>
    /// This 16-bit field defines the entire packet size in bytes, including header and data.
    /// The minimum size is 20 bytes (header without data) and the maximum is 65,535 bytes.
    /// All hosts are required to be able to reassemble datagrams of size up to 576 bytes, but
    /// most modern hosts handle much larger packets. Links may impose further restrictions on the
    /// packet size, in which case datagrams must be fragmented. Fragmentation in IPv4 is performed in
    /// either the sending host or in routers. Reassembly is performed at the receiving host.
    /// </remarks>
    public ushort TotalLength { get; set; }

    /// <summary>
    /// This field is an identification field and is primarily used for uniquely identifying the
    /// group of fragments of a single IP datagram.
    /// </summary>
    public ushort Identification { get; set; }

    [PacketField(Size = sizeof(ushort))]
    private struct U3
    {
        /// <summary>
        /// Fragmentation flags.
        /// </summary>
        [PacketFieldBits(0, 2)]
        public FragmentFlags FragmentFlags { get; set; }

        /// <summary>
        /// This field specifies the offset of a particular fragment relative to the beginning
        /// of the original unfragmented IP datagram in units of eight-byte blocks.
        /// </summary>
        [PacketFieldBits(3, 15)]
        public ushort FragmentOffset { get; set; }
    }

    /// <summary>
    /// An eight-bit time to live field limits a datagram's lifetime to prevent network failure in the event of a routing loop.
    /// The field is generally used a hop count, decremented by 1 each time a packet goes through a router.
    /// </summary>
    public byte Ttl { get; set; }

    /// <summary>
    /// The protocol code.
    /// </summary>
    [PacketField(EnumBackingType = typeof(byte))]
    public IpProtocol Protocol { get; set; }

    /// <summary>
    /// Header checksum field used for error-checking of the header.
    /// </summary>
    public ushort HeaderChecksum { get; set; }

    /// <summary>
    /// The source IPv4 address.
    /// </summary>
    [PacketField(Size = ValueIpAddress.Ipv4Length)]
    public ValueIpAddress Source { get; set; }

    /// <summary>
    /// The destination IPv4 address.
    /// </summary>
    [PacketField(Size = ValueIpAddress.Ipv4Length)]
    public ValueIpAddress Destination { get; set; }

    /// <summary>
    /// An options block (rarely used).
    /// </summary>
    [PacketField(SizeFunction = nameof(GetOptionsSize))]
    public ReadOnlySpan<byte> Options { get; set; }

    /// <summary>
    /// The IP payload.
    /// </summary>
    public ReadOnlySpan<byte> Payload { get; set; }

    public static int GetOptionsSize(ReadOnlySpan<byte> span)
    {
        // IHL field is the header size in 32-bit words. Options is 0-length if
        // IHL is 5.
        return (new ReadOnlyIpv4PacketSpan(span).IHL - 5) * 4;
    }
}

/// <summary>
/// A read-write decoder for an IPv4 packet.
/// </summary>
[PacketImplementation(typeof(Ipv4Definition))]
public readonly ref partial struct Ipv4PacketSpan
{
}

/// <summary>
/// A readonly decoder for an IPv4 packet.
/// </summary>
[PacketImplementation(typeof(Ipv4Definition), IsReadOnly = true)]
public readonly ref partial struct ReadOnlyIpv4PacketSpan
{
    /// <summary>
    /// Convert to a readonly representation.
    /// </summary>
    public static implicit operator ReadOnlyIpv4PacketSpan(Ipv4PacketSpan s) => new ReadOnlyIpv4PacketSpan(s.GetRawData());
}
