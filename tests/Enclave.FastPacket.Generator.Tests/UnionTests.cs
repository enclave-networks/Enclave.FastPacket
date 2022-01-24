using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;
using Xunit;

namespace Enclave.FastPacket.Generator.Tests
{
    public class UnionTests
    {
        [Fact]
        public void CanGenerateUnion()
        {
            var inputCompilation = CreateCompilation(@"

using Enclave.FastPacket.Generator;

namespace T
{
    internal class PacketDefinition
    {
        ushort Value { get; set; }

        [PacketField(Size = sizeof(byte))]
        struct Union1
        {
            public byte UnionVal1 { get; set; }

            public byte UnionVal2 { get; set; }
        }

        ushort Value2 { get; set; }
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
        public void CanGenerateUnionWithBitmask()
        {
            var inputCompilation = CreateCompilation(@"

using Enclave.FastPacket.Generator;

namespace T
{
    internal class PacketDefinition
    {
        ushort Value { get; set; }

        [PacketField(Size = sizeof(byte))]
        struct Union1
        {
            [PacketFieldBits(0, 3)]
            public byte UnionVal1 { get; set; }

            [PacketFieldBits(4, 7)]
            public byte UnionVal2 { get; set; }
        }

        ushort Value2 { get; set; }
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
        public void CanGenerateUnionWithLargeBitmask()
        {
            var inputCompilation = CreateCompilation(@"

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
        ushort Value { get; set; }

        [PacketField(Size = sizeof(byte))]
        private struct U1
        {
            [PacketFieldBits(0, 3)]
            public byte Version { get; set; }

            [PacketFieldBits(4, 7)]
            public byte IHL { get; set; }
        }

        [PacketField(Size = sizeof(ushort))]
        private struct U3
        {
            [PacketFieldBits(0, 2)]
            public FragmentFlags Flags { get; set; }

            /// <summary>
            /// Can specify the bitmask by inverting the other one.
            /// </summary>
            [PacketFieldBits(3, 15)]
            public ushort FragmentValue { get; set; }
        }

        ushort Value2 { get; set; }
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


        private static Compilation CreateCompilation(string source)
            => CSharpCompilation.Create("compilation",
                new[] { CSharpSyntaxTree.ParseText(source) },
                new[] {
                    MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location),
                },
                new CSharpCompilationOptions(OutputKind.ConsoleApplication));
    }
}
