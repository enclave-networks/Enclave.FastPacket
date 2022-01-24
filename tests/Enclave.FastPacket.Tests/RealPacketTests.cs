using NUnit.Framework;
using PacketDotNet.Utils;
using System.IO;
using System.Net.Sockets;

namespace Enclave.FastPacket.Tests;

public class RealPacketTests
{
    /// <summary>
    /// Packet captured on Linux Ubuntu 20.04; MongoDB response packet that was failing to be routed correctly on older versions.
    /// </summary>
    [Test]
    public void MongoDbSshTcpPacket()
    {
        var packetContent = File.ReadAllBytes(Path.Combine(TestContext.CurrentContext.TestDirectory, "Fixtures", "mongodb-ssh-tcp-packet.bin"));

        var ethPacket = new EthernetPacketSpan(packetContent);

        Assert.AreEqual(EthernetType.IPv4, ethPacket.Type);

        var ipSpan = new Ipv4PacketReadOnlySpan(ethPacket.Payload);

        Assert.AreEqual("100.73.154.85", ipSpan.Source.ToString());
        Assert.AreEqual("100.83.102.174", ipSpan.Destination.ToString());
        Assert.AreEqual(ProtocolType.Tcp, ipSpan.Protocol);

        var tcpSpan = new TcpPacketReadOnlySpan(ipSpan.Payload);

        Assert.AreEqual(27017, tcpSpan.SourcePort);
        Assert.AreEqual(59272, tcpSpan.DestinationPort);

        var tcpPacket = new PacketDotNet.TcpPacket(new ByteArraySegment(ipSpan.Payload.ToArray()));

        Assert.AreEqual(1922, tcpSpan.Payload.Length);
    }
}
