using FluentAssertions;
using PacketDotNet;
using PacketDotNet.Utils;
using System;
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

    [Theory]
    [InlineData(IpProtocol.Udp, FragmentFlags.DontFragment)]
    [InlineData(IpProtocol.Icmp, FragmentFlags.MoreFragments)]
    [InlineData(IpProtocol.Icmp, FragmentFlags.Reserved)]
    [InlineData(IpProtocol.Tcp, FragmentFlags.None)]
    public void CanReadIpv4FragmentedPacket(IpProtocol protocol, FragmentFlags fragmentFlags)
    {
        var random = new Random();
        var fragmentOffset = (ushort)random.Next(0, 8191); // (13 bits available for offset)
        var sourceIp = IPAddress.Parse("10.0.0.1");
        var destIp = IPAddress.Parse("127.0.0.2");

        var packet = new IPv4Packet(sourceIp, destIp)
        {
            Id = (ushort)random.Next(0, ushort.MaxValue),
            Protocol = (ProtocolType)protocol,
            FragmentFlags = (ushort)fragmentFlags,
            FragmentOffset = fragmentOffset
        };

        var packetData = packet.Bytes;

        // Parse the packet
        var myIp = new ReadOnlyIpv4PacketSpan(packetData);

        myIp.Protocol.Should().Be(protocol);
        myIp.Source.ToIpAddress().Should().Be(sourceIp);
        myIp.Destination.ToIpAddress().Should().Be(destIp);
        myIp.FragmentFlags.Should().Be(fragmentFlags);
        myIp.FragmentOffset.Should().Be(fragmentOffset);
    }
}
