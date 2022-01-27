using FluentAssertions;
using Xunit;

namespace Enclave.FastPacket.Tests;

public class RealPacketTests
{
    /// <summary>
    /// Packet captured on Linux Ubuntu 20.04; MongoDB response packet that was failing to be routed correctly on older versions.
    /// </summary>
    [Fact]
    public void MongoDbSshTcpPacket()
    {
        var packetContent = FixtureHelpers.LoadFixture("mongodb-ssh-tcp-packet.bin");

        var ethPacket = new EthernetPacketSpan(packetContent);

        ethPacket.Type.Should().Be(EthernetType.IPv4);

        var ipSpan = new Ipv4PacketReadOnlySpan(ethPacket.Payload);

        ipSpan.Source.ToString().Should().Be("100.73.154.85");
        ipSpan.Destination.ToString().Should().Be("100.83.102.174");
        ipSpan.Protocol.Should().Be(IpProtocol.Tcp);

        var tcpSpan = new TcpPacketReadOnlySpan(ipSpan.Payload);

        tcpSpan.SourcePort.Should().Be(27017);
        tcpSpan.DestinationPort.Should().Be(59272);

        tcpSpan.Payload.Should().HaveCount(1922);
    }
}
