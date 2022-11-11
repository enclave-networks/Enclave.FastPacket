using FluentAssertions;
using System;
using System.Net;
using System.Numerics;
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

    [Theory]
    [InlineData("81.152.41.187", 1368926651u)]
    [InlineData("100.154.122.4", 1687845380u)]
    [InlineData("4.122.154.100", 75143780u)]
    public void CanConvertToUInt(string ip, uint expected)
    {
        Assert.Equal(expected, ValueIpAddress.Create(IPAddress.Parse(ip)).ToUInt());
    }

    [Fact]
    public void CannotConvertIpv6ToUInt()
    {
        Assert.Throws<InvalidOperationException>(() => ValueIpAddress.Create(IPAddress.Parse("2001:db8:3333:4444:5555:6666:7777:8889")).ToUInt());
    }

    [Fact]
    public void CanConvertToBigInt()
    {
        var ip = ValueIpAddress.Create(IPAddress.Parse("2001:db8:3333:4444:5555:6666:7777:8889"));

        var expected = BigInteger.Parse("42540766427128305956041295149173016713");

        Assert.Equal(expected, ip.ToBigInteger());
    }

    [Theory]
    [InlineData("100.1.1.1", "101.1.1.1")]
    [InlineData("1.255.255.255", "2.0.0.0")]
    [InlineData("1.1.1.1", "1.1.1.2")]
    [InlineData("0.0.0.0", "0.0.0.1")]
    public void CanCompareLessThanIpv4Address(string left, string right)
    {
        var ipAddress1 = IPAddress.Parse(left);
        var ipAddress2 = IPAddress.Parse(right);

        var valueIp1 = ValueIpAddress.Create(ipAddress1);
        var valueIp2 = ValueIpAddress.Create(ipAddress2);
        var repeat1 = ValueIpAddress.Create(ipAddress1);

        (valueIp1 < valueIp2).Should().BeTrue();
        (valueIp2 < valueIp1).Should().BeFalse();
        (valueIp1 < repeat1).Should().BeFalse();

    }

    [Theory]
    [InlineData("100.1.1.1", "101.1.1.1")]
    [InlineData("1.255.255.255", "2.0.0.0")]
    [InlineData("1.1.1.1", "1.1.1.2")]
    [InlineData("0.0.0.0", "0.0.0.1")]
    public void CanCompareLessThanOrEqualIpv4Address(string left, string right)
    {
        var ipAddress1 = IPAddress.Parse(left);
        var ipAddress2 = IPAddress.Parse(right);

        var valueIp1 = ValueIpAddress.Create(ipAddress1);
        var valueIp2 = ValueIpAddress.Create(ipAddress2);
        var repeat1 = ValueIpAddress.Create(ipAddress1);

        (valueIp1 <= valueIp2).Should().BeTrue();
        (valueIp2 <= valueIp1).Should().BeFalse();
        (valueIp1 <= repeat1).Should().BeTrue();

    }

    [Theory]
    [InlineData("101.1.1.1", "100.1.1.1")]
    [InlineData("2.0.0.0", "1.255.255.255")]
    [InlineData("1.1.1.2", "1.1.1.1")]
    [InlineData("0.0.0.1", "0.0.0.0")]
    public void CanCompareGreaterThanIpv4Address(string left, string right)
    {
        var ipAddress1 = IPAddress.Parse(left);
        var ipAddress2 = IPAddress.Parse(right);

        var valueIp1 = ValueIpAddress.Create(ipAddress1);
        var valueIp2 = ValueIpAddress.Create(ipAddress2);
        var repeat1 = ValueIpAddress.Create(ipAddress1);

        (valueIp1 > valueIp2).Should().BeTrue();
        (valueIp2 > valueIp1).Should().BeFalse();
        (valueIp1 > repeat1).Should().BeFalse();

    }

    [Theory]
    [InlineData("101.1.1.1", "100.1.1.1")]
    [InlineData("2.0.0.0", "1.255.255.255")]
    [InlineData("1.1.1.2", "1.1.1.1")]
    [InlineData("0.0.0.1", "0.0.0.0")]
    public void CanCompareGreaterThanOrEqualIpv4Address(string left, string right)
    {
        var ipAddress1 = IPAddress.Parse(left);
        var ipAddress2 = IPAddress.Parse(right);

        var valueIp1 = ValueIpAddress.Create(ipAddress1);
        var valueIp2 = ValueIpAddress.Create(ipAddress2);
        var repeat1 = ValueIpAddress.Create(ipAddress1);

        (valueIp1 >= valueIp2).Should().BeTrue();
        (valueIp2 >= valueIp1).Should().BeFalse();
        (valueIp1 >= repeat1).Should().BeTrue();

    }

    [Theory]
    [InlineData("2001:db8:3333:4444:5555:6666:7777:8889", "2001:db8:3333:4444:5555:6666:7777:8888")]
    [InlineData("2001:db8:3333:4445:5555:6666:7777:8888", "2001:db8:3333:4444:5555:6666:7777:8888")]
    [InlineData("2002:db8:3333:4444:5555:6666:7777:8888", "2001:db8:3333:4444:5555:6666:7777:8888")]
    [InlineData("::1", "::")]
    [InlineData("::18.52.86.121", "::18.52.86.120")]
    [InlineData("2002::", "2001:db8::1234:5678")]
    [InlineData("1::", "::1")]
    public void CanCompareGreaterThanIpv6Address(string left, string right)
    {
        var ipAddress1 = IPAddress.Parse(left);
        var ipAddress2 = IPAddress.Parse(right);

        var valueIp1 = ValueIpAddress.Create(ipAddress1);
        var valueIp2 = ValueIpAddress.Create(ipAddress2);
        var repeat1 = ValueIpAddress.Create(ipAddress1);

        (valueIp1 > valueIp2).Should().BeTrue();
        (valueIp2 > valueIp1).Should().BeFalse();
        (valueIp1 > repeat1).Should().BeFalse();

    }

    [Theory]
    [InlineData("2001:db8:3333:4444:5555:6666:7777:8889", "2001:db8:3333:4444:5555:6666:7777:8888")]
    [InlineData("2001:db8:3333:4445:5555:6666:7777:8888", "2001:db8:3333:4444:5555:6666:7777:8888")]
    [InlineData("2002:db8:3333:4444:5555:6666:7777:8888", "2001:db8:3333:4444:5555:6666:7777:8888")]
    [InlineData("::1", "::")]
    [InlineData("::18.52.86.121", "::18.52.86.120")]
    [InlineData("2002::", "2001:db8::1234:5678")]
    [InlineData("1::", "::1")]
    public void CanCompareGreaterThanOrEqualIpv6Address(string left, string right)
    {
        var ipAddress1 = IPAddress.Parse(left);
        var ipAddress2 = IPAddress.Parse(right);

        var valueIp1 = ValueIpAddress.Create(ipAddress1);
        var valueIp2 = ValueIpAddress.Create(ipAddress2);
        var repeat1 = ValueIpAddress.Create(ipAddress1);

        (valueIp1 >= valueIp2).Should().BeTrue();
        (valueIp2 >= valueIp1).Should().BeFalse();
        (valueIp1 >= repeat1).Should().BeTrue();

    }
}
