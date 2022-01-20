using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;
using Xunit;

namespace Enclave.FastPacket.Generator.Tests
{
    public class GeneratorTest
    {
        [Fact]
        public void CanGenerateDefaultType()
        {
            var inputCompilation = CreateCompilation(@"

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
}
            ");

            var generator = new PacketParserGenerator();

            GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

            driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out var diagnostics);

            var result = driver.GetRunResult();

            Assert.Empty(result.Diagnostics);

            result.AssertGeneratedFile("T.ValueItem.Generated.cs", tree =>
            {
                var treeText = tree.GetText().ToString();

                // Make sure we created a GUID generator.
                Assert.Contains("private readonly Guid _backingId;", treeText);

                // Check that the typeconverter gets added.
                Assert.Contains("[TypeConverter(typeof(ValueItemTypeConverter))]", treeText);
            });

            result.AssertGeneratedFile("T.ValueItem.TypeConverter.cs");
        }

        [Fact]
        public void CanGenerateTypeWithPositionFunction()
        {
            var inputCompilation = CreateCompilation(@"

using System;
using Enclave.FastPacket.Generator;

namespace T
{
    internal class PacketDefinition
    {
        [PacketField(Position = 6)]
        ushort Value { get; set; }

        [PacketField(PositionFunction = nameof(GetNextValuePosition))]
        ushort NextValue { get; set; }

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
            ");

            var generator = new PacketParserGenerator();

            GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

            driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out var diagnostics);

            var result = driver.GetRunResult();

            Assert.Empty(result.Diagnostics);

            result.AssertGeneratedFile("T.ValueItem.Generated.cs", tree =>
            {
                var treeText = tree.GetText().ToString();

                // Make sure we created a GUID generator.
                Assert.Contains("private readonly Guid _backingId;", treeText);

                // Check that the typeconverter gets added.
                Assert.Contains("[TypeConverter(typeof(ValueItemTypeConverter))]", treeText);
            });

            result.AssertGeneratedFile("T.ValueItem.TypeConverter.cs");
        }


        [Fact]
        public void CanGenerateTypeWithEnumValue()
        {
            var inputCompilation = CreateCompilation(@"

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
        TcpFlags Value { get; set; }
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

            var result = driver.GetRunResult();

            Assert.Empty(result.Diagnostics);

            result.AssertGeneratedFile("T.ValueItem.Generated.cs", tree =>
            {
                var treeText = tree.GetText().ToString();

                // Make sure we created a GUID generator.
                Assert.Contains("private readonly Guid _backingId;", treeText);

                // Check that the typeconverter gets added.
                Assert.Contains("[TypeConverter(typeof(ValueItemTypeConverter))]", treeText);
            });

            result.AssertGeneratedFile("T.ValueItem.TypeConverter.cs");
        }

        [Fact]
        public void CanGenerateTypeWithEnumCustomBackingTypeValue()
        {
            var inputCompilation = CreateCompilation(@"

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
        TcpFlags Value { get; set; }
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

            var result = driver.GetRunResult();

            Assert.Empty(result.Diagnostics);

            result.AssertGeneratedFile("T.ValueItem.Generated.cs", tree =>
            {
                var treeText = tree.GetText().ToString();

                // Make sure we created a GUID generator.
                Assert.Contains("private readonly Guid _backingId;", treeText);

                // Check that the typeconverter gets added.
                Assert.Contains("[TypeConverter(typeof(ValueItemTypeConverter))]", treeText);
            });

            result.AssertGeneratedFile("T.ValueItem.TypeConverter.cs");
        }


        [Fact]
        public void CanGenerateTypeWithCustomTypeSizeConstant()
        {
            var inputCompilation = CreateCompilation(@"

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
        int Value1 { get; set; }

        HardwareAddress Source { get; set; }

        HardwareAddress Destination { get; set; }

        ushort Value2 { get; set; }
    }

    [PacketImplementation(typeof(PacketDefinition), IsReadOnly =    true)]
    public readonly ref partial struct PacketParser
    {   
    }
}
            ");

            var generator = new PacketParserGenerator();

            GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

            driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out var diagnostics);

            var result = driver.GetRunResult();

            Assert.Empty(result.Diagnostics);

            result.AssertGeneratedFile("T.ValueItem.Generated.cs", tree =>
            {
                var treeText = tree.GetText().ToString();

                // Make sure we created a GUID generator.
                Assert.Contains("private readonly Guid _backingId;", treeText);

                // Check that the typeconverter gets added.
                Assert.Contains("[TypeConverter(typeof(ValueItemTypeConverter))]", treeText);
            });

            result.AssertGeneratedFile("T.ValueItem.TypeConverter.cs");
        }

        [Fact]
        public void CanGenerateTypeWithExternalSize()
        {
            var inputCompilation = CreateCompilation(@"

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
        int Value1 { get; set; }

        [PacketField(Size = 6)]
        HardwareAddress Source { get; set; }

        [PacketField(Size = 6)]
        HardwareAddress Destination { get; set; }

        ushort Value2 { get; set; }
    }

    [PacketImplementation(typeof(PacketDefinition))]
    public readonly ref partial struct PacketParser
    {   
    }
}
            ");

            var generator = new PacketParserGenerator();

            GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

            driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out var diagnostics);

            var result = driver.GetRunResult();

            Assert.Empty(result.Diagnostics);

            result.AssertGeneratedFile("T.ValueItem.Generated.cs", tree =>
            {
                var treeText = tree.GetText().ToString();

                // Make sure we created a GUID generator.
                Assert.Contains("private readonly Guid _backingId;", treeText);

                // Check that the typeconverter gets added.
                Assert.Contains("[TypeConverter(typeof(ValueItemTypeConverter))]", treeText);
            });

            result.AssertGeneratedFile("T.ValueItem.TypeConverter.cs");
        }

        [Fact]
        public void CanGenerateTypeWithPayload()
        {
            var inputCompilation = CreateCompilation(@"

using System;
using Enclave.FastPacket.Generator;

namespace T
{
    internal class PacketDefinition
    {
        int Value1 { get; set; }

        ReadOnlySpan<byte> Payload { get; set; }
    }

    [PacketImplementation(typeof(PacketDefinition))]
    public readonly ref partial struct PacketParser
    {   
    }
}
            ");

            var generator = new PacketParserGenerator();

            GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

            driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out var diagnostics);

            var result = driver.GetRunResult();

            Assert.Empty(result.Diagnostics);

            result.AssertGeneratedFile("T.ValueItem.Generated.cs", tree =>
            {
                var treeText = tree.GetText().ToString();

                // Make sure we created a GUID generator.
                Assert.Contains("private readonly Guid _backingId;", treeText);

                // Check that the typeconverter gets added.
                Assert.Contains("[TypeConverter(typeof(ValueItemTypeConverter))]", treeText);
            });

            result.AssertGeneratedFile("T.ValueItem.TypeConverter.cs");
        }

        [Fact]
        public void CanGenerateTypeWithLongerNamespace()
        {
            var inputCompilation = CreateCompilation(@"

using System;
using Enclave.FastPacket.Generator;

namespace Enclave.FastPacket
{
    internal class PacketDefinition
    {
        int Value1 { get; set; }
    }

    [PacketImplementation(typeof(PacketDefinition))]
    public readonly ref partial struct PacketParser
    {   
    }
}
            ");

            var generator = new PacketParserGenerator();

            GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

            driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out var diagnostics);

            var result = driver.GetRunResult();

            Assert.Empty(result.Diagnostics);

            result.AssertGeneratedFile("T.ValueItem.Generated.cs", tree =>
            {
                var treeText = tree.GetText().ToString();

                // Make sure we created a GUID generator.
                Assert.Contains("private readonly Guid _backingId;", treeText);

                // Check that the typeconverter gets added.
                Assert.Contains("[TypeConverter(typeof(ValueItemTypeConverter))]", treeText);
            });

            result.AssertGeneratedFile("T.ValueItem.TypeConverter.cs");
        }

        private static Compilation CreateCompilation(string source)
            => CSharpCompilation.Create("compilation",
                new[] { CSharpSyntaxTree.ParseText(source) },
                new[] {
                    MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location),
                },
                new CSharpCompilationOptions(OutputKind.ConsoleApplication));
    }
}
