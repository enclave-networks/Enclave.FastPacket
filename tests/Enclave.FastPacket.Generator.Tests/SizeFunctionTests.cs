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
        var inputCompilation = CompilationVerifier.Create(@"
using System;
using Enclave.FastPacket.Generator;

namespace T
{
    internal ref struct PacketDefinition
    {
        [PacketField(SizeFunction = nameof(ValueFunc))]
        ReadOnlySpan<byte> Value { get; set; }

        [PacketField(SizeFunction = nameof(Value2Func))]
        ReadOnlySpan<byte> Value2 { get; set; }

        ReadOnlySpan<byte> Remaining { get; set; }

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

        var generator = new PacketParserGenerator();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out var diagnostics);

        return Verify(driver);
    }
}
