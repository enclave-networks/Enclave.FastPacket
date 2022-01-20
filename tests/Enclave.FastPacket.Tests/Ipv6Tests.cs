using NUnit.Framework;
using PacketDotNet;
using System;
using System.Net;

namespace Enclave.FastPacket.Tests;

public class Ipv6Tests
{
    [Test]
    public void CanReadIpv6PacketWithUdpPayload()
    {
        var sourceIp = IPAddress.Parse("2001:db8:3333:4444:5555:6666:7777:8888");
        var destIp = IPAddress.Parse("2001:db8::123.123.123.123");

        var packet = new PacketDotNet.IPv6Packet(sourceIp, destIp);

        var payload = new UdpPacket(1024, 65102);
        payload.PayloadData = new byte[] { 0x01, 0x02, 0x03 };

        packet.PayloadPacket = payload;

        var packetData = packet.Bytes;

        var myIp = new Ipv6PacketSpan(packetData);

        Assert.AreEqual(System.Net.Sockets.ProtocolType.Udp, myIp.Protocol);
        Assert.AreEqual(sourceIp, myIp.Source.ToIpAddress());
        Assert.AreEqual(destIp, myIp.Destination.ToIpAddress());

        var udp = new UdpPacketSpan(myIp.Payload);

        Assert.AreEqual(1024, udp.SourcePort);
        Assert.AreEqual(65102, udp.DestinationPort);
        Assert.True(udp.Payload.SequenceEqual(new byte[] { 0x01, 0x02, 0x03 }));
    }

    [Test]
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

        Assert.AreEqual(System.Net.Sockets.ProtocolType.Tcp, myIp.Protocol);
        Assert.AreEqual(sourceIp, myIp.Source.ToIpAddress());
        Assert.AreEqual(destIp, myIp.Destination.ToIpAddress());

        var tcp = new TcpPacketSpan(myIp.Payload);

        Assert.AreEqual(1024, tcp.SourcePort);
        Assert.AreEqual(65102, tcp.DestinationPort);
        Assert.True(tcp.Payload.SequenceEqual(new byte[] { 0x01, 0x02, 0x03 }));
    }
}
