using System;
using System.IO;

namespace Enclave.FastPacket.Tests;

internal static class FixtureHelpers
{
    public static byte[] LoadFixture(string name)
    {   
        return File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Fixtures", name));
    }
}
