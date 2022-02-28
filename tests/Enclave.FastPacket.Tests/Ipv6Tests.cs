using Xunit;
using PacketDotNet;
using System;
using System.Net;
using FluentAssertions;
using System.Buffers.Binary;

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

    [Fact]
    public void Ipv6ExtensionVisitorReturnsPayloadSegment()
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

        var visitor = myIp.GetExtensionVisitor();

        visitor.GetActualPayload(out var payloadSpan, out var protocol);

        Assert.Equal(IpProtocol.Udp, protocol);

        var udp = new ReadOnlyUdpPacketSpan(payloadSpan);

        udp.SourcePort.Should().Be(1024);
        udp.DestinationPort.Should().Be(65102);
        udp.Payload.Should().Equal(new byte[] { 0x01, 0x02, 0x03 });
    }

    [Fact]
    public void Ipv6ExtensionCanBeRead()
    {
        var packetHex = Convert.FromHexString(
            "3333000000160800" +
            "27d410bb86dd6000" +
            "000000380001fe80" +
            "0000000000000a00" +
            "27fffed410bbff02" +
            "0000000000000000" +
            "0000000000163a00" +
            "0502000001008f00" +
            "2b5a000000020400" +
            "0000ff0500000000" +
            "0000000000000001" +
            "000304000000ff02" +
            "0000000000000000" +
            "000000010002");

        var ethernet = new ReadOnlyEthernetPacketSpan(packetHex);

        Assert.Equal(EthernetType.IPv6, ethernet.Type);

        var ipv6 = new ReadOnlyIpv6PacketSpan(ethernet.Payload);

        var protocolVisitor = new HopByHopVisitor();

        var visitCount = 0;
        ipv6.GetExtensionVisitor().Visit(protocolVisitor, ref visitCount);

        Assert.Equal(1, visitCount);
    }

    private class HopByHopVisitor : IIpv6ExtensionVisitor<int>
    {
        public void VisitIpv6FragmentExtension(ref int state, IpProtocol protocol, ReadOnlyIpv6FragmentExtensionSpan span)
        {
            throw new NotImplementedException();
        }

        public void VisitIpv6HopByHopAndDestinationExtension(ref int state, IpProtocol protocol, ReadOnlyIpv6HopByHopAndDestinationExtensionSpan span)
        {
            Assert.Equal(IpProtocol.IPv6HopByHopOptions, protocol);
            Assert.Equal(IpProtocol.IcmpV6, span.NextHeader);

            // Router alert.
            Assert.Equal(0x05, span.OptionsAndPadding[0]);
            Assert.Equal(0x02, span.OptionsAndPadding[1]);

            state++;
        }

        public void VisitIpv6RoutingExtension(ref int state, IpProtocol protocol, ReadOnlyIpv6RoutingExtensionSpan span)
        {
            throw new NotImplementedException();
        }
    }
}
