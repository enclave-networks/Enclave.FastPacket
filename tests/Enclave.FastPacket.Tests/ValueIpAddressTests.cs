using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Enclave.FastPacket.Tests;

public class ValueIpAddressTests
{
    [Test]
    public void CanReadIpv4Address()
    {
        var ipAddress = IPAddress.Parse("100.1.1.1");

        var valueIp = ValueIpAddress.Create(ipAddress);

        Assert.AreEqual("100.1.1.1", valueIp.ToString());
    }

    [Test]
    public void CanCompareIpv4Address()
    {
        var ipAddress1 = IPAddress.Parse("100.1.1.1");
        var ipAddress2 = IPAddress.Parse("101.1.1.1");

        var valueIp = ValueIpAddress.Create(ipAddress1);

        var repeat = ValueIpAddress.Create(ipAddress1);

        var notCorrectIp = ValueIpAddress.Create(ipAddress2);

        Assert.AreEqual(valueIp, repeat);
        Assert.AreNotEqual(notCorrectIp, valueIp);
    }

    [TestCase("2001:db8:3333:4444:5555:6666:7777:8888")]
    [TestCase("2001:db8:3333:4444:cccc:dddd:eeee:ffff")]
    [TestCase("::")]
    [TestCase("::18.52.86.120")]
    [TestCase("2001:db8::1234:5678")]
    public void CanProcessIpv6Addresses(string ipv6Address)
    {
        var ipAddress = IPAddress.Parse(ipv6Address);

        var valueIp = ValueIpAddress.Create(ipAddress);

        var repeat = ValueIpAddress.Create(ipAddress);

        Assert.AreEqual(valueIp, repeat);
        Assert.AreEqual(ipv6Address, valueIp.ToString());
    }

    [Test]
    public void EmptyIpv6AddressNotSameAsEmptyIpv4()
    {
        var emptyIpv6 = IPAddress.Parse("::");
        var emptyIpv4 = IPAddress.Parse("0.0.0.0");

        var valueIpv6 = ValueIpAddress.Create(emptyIpv6);
        var valueIpv4 = ValueIpAddress.Create(emptyIpv4);

        Assert.AreNotEqual(valueIpv6, valueIpv4);
    }
}
