using NUnit.Framework;
using PacketDotNet;
using PacketDotNet.Tcp;
using PacketDotNet.Utils;
using System;
using System.Net;

namespace Enclave.FastPacket.Tests;

public class Ipv4Tests
{
    [Test]
    public void CanReadIpv4PacketWithUdpPayload()
    {
        var sourceIp = IPAddress.Parse("10.0.0.1");
        var destIp = IPAddress.Parse("127.0.0.2");

        var packet = new PacketDotNet.IPv4Packet(sourceIp, destIp);
        packet.FragmentFlags = 2;
        packet.FragmentOffset = 56;
        packet.Id = 10;
        packet.HopLimit = 15;
        packet.DifferentiatedServices = 2;

        var payload = new UdpPacket(1024, 65102);
        payload.PayloadData = new byte[] { 0x01, 0x02, 0x03 };

        packet.PayloadPacket = payload;

        var packetData = packet.Bytes;

        var myIp = new Ipv4PacketReadOnlySpan(packetData);

        Assert.AreEqual(System.Net.Sockets.ProtocolType.Udp, myIp.Protocol);
        Assert.AreEqual(sourceIp, myIp.Source.ToIpAddress());
        Assert.AreEqual(destIp, myIp.Destination.ToIpAddress());
        Assert.AreEqual(FragmentFlags.MoreFragments, myIp.FragmentFlags);
        Assert.AreEqual(56, myIp.FragmentValue);
        Assert.AreEqual(10, myIp.Identification);
        Assert.AreEqual(2, myIp.Dscp);
        Assert.AreEqual(15, myIp.Ttl);
        Assert.AreEqual(0, myIp.Options.Length);
        
        var udp = new UdpPacketReadOnlySpan(myIp.Payload);

        Assert.AreEqual(1024, udp.SourcePort);
        Assert.AreEqual(65102, udp.DestinationPort);
        Assert.True(udp.Payload.SequenceEqual(new byte[] { 0x01, 0x02, 0x03 }));
    }

    [Test]
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

        var myIp = new Ipv4PacketReadOnlySpan(packetData);

        Assert.AreEqual(System.Net.Sockets.ProtocolType.Tcp, myIp.Protocol);
        Assert.AreEqual(sourceIp, myIp.Source.ToIpAddress());
        Assert.AreEqual(destIp, myIp.Destination.ToIpAddress());

        var tcp = new TcpPacketReadOnlySpan(myIp.Payload);

        Assert.AreEqual(1024, tcp.SourcePort);
        Assert.AreEqual(65102, tcp.DestinationPort);
        Assert.AreEqual(TcpFlags.Psh | TcpFlags.Urg, tcp.Flags);
        Assert.AreEqual(15, tcp.UrgentPointer);
        Assert.AreEqual(2000, tcp.WindowSize);
        Assert.True(tcp.Payload.SequenceEqual(new byte[] { 0x01, 0x02, 0x03 }));
    }

    [Test]
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

        Assert.AreEqual(System.Net.Sockets.ProtocolType.Icmp, myIp.Protocol);
        Assert.AreEqual(sourceIp, myIp.Source.ToIpAddress());
        Assert.AreEqual(destIp, myIp.Destination.ToIpAddress());

        var icmp = new Icmpv4PacketSpan(myIp.Payload);

        Assert.AreEqual(Icmpv4Types.DestinationUnreachable, icmp.Type);

        // 3 is the code for port unreachable.
        Assert.AreEqual(3, icmp.Code);
    }

    [Test]
    public void CanCreateBlankIpPacket()
    {
        //var length = Ipv4PacketSpan.GetLengthIncludingPayload(128);

        //var buffer = new byte[length];

        //var blank = Ipv4PacketSpan.CreateBlank(buffer, 128);

        //Assert.AreEqual(0x45, buffer[0]);
    }

}
