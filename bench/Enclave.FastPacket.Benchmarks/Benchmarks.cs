using Enclave.FastPacket.Benchmarks.Packets;
using System;

namespace Enclave.FastPacket.Benchmarks;

public static class Benchmarks
{
    public static Type[] All => new[]
    {
            typeof(PacketInspectionBenchmark),
    };
}
