using FluentAssertions;
using System.Net;
using Xunit;

namespace Enclave.FastPacket.Tests;

public class ValueIpAddressTests
{
    [Fact]
    public void CanReadIpv4Address()
    {
        var ipAddress = IPAddress.Parse("100.1.1.1");

        var valueIp = ValueIpAddress.Create(ipAddress);

        valueIp.ToString().Should().Be("100.1.1.1");
    }

    [Fact]
    public void CanCompareIpv4Address()
    {
        var ipAddress1 = IPAddress.Parse("100.1.1.1");
        var ipAddress2 = IPAddress.Parse("101.1.1.1");

        var valueIp = ValueIpAddress.Create(ipAddress1);

        var repeat = ValueIpAddress.Create(ipAddress1);

        var notCorrectIp = ValueIpAddress.Create(ipAddress2);

        repeat.Should().Be(valueIp);
        notCorrectIp.Should().NotBe(valueIp);
    }

    [Theory]
    [InlineData("2001:db8:3333:4444:5555:6666:7777:8888")]
    [InlineData("2001:db8:3333:4444:cccc:dddd:eeee:ffff")]
    [InlineData("::")]
    [InlineData("::18.52.86.120")]
    [InlineData("2001:db8::1234:5678")]
    public void CanProcessIpv6Addresses(string ipv6Address)
    {
        var ipAddress = IPAddress.Parse(ipv6Address);

        var valueIp = ValueIpAddress.Create(ipAddress);

        var repeat = ValueIpAddress.Create(ipAddress);

        repeat.Should().Be(valueIp);
        valueIp.ToString().Should().Be(ipv6Address);
    }

    [Fact]
    public void EmptyIpv6AddressNotSameAsEmptyIpv4()
    {
        var emptyIpv6 = IPAddress.Parse("::");
        var emptyIpv4 = IPAddress.Parse("0.0.0.0");

        var valueIpv6 = ValueIpAddress.Create(emptyIpv6);
        var valueIpv4 = ValueIpAddress.Create(emptyIpv4);

        valueIpv6.Should().NotBe(valueIpv4);
    }
}
