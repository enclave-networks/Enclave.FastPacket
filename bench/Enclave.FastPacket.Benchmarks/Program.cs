using BenchmarkDotNet.Running;
using System;
using System.Threading.Tasks;

namespace Enclave.FastPacket.Benchmarks;

class Program
{
    static void Main(string[] args)
    {
        //var bench = new WindowsTapIocpSingleRxPathBenchmark();
        //bench.Setup();

        //Console.WriteLine("Starting");

        //await bench.TapWrite1GB();
        //bench.Cleanup();
        new BenchmarkSwitcher(Benchmarks.All).Run(args);
    }
}
