using BenchmarkDotNet.Attributes;
using PacketDotNet;
using System;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;

namespace Enclave.FastPacket.Benchmarks.Packets;

[MemoryDiagnoser]
public class PacketInspectionBenchmark
{
    private byte[] _etherPacketUdpPortsContent;
    private byte[] _etherPacketTcpContent;

    [GlobalSetup]
    public void Setup()
    {
       _etherPacketUdpPortsContent = GetEthernetPacketWithUdpPorts();
       _etherPacketTcpContent = GetRealEthernetAndTcpPacket();
    }

    [Benchmark]
    public void GetEthernetHardwareAddress_PacketDotNet()
    {
        var byteArraySegment = new PacketDotNet.Utils.ByteArraySegment(_etherPacketTcpContent);

        var ethernetPacket = new PacketDotNet.EthernetPacket(byteArraySegment);

        var hwAddress = ethernetPacket.DestinationHardwareAddress;

        if (hwAddress == PhysicalAddress.None)
        {
            throw new InvalidOperationException("bad hardware address");
        }
    }

    [Benchmark]
    public void GetEthernetHardwareAddress_FastPacket()
    {
        var ethernetPacket = new EthernetPacketSpan(_etherPacketTcpContent);

        var hwAddress = ethernetPacket.Destination;

        if (hwAddress == HardwareAddress.None)
        {
            throw new InvalidOperationException("bad hardware address");
        }
    }

    [Benchmark]
    public void GetUdpPorts_PacketDotNet()
    {
        var byteArraySegment = new PacketDotNet.Utils.ByteArraySegment(_etherPacketUdpPortsContent);

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
    public void GetUdpPorts_FastPacket()
    {
        var ethernetPacket = new EthernetPacketSpan(_etherPacketUdpPortsContent);

        if (ethernetPacket.Type == EthernetType.IPv4)
        {
            var ipPacket = new Ipv4PacketReadOnlySpan(ethernetPacket.Payload);

            if (ipPacket.Protocol == IpProtocol.Udp)
            {
                var udpPacket = new UdpPacketReadOnlySpan(ipPacket.Payload);

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
    public void GetTcpPorts_PacketDotNet()
    {
        var byteArraySegment = new PacketDotNet.Utils.ByteArraySegment(_etherPacketTcpContent);

        var ethernetPacket = new PacketDotNet.EthernetPacket(byteArraySegment);

        if (ethernetPacket.Type == PacketDotNet.EthernetType.IPv4)
        {
            var ipPacket = ethernetPacket.Extract<IPPacket>();

            var protocol = ipPacket.Protocol;

            if (protocol == ProtocolType.Tcp)
            {
                var tcpPacket = ipPacket.Extract<PacketDotNet.TcpPacket>();

                var srcPort = tcpPacket.SourcePort;
                var dstPort = tcpPacket.DestinationPort;

                if (srcPort != 27017 || dstPort != 59272)
                {
                    throw new InvalidOperationException("bad ports");
                }
            }
        }
    }

    [Benchmark]
    public void GetTcpPorts_FastPacket()
    {
        var ethernetPacket = new EthernetPacketSpan(_etherPacketTcpContent);

        if (ethernetPacket.Type == EthernetType.IPv4)
        {
            var ipPacket = new Ipv4PacketReadOnlySpan(ethernetPacket.Payload);

            if (ipPacket.Protocol == IpProtocol.Tcp)
            {
                var tcpPacket = new TcpPacketReadOnlySpan(ipPacket.Payload);

                var srcPort = tcpPacket.SourcePort;
                var dstPort = tcpPacket.DestinationPort;

                if (srcPort != 27017 || dstPort != 59272)
                {
                    throw new InvalidOperationException("bad ports");
                }
            }
        }
    }


    [Benchmark]
    public void GetTcpPayloadSize_PacketDotNet()
    {
        var byteArraySegment = new PacketDotNet.Utils.ByteArraySegment(_etherPacketTcpContent);

        var ethernetPacket = new PacketDotNet.EthernetPacket(byteArraySegment);

        if (ethernetPacket.Type == PacketDotNet.EthernetType.IPv4)
        {
            var ipPacket = ethernetPacket.Extract<IPPacket>();

            var protocol = ipPacket.Protocol;

            if (protocol == ProtocolType.Tcp)
            {
                var tcpPacket = ipPacket.Extract<PacketDotNet.TcpPacket>();

                var payloadSize = tcpPacket.PayloadDataSegment.Length;

                if (payloadSize != 1922)
                {
                    throw new InvalidOperationException("bad size");
                }
            }
        }
    }

    [Benchmark]
    public void GetTcpPayloadSize_FastPacket()
    {
        var ethernetPacket = new EthernetPacketSpan(_etherPacketTcpContent);

        if (ethernetPacket.Type == EthernetType.IPv4)
        {
            var ipPacket = new Ipv4PacketReadOnlySpan(ethernetPacket.Payload);

            if (ipPacket.Protocol == IpProtocol.Tcp)
            {
                var tcpPacket = new TcpPacketReadOnlySpan(ipPacket.Payload);

                var payloadSize = tcpPacket.Payload.Length;

                if (payloadSize != 1922)
                {
                    throw new InvalidOperationException("bad size");
                }
            }
        }
    }

    [Benchmark]
    public void CheckForTcpAck_PacketDotNet()
    {
        var byteArraySegment = new PacketDotNet.Utils.ByteArraySegment(_etherPacketTcpContent);

        var ethernetPacket = new PacketDotNet.EthernetPacket(byteArraySegment);

        if (ethernetPacket.Type == PacketDotNet.EthernetType.IPv4)
        {
            var ipPacket = ethernetPacket.Extract<IPPacket>();

            var protocol = ipPacket.Protocol;

            if (protocol == ProtocolType.Tcp)
            {
                var tcpPacket = ipPacket.Extract<PacketDotNet.TcpPacket>();

                if (!tcpPacket.Acknowledgment)
                {
                    throw new InvalidOperationException("bad flag");
                }
            }
        }
    }

    [Benchmark]
    public void CheckForTcpAck_FastPacket()
    {
        var ethernetPacket = new EthernetPacketSpan(_etherPacketTcpContent);

        if (ethernetPacket.Type == EthernetType.IPv4)
        {
            var ipPacket = new Ipv4PacketReadOnlySpan(ethernetPacket.Payload);

            if (ipPacket.Protocol == IpProtocol.Tcp)
            {
                var tcpPacket = new TcpPacketReadOnlySpan(ipPacket.Payload);

                if (!tcpPacket.Flags.HasFlag(TcpFlags.Ack))
                {
                    throw new InvalidOperationException("bad flag");
                }
            }
        }
    }

    private static byte[] GetEthernetPacketWithUdpPorts()
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

        return ethernetPacket.Bytes;
    }

    private static byte[] GetRealEthernetAndTcpPacket()
    {
        return File.ReadAllBytes(Path.Combine("Examples", "mongodb-ssh-tcp-packet.bin"));
    }
}
