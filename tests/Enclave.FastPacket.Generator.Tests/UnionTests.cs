using Enclave.FastPacket.Generator.Tests.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace Enclave.FastPacket.Generator.Tests;

[UsesVerify]
public class UnionTests
{
    public UnionTests()
        : base()
    {
    }

    [Fact]
    public Task CanGenerateUnion()
    {
        return CompilationVerifier.Verify(@"

using Enclave.FastPacket.Generator;

namespace T
{
    internal class PacketDefinition
    {
        public ushort Value { get; set; }

        [PacketField(Size = sizeof(byte))]
        struct Union1
        {
            public byte UnionVal1 { get; set; }

            public byte UnionVal2 { get; set; }
        }

        public ushort Value2 { get; set; }
    }

    [PacketImplementation(typeof(PacketDefinition))]
    public readonly ref partial struct ValueItem
    {   
    }
}
            ");
    }

    [Fact]
    public Task CanGenerateUnionWithBitmask()
    {
        return CompilationVerifier.Verify(@"

using Enclave.FastPacket.Generator;

namespace T
{
    internal class PacketDefinition
    {
        public ushort Value { get; set; }

        [PacketField(Size = sizeof(byte))]
        struct Union1
        {
            [PacketFieldBits(0, 3)]
            public byte UnionVal1 { get; set; }

            [PacketFieldBits(4, 7)]
            public byte UnionVal2 { get; set; }
        }

        public ushort Value2 { get; set; }
    }

    [PacketImplementation(typeof(PacketDefinition))]
    public readonly ref partial struct ValueItem
    {   
    }
}
            ");
    }

    [Fact]
    public Task CanGenerateUnionWithLargeBitmask()
    {
        return CompilationVerifier.Verify(@"

using Enclave.FastPacket.Generator;

namespace T
{
    [Flags]
    public enum FragmentFlags
    {
        Reserved = 0x00,
        DontFragment = 0x01,
        MoreFragments = 0x02,
    }
    internal class PacketDefinition
    {
        public ushort Value { get; set; }

        [PacketField(Size = sizeof(ushort))]
        private struct U3
        {
            [PacketFieldBits(0, 2)]
            public FragmentFlags Flags { get; set; }

            [PacketFieldBits(3, 15)]
            public ushort FragmentValue { get; set; }
        }

        public ushort Value2 { get; set; }
    }

    [PacketImplementation(typeof(PacketDefinition))]
    public readonly ref partial struct ValueItem
    {   
    }
}
            ");
    }
}
