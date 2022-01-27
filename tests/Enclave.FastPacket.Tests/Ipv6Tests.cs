using Xunit;
using PacketDotNet;
using System;
using System.Net;
using FluentAssertions;

namespace Enclave.FastPacket.Tests;

public class Ipv6Tests
{
    [Fact]
    public void CanReadIpv6PacketWithUdpPayload()
    {
        var sourceIp = IPAddress.Parse("2001:db8:3333:4444:5555:6666:7777:8888");
        var destIp = IPAddress.Parse("2001:db8::123.123.123.123");

        var packet = new PacketDotNet.IPv6Packet(sourceIp, destIp);
        packet.HopLimit = 10;
        packet.TrafficClass = 5;
        packet.FlowLabel = 1056;
        
        var payload = new UdpPacket(1024, 65102);
        payload.PayloadData = new byte[] { 0x01, 0x02, 0x03 };

        packet.PayloadPacket = payload;

        var packetData = packet.Bytes;

        var myIp = new Ipv6PacketSpan(packetData);

        myIp.NextHeader.Should().Be(IpProtocol.Udp);
        myIp.Source.ToIpAddress().Should().Be(sourceIp);
        myIp.Destination.ToIpAddress().Should().Be(destIp);

        var udp = new UdpPacketSpan(myIp.Payload);

        udp.SourcePort.Should().Be(1024);
        udp.DestinationPort.Should().Be(65102);
        udp.Payload.Should().Equal(new byte[] { 0x01, 0x02, 0x03 });
    }

    [Fact]
    public void CanReadIpv6PacketWithTcpPayload()
    {
        var sourceIp = IPAddress.Parse("2001:db8:3333:4444:5555:6666:7777:8888");
        var destIp = IPAddress.Parse("2001:db8::123.123.123.123");

        var packet = new PacketDotNet.IPv6Packet(sourceIp, destIp);

        var payload = new TcpPacket(1024, 65102);
        payload.PayloadData = new byte[] { 0x01, 0x02, 0x03 };

        packet.PayloadPacket = payload;

        var packetData = packet.Bytes;

        var myIp = new Ipv6PacketSpan(packetData);

        myIp.NextHeader.Should().Be(IpProtocol.Tcp);
        myIp.Source.ToIpAddress().Should().Be(sourceIp);
        myIp.Destination.ToIpAddress().Should().Be(destIp);

        var tcp = new TcpPacketSpan(myIp.Payload);

        tcp.SourcePort.Should().Be(1024);
        tcp.DestinationPort.Should().Be(65102);
        tcp.Payload.Should().Equal(new byte[] { 0x01, 0x02, 0x03 });
    }
}
