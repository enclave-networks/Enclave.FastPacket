using Enclave.FastPacket.Generator.Tests.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace Enclave.FastPacket.Generator.Tests;

[UsesVerify]
public class SizeFieldTests
{
    [Fact]
    public Task CanUsePrecedingFieldForSize()
    {
        return FluentVerify.ForSource(@"
using System;
using Enclave.FastPacket.Generator;

namespace T
{
    internal ref struct PacketDefinition
    {
        public ushort DataSize { get; set; }

        [PacketField(SizeField = nameof(DataSize))]
        public ReadOnlySpan<byte> Value { get; set; }
    }

    [PacketImplementation(typeof(PacketDefinition))]
    public readonly ref partial struct ValueItem
    {   
    }
}
            ").Verify();
    }

    [Fact]
    public Task SizeFieldCannotBeAfterDataField()
    {
        return FluentVerify.ForSource(@"
using System;
using Enclave.FastPacket.Generator;

namespace T
{
    internal ref struct PacketDefinition
    {
        [PacketField(SizeField = nameof(DataSize))]
        public ReadOnlySpan<byte> Value { get; set; }

        public ushort DataSize { get; set; }
    }

    [PacketImplementation(typeof(PacketDefinition))]
    public readonly ref partial struct ValueItem
    {   
    }
}
            ")
            .WithDiagnostics(Diagnostics.SizeFieldAppearsAfter)
            .Verify();
    }

    [Fact]
    public Task SizeFieldCanBeAfterDataFieldIfExplicitPositionUsed()
    {
        return FluentVerify.ForSource(@"
using System;
using Enclave.FastPacket.Generator;

namespace T
{
    internal ref struct PacketDefinition
    {
        [PacketField(SizeField = nameof(DataSize))]
        public ReadOnlySpan<byte> Value { get; set; }

        [PacketField(Position = 20)]
        public ushort DataSize { get; set; }
    }

    [PacketImplementation(typeof(PacketDefinition))]
    public readonly ref partial struct ValueItem
    {
    }
}
            ")
            .Verify();
    }

    [Fact]
    public Task SizeFieldMustBeNumeric()
    {
        return FluentVerify.ForSource(@"
using System;
using Enclave.FastPacket.Generator;

namespace T
{
    internal ref struct PacketDefinition
    {
        public bool DataSize { get; set; }

        [PacketField(SizeField = nameof(DataSize))]
        public ReadOnlySpan<byte> Value { get; set; }
    }

    [PacketImplementation(typeof(PacketDefinition))]
    public readonly ref partial struct ValueItem
    {   
    }
}
            ")
            .WithDiagnostics(Diagnostics.SizeFieldNotValidType)
            .Verify();
    }

    [Fact]
    public Task SizeFieldMustExist()
    {
        return FluentVerify.ForSource(@"
using System;
using Enclave.FastPacket.Generator;

namespace T
{
    internal ref struct PacketDefinition
    {
        [PacketField(SizeField = nameof(DataSize))]
        public ReadOnlySpan<byte> Value { get; set; }
    }

    [PacketImplementation(typeof(PacketDefinition))]
    public readonly ref partial struct ValueItem
    {   
    }
}
            ")
            .WithDiagnostics(Diagnostics.SizeFieldNotFound)
            .Verify();
    }

}
