using System.Runtime.CompilerServices;
using VerifyTests;

namespace Enclave.FastPacket.Generator.Tests.Helpers;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifySourceGenerators.Enable();
    }
}
