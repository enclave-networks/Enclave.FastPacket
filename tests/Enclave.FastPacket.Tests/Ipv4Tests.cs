using FluentAssertions;
using PacketDotNet;
using PacketDotNet.Utils;
using System;
using System.Linq;
using System.Net;
using Xunit;

namespace Enclave.FastPacket.Tests;

public class Ipv4Tests
{
    [Fact]
    public void CanReadIpv4PacketWithUdpPayload()
    {
        var sourceIp = IPAddress.Parse("10.0.0.1");
        var destIp = IPAddress.Parse("127.0.0.2");

        var packet = new PacketDotNet.IPv4Packet(sourceIp, destIp);
        packet.FragmentFlags = (ushort)FragmentFlags.MoreFragments;
        packet.FragmentOffset = 56;
        packet.Id = 10;
        packet.HopLimit = 15;
        packet.DifferentiatedServices = 2;

        var payload = new UdpPacket(1024, 65102);
        payload.PayloadData = new byte[] { 0x01, 0x02, 0x03 };

        packet.PayloadPacket = payload;

        var packetData = packet.Bytes;

        var myIp = new ReadOnlyIpv4PacketSpan(packetData);

        myIp.Protocol.Should().Be(IpProtocol.Udp);
        myIp.Source.ToIpAddress().Should().Be(sourceIp);
        myIp.Destination.ToIpAddress().Should().Be(destIp);
        myIp.FragmentFlags.Should().Be(FragmentFlags.MoreFragments);
        myIp.FragmentOffset.Should().Be(56);
        myIp.Identification.Should().Be(10);
        myIp.Dscp.Should().Be(2);
        myIp.Ttl.Should().Be(15);
        myIp.Options.Length.Should().Be(0);
        
        var udp = new ReadOnlyUdpPacketSpan(myIp.Payload);

        udp.SourcePort.Should().Be(1024);
        udp.DestinationPort.Should().Be(65102);
        udp.Payload.Should().Equal(new byte[] { 0x01, 0x02, 0x03 });
    }

    [Fact]
    public void CanReadIpv4PacketWithTcpPayload()
    {
        var sourceIp = IPAddress.Parse("10.0.0.1");
        var destIp = IPAddress.Parse("127.0.0.2");

        var packet = new PacketDotNet.IPv4Packet(sourceIp, destIp);

        var payload = new TcpPacket(1024, 65102);
        payload.PayloadData = new byte[] { 0x01, 0x02, 0x03 };
        payload.Push = true;
        payload.Urgent = true;
        payload.UrgentPointer = 15;
        payload.WindowSize = 2000;
        packet.PayloadPacket = payload;

        var packetData = packet.Bytes;

        var myIp = new ReadOnlyIpv4PacketSpan(packetData);

        myIp.Protocol.Should().Be(IpProtocol.Tcp);
        myIp.Source.ToIpAddress().Should().Be(sourceIp);
        myIp.Destination.ToIpAddress().Should().Be(destIp);
        var tcp = new ReadOnlyTcpPacketSpan(myIp.Payload);

        tcp.SourcePort.Should().Be(1024);
        tcp.DestinationPort.Should().Be(65102);
        tcp.Flags.Should().Be(TcpFlags.Psh | TcpFlags.Urg);
        tcp.UrgentPointer.Should().Be(15);
        tcp.WindowSize.Should().Be(2000);
        tcp.Payload.Should().Equal(new byte[] { 0x01, 0x02, 0x03 });
    }

    [Fact]
    public void CanReadIpv4PacketWithIcmpPayload()
    {
        var sourceIp = IPAddress.Parse("10.0.0.1");
        var destIp = IPAddress.Parse("127.0.0.2");

        var packet = new PacketDotNet.IPv4Packet(sourceIp, destIp);

        var icmpBytes = new ByteArraySegment(new byte[128]);

        var icmpPacket = new IcmpV4Packet(icmpBytes);
        icmpPacket.TypeCode = IcmpV4TypeCode.UnreachablePort;

        packet.PayloadPacket = icmpPacket;

        var packetData = packet.Bytes;

        var myIp = new Ipv4PacketSpan(packetData);

        myIp.Protocol.Should().Be(IpProtocol.Icmp);
        myIp.Source.ToIpAddress().Should().Be(sourceIp);
        myIp.Destination.ToIpAddress().Should().Be(destIp);

        var icmp = new Icmpv4PacketSpan(myIp.Payload);

        icmp.Type.Should().Be(Icmpv4Types.DestinationUnreachable);

        // 3 is the code for port unreachable.
        icmp.Code.Should().Be(3);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="standardMtu">Assumed Internet MTU</param>
    /// <param name="workingMtu">MTU of the encapsualting adapter that causing fragmentation</param>
    [Theory]
    [InlineData(1500, 1472, 1500)] // Standard MTU, No Encapsulation, No Fragmentation
    [InlineData(1500, 1472, 1400)] // Adjusted MTU closer to Standard MTU
    [InlineData(1500, 1472, 1280)] // Standard MTU, Encapsulating MTU
    [InlineData(1500, 1472, 576)]  // Encapsulation with low MTU
    [InlineData(9000, 1472, 576)]  // Encapsulation with low MTU
    public void CanReadIpv4FragmentedPacketWithIcmpPayload(int standardMtu, int icmpPayloadLength, int workingMtu)
    {
        const int ipHeaderSize = 20; // IPv4 header size
        const int icmpHeaderSize = 8; // ICMP header size
        const int ipIdentity = 0x1234;

        int icmpPacketPayloadMaxSize = (workingMtu - ipHeaderSize - icmpHeaderSize) % 8; // Ensure fragment size is a multiple of 8

        var sourceIp = IPAddress.Parse("10.0.0.1");
        var destIp = IPAddress.Parse("127.0.0.2");

        // Build ICMP message
        var icmpPayload = Enumerable.Range(0, icmpPayloadLength).Select(i => (byte)"ABCD"[i % 4]).ToArray();

        var icmpMessage = new IcmpV4Packet(new ByteArraySegment(icmpPayload.Take(icmpPacketPayloadMaxSize).ToArray()));
        icmpMessage.TypeCode = IcmpV4TypeCode.EchoReply;

        // Build first IPv4 packet fragment
        var firstFragment = new IPv4Packet(sourceIp, destIp)
        {
            PayloadPacket = icmpMessage,
            Protocol = ProtocolType.Icmp,
            Id = ipIdentity,
            FragmentFlags = (ushort)FragmentFlags.MoreFragments,
            FragmentOffset = 0
        };

        // Build second IPv4 packet fragment
        var secondFragment = new IPv4Packet(sourceIp, destIp)
        {
            PayloadData = icmpPayload.Skip(icmpPacketPayloadMaxSize).Take(icmpPayload.Length - icmpPacketPayloadMaxSize).ToArray(),
            Protocol = ProtocolType.Icmp,
            Id = ipIdentity,
            FragmentFlags = 0,
            FragmentOffset = icmpPacketPayloadMaxSize / 8 // Offset in 8-byte units
        };

        // Check first packet fragment
        var myIp1 = new Ipv4PacketSpan(firstFragment.Bytes);

        myIp1.Protocol.Should().Be(IpProtocol.Icmp);
        myIp1.Source.ToIpAddress().Should().Be(sourceIp);
        myIp1.Destination.ToIpAddress().Should().Be(destIp);
        myIp1.FragmentFlags.Should().Be(FragmentFlags.MoreFragments);
        myIp1.FragmentOffset.Should().Be(0);
        
        var icmp1 = new Icmpv4PacketSpan(myIp1.Payload);

        icmp1.Type.Should().Be(Icmpv4Types.EchoReply);
        icmp1.Code.Should().Be(0); // 0 is the code for EchoReply.
        icmp1.Data.Length.Should().Be(icmpPacketPayloadMaxSize - 8);

        // Check second packet fragment
        var myIp2 = new Ipv4PacketSpan(secondFragment.Bytes);

        myIp2.Protocol.Should().Be(IpProtocol.Icmp);
        myIp2.Source.ToIpAddress().Should().Be(sourceIp);
        myIp2.Destination.ToIpAddress().Should().Be(destIp);
        myIp2.FragmentFlags.Should().Be(FragmentFlags.None);
        myIp2.FragmentOffset.Should().Be((ushort)(icmpPacketPayloadMaxSize / 8));
        myIp2.Payload.Length.Should().Be(icmpPayloadLength - icmpPacketPayloadMaxSize); // Remaining payload size (without ICMP header).
    }
}
