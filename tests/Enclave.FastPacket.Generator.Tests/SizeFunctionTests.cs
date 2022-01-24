using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;
using Xunit;

namespace Enclave.FastPacket.Generator.Tests
{
    public class SizeFunctionTests
    {
        [Fact]
        public void CanHaveOptionalPosition()
        {
            var inputCompilation = CreateCompilation(@"
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
