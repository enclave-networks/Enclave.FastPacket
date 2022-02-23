using Enclave.FastPacket.Generator.Tests.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace Enclave.FastPacket.Generator.Tests;

[UsesVerify]
public class SizeFunctionTests
{
    [Fact]
    public Task CanHaveOptionalPosition()
    {
        return CompilationVerifier.Verify(@"
using System;
using Enclave.FastPacket.Generator;

namespace T
{
    internal ref struct PacketDefinition
    {
        [PacketField(SizeFunction = nameof(ValueFunc))]
        public ReadOnlySpan<byte> Value { get; set; }

        [PacketField(SizeFunction = nameof(Value2Func))]
        public ReadOnlySpan<byte> Value2 { get; set; }

        public ReadOnlySpan<byte> Remaining { get; set; }

        public static int ValueFunc(ReadOnlySpan<byte> span, int position)
        {
            return position;
        }

        public static int Value2Func(ReadOnlySpan<byte> span)
        {
            return 1;
        }
    }

    [PacketImplementation(typeof(PacketDefinition))]
    public readonly ref partial struct ValueItem
    {   
    }
}
            ");
    }
}
