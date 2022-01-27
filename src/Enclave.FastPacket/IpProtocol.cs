namespace Enclave.FastPacket;

/// <summary>
/// Defines the possible values for the IP Protocol field.
/// </summary>
public enum IpProtocol
{
    /// <summary>
    /// IPv6 Hop-by-Hop options.
    /// </summary>
    IPv6HopByHopOptions = 0,

    /// <summary>
    /// Internet Control Message Protocol.
    /// </summary>
    Icmp = 1,

    /// <summary>
    /// Internet Group Management Protocol.
    /// </summary>
    Igmp = 2,

    /// <summary>
    /// Gateway (deprecated).
    /// </summary>
    Gpg = 3,

    /// <summary>
    /// IPv4.
    /// </summary>
    IPv4 = 4,

    /// <summary>
    /// Transmission Control Protocol.
    /// </summary>
    Tcp = 6,

    /// <summary>
    /// Exterior Gateway Protocol.
    /// </summary>
    Egp = 8,

    /// <summary>
    /// PUP protocol.
    /// </summary>
    Pup = 12,

    /// <summary>
    /// User Datagram Protocol.
    /// </summary>
    Udp = 17,

    /// <summary>
    /// XNS IDP protocol.
    /// </summary>
    Idp = 22,

    /// <summary>
    /// SO Transport Protocol Class 4.
    /// </summary>
    TP = 29,

    /// <summary>
    /// IPv6.
    /// </summary>
    IPv6 = 41,

    /// <summary>
    /// IPv6 routing header.
    /// </summary>
    IPv6RoutingHeader = 43,

    /// <summary>
    /// IPv6 fragmentation header.
    /// </summary>
    IPv6FragmentHeader = 44,

    /// <summary>
    /// Reservation Protocol.
    /// </summary>
    Rsvp = 46,

    /// <summary>
    /// General Routing Encapsulation.
    /// </summary>
    Gre = 47,

    /// <summary>
    /// Encapsulating security payload.
    /// </summary>
    IPSecEncapsulatingSecurityPayload = 50,

    /// <summary>
    /// Authentication header.
    /// </summary>
    IPSecAuthenticationHeader = 51,

    /// <summary>
    /// ICMPv6.
    /// </summary>
    IcmpV6 = 58,

    /// <summary>
    /// IPv6 no next header.
    /// </summary>
    IPv6NoNextHeader = 59,

    /// <summary>
    /// IPv6 destination options.
    /// </summary>
    IPv6DestinationOptions = 60,

    /// <summary>
    /// OSPF v2/v3.
    /// </summary>
    Ospf = 89,

    /// <summary>
    /// Multicast Transport Protocol.
    /// </summary>
    Mtp = 92,

    /// <summary>
    /// Encapsulation Header.
    /// </summary>
    Encapsulation = 98,

    /// <summary>
    /// Protocol Independent Multicast.
    /// </summary>
    Pim = 103,

    /// <summary>
    /// Compression Header Protocol.
    /// </summary>
    CompressionHeader = 108,

    /// <summary>
    /// Mobility Header Protocol.
    /// </summary>
    MobilityHeader = 135,

    /// <summary>
    /// Host Identity Protocol.
    /// </summary>
    HostIdentity = 139,

    /// <summary>
    /// Shim6 Protocol.
    /// </summary>
    Shim6 = 140,

    /// <summary>
    /// Raw IP packets.
    /// </summary>
    Raw = 255,
}
