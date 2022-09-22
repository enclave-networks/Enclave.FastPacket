using Enclave.FastPacket.Generator.Tests.Helpers;

namespace Enclave.FastPacket.Generator.Tests;

[UsesVerify]
public class TypeTests
{
    public TypeTests()
        : base()
    {
    }

    [Fact]
    public Task CanGenerateBoolBitFields()
    {
        return FluentVerify.ForSource(@"

using Enclave.FastPacket.Generator;

namespace T
{
    internal class PacketDefinition
    {
        public ushort Value { get; set; }

        [PacketField(Size = sizeof(byte))]
        struct Union1
        {
            [PacketFieldBits(0)]
            public bool Value1 { get; set; }

            [PacketFieldBits(1)]
            public bool UnionVal2 { get; set; }
        }

        public ushort Value2 { get; set; }
    }

    [PacketImplementation(typeof(PacketDefinition))]
    public readonly ref partial struct ValueItem
    {   
    }
}
            ").Verify();
    }
}
