using Enclave.FastPacket.Generator.Tests.Helpers;

namespace Enclave.FastPacket.Generator.Tests;

[UsesVerify]
public class GeneratorTest
{
    [Fact]
    public Task CanGenerateDefaultType()
    {
        return FluentVerify.ForSource(@"
            using Enclave.FastPacket.Generator;

            namespace T
            {
                internal interface IPacketDefinition
                {
                    ushort Value { get; set; }
                }

                [PacketImplementation(typeof(IPacketDefinition))]
                public readonly ref partial struct ValueItem
                {   
                }
            }").Verify();
    }

    [Fact]
    public Task CanGenerateTypeCustomToString()
    {
        return FluentVerify.ForSource(@"
            using Enclave.FastPacket.Generator;

            namespace T
            {
                internal interface IPacketDefinition
                {
                    ushort Value { get; set; }
                }

                [PacketImplementation(typeof(IPacketDefinition))]
                public readonly ref partial struct ValueItem
                {
                    public override string ToString()
                    {
                        return $""{Value}"";
                    }
                }
            }").Verify();
    }


    [Fact]
    public Task CanGenerateTypeWithPositionFunction()
    {
        return FluentVerify.ForSource(@"

using System;
using Enclave.FastPacket.Generator;

namespace T
{
    internal class PacketDefinition
    {
        [PacketField(Position = 6)]
        public ushort Value { get; set; }

        [PacketField(PositionFunction = nameof(GetNextValuePosition))]
        public ushort NextValue { get; set; }

        public static int GetNextValuePosition(ReadOnlySpan<byte> packetData, int defaultPosition)
        {
            return defaultPosition;
        }
    }

    [PacketImplementation(typeof(PacketDefinition))]
    public readonly ref partial struct ValueItem
    {   
    }
}
            ").Verify();
    }


    [Fact]
    public Task CanGenerateTypeWithEnumValue()
    {
        return FluentVerify.ForSource(@"

using System;
using Enclave.FastPacket.Generator;

namespace T
{
    [Flags]
    public enum TcpFlags
    {
        Syn = 0x01,
    }

    internal class PacketDefinition
    {
        [PacketField]
        public TcpFlags Value { get; set; }
    }

    [PacketImplementation(typeof(PacketDefinition))]
    public readonly ref partial struct ValueItem
    {   
    }
}
            ").Verify();
    }

    [Fact]
    public Task CanGenerateTypeWithEnumCustomBackingTypeValue()
    {
        return FluentVerify.ForSource(@"

using System;
using Enclave.FastPacket.Generator;

namespace T
{
    [Flags]
    public enum TcpFlags
    {
        Syn = 0x01,
    }

    internal class PacketDefinition
    {
        [PacketField(EnumBackingType = typeof(byte))]
        public TcpFlags Value { get; set; }
    }

    [PacketImplementation(typeof(PacketDefinition))]
    public readonly ref partial struct ValueItem
    {   
    }
}
            ").Verify();
    }


    [Fact]
    public Task CanGenerateTypeWithCustomTypeSizeConstant()
    {
        return FluentVerify.ForSource(@"

using System;
using Enclave.FastPacket.Generator;

namespace T
{
    public struct HardwareAddress
    {
        public const int Size = 6;

        public HardwareAddress(ReadOnlySpan<byte> span)
        {
        }

        public void CopyTo(Span<byte> destination)
        {
        }
    }

    internal class PacketDefinition
    {
        /// <summary> This is value 1 </summary>
        /// <remarks>
        /// Some extra stuff
        /// </remarks>
        public int Value1 { get; set; }

        public HardwareAddress Source { get; set; }

        public HardwareAddress Destination { get; set; }

        public ushort Value2 { get; set; }
    }

    [PacketImplementation(typeof(PacketDefinition), IsReadOnly = true)]
    public readonly ref partial struct PacketParser
    {   
    }
}
            ").Verify();
    }

    [Fact]
    public Task CanGenerateTypeWithExternalSize()
    {
        return FluentVerify.ForSource(@"

using System;
using Enclave.FastPacket.Generator;

namespace T
{
    public struct HardwareAddress
    {
        public HardwareAddress(ReadOnlySpan<byte> span)
        {
        }

        public void CopyTo(Span<byte> destination)
        {
        }
    }

    internal class PacketDefinition
    {
        /// <summary> This is value 1 </summary>
        /// <remarks>
        /// Some extra stuff
        /// </remarks>
        public int Value1 { get; set; }

        [PacketField(Size = 6)]
        public HardwareAddress Source { get; set; }

        [PacketField(Size = 6)]
        public HardwareAddress Destination { get; set; }

        public ushort Value2 { get; set; }
    }

    [PacketImplementation(typeof(PacketDefinition))]
    public readonly ref partial struct PacketParser
    {   
    }
}
            ").Verify();
    }

    [Fact]
    public Task CanGenerateTypeWithPayload()
    {
        return FluentVerify.ForSource(@"

using System;
using Enclave.FastPacket.Generator;

namespace T
{
    internal class PacketDefinition
    {
        public int Value1 { get; set; }

        public ReadOnlySpan<byte> Payload { get; set; }
    }

    [PacketImplementation(typeof(PacketDefinition))]
    public readonly ref partial struct PacketParser
    {   
    }
}
            ").Verify();
    }

    [Fact]
    public Task CanGenerateTypeWithLongerNamespace()
    {
        return FluentVerify.ForSource(@"

using System;
using Enclave.FastPacket.Generator;

namespace Enclave.FastPacket
{
    internal class PacketDefinition
    {
        public int Value1 { get; set; }
    }

    [PacketImplementation(typeof(PacketDefinition))]
    public readonly ref partial struct PacketParser
    {   
    }
}
            ").Verify();
    }

    [Fact]
    public Task CanGenerateTypeWithPrivateMember()
    {
        return FluentVerify.ForSource(@"

using System;
using Enclave.FastPacket.Generator;

namespace T
{
    internal class PacketDefinition
    {
        private ushort Value { get; set; }

        public ushort NextValue { get; set; }
    }

    [PacketImplementation(typeof(PacketDefinition))]
    public readonly ref partial struct ValueItem
    {   
    }
}
            ").Verify();
    }

}
