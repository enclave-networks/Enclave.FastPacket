using BenchmarkDotNet.Attributes;
using PacketDotNet;
using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

namespace Enclave.FastPacket.Benchmarks.Packets;

[MemoryDiagnoser]
public class PacketInspectionBenchmark
{
    private byte[] _packetContent;

    [GlobalSetup]
    public void Setup()
    {
        var ethernetPacket = new PacketDotNet.EthernetPacket(
            PhysicalAddress.Parse("FF:FF:FF:FF:FF:FF"),
            PhysicalAddress.Parse("00:FF:00:FF:FF:FF"),
            PacketDotNet.EthernetType.IPv4);

        var ipPacket = new IPv4Packet(IPAddress.Parse("127.0.0.1"), IPAddress.Parse("10.0.0.1"));

        var udpPacket = new PacketDotNet.UdpPacket(1024, 63000);

        var data = new byte[32_000];

        Array.Fill<byte>(data, 0xFF);

        udpPacket.PayloadData = data;

        ipPacket.PayloadPacket = udpPacket;

        ethernetPacket.PayloadPacket = ipPacket;

        _packetContent = ethernetPacket.Bytes;
    }

    [Benchmark]
    public void InspectPacketDotNet()
    {
        var byteArraySegment = new PacketDotNet.Utils.ByteArraySegment(_packetContent);

        var ethernetPacket = new PacketDotNet.EthernetPacket(byteArraySegment);

        if (ethernetPacket.Type == PacketDotNet.EthernetType.IPv4)
        {
            var ipPacket = ethernetPacket.Extract<IPPacket>();

            var protocol = ipPacket.Protocol;

            if (protocol == ProtocolType.Udp)
            {
                var udpPacket = ipPacket.Extract<PacketDotNet.UdpPacket>();

                var srcPort = udpPacket.SourcePort;
                var dstPort = udpPacket.DestinationPort;

                if (srcPort != 1024 || dstPort != 63000)
                {
                    throw new InvalidOperationException("bad ports");
                }
            }
        }
    }

    [Benchmark]
    public void InspectFastPacket()
    {
        var ethernetPacket = new EthernetPacketSpan(_packetContent);

        if (ethernetPacket.Type == EthernetType.IPv4)
        {
            var ipPacket = new Ipv4PacketSpan(ethernetPacket.Payload);

            if (ipPacket.Protocol == System.Net.Sockets.ProtocolType.Udp)
            {
                var udpPacket = new UdpPacketSpan(ipPacket.Payload);

                var srcPort = udpPacket.SourcePort;
                var dstPort = udpPacket.DestinationPort;

                if (srcPort != 1024 || dstPort != 63000)
                {
                    throw new InvalidOperationException("bad ports");
                }
            }
        }
    }

    [Benchmark]
    public void InspectPacketDotNetRepeatedAccess()
    {
        var byteArraySegment = new PacketDotNet.Utils.ByteArraySegment(_packetContent);

        var ethernetPacket = new PacketDotNet.EthernetPacket(byteArraySegment);

        if (ethernetPacket.Type == PacketDotNet.EthernetType.IPv4)
        {
            var ipPacket = ethernetPacket.Extract<IPPacket>();

            var protocol = ipPacket.Protocol;

            if (protocol == ProtocolType.Udp)
            {
                var udpPacket = ipPacket.Extract<PacketDotNet.UdpPacket>();

                foreach (var i in Enumerable.Range(0, 1000))
                {
                    var srcPort = udpPacket.SourcePort;
                    var dstPort = udpPacket.DestinationPort;

                    if (srcPort != 1024 || dstPort != 63000)
                    {
                        throw new InvalidOperationException("bad ports");
                    }
                }
            }
        }
    }

    [Benchmark]
    public void InspectFastPacketRepeatedAccess()
    {
        var ethernetPacket = new EthernetPacketSpan(_packetContent);

        if (ethernetPacket.Type == EthernetType.IPv4)
        {
            var ipPacket = new Ipv4PacketSpan(ethernetPacket.Payload);

            if (ipPacket.Protocol == System.Net.Sockets.ProtocolType.Udp)
            {
                var udpPacket = new UdpPacketSpan(ipPacket.Payload);

                foreach (var i in Enumerable.Range(0, 1000))
                {
                    var srcPort = udpPacket.SourcePort;
                    var dstPort = udpPacket.DestinationPort;

                    if (srcPort != 1024 || dstPort != 63000)
                    {
                        throw new InvalidOperationException("bad ports");
                    }
                }
            }
        }
    }
}
