using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Enclave.FastPacket.Tests;

public class EthernetTests
{
    [Test]
    public void CanParseEthernetPacket()
    {
        var bytes = Convert.FromHexString("01 00 5E 00 00 0D C2 03 3D 80 00 01 08 00 45 C0 00 36 00 A3 00 00 01 67 CD E4 0A 00 00 0D E0 00 00 0D 20 00 B5 2E 00 01 00 02 00 69 00 14 00 04 D7 70 51 AB 00 13 00 04 00 00 00 01 00 15 00 04 01 00 00 00".Replace(" ", ""));

        var ethPacketSpan = new EthernetPacketSpan(bytes);

        var type = ethPacketSpan.Type;

        ethPacketSpan.Type = EthernetType.IPv6;

        Assert.AreEqual(EthernetType.IPv4, type);
    }
}
