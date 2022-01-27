using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Data;
using System.Reflection;

namespace Enclave.FastPacket.Generator.Tests.Helpers;

internal static class CompilationVerifier
{
    public static Task Verify(string source)
    {
        var compilation = Create(source);

        var generator = new PacketParserGenerator();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        // Once through for the generated code diagnostics, where we update the compilation.
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var newCompilation, out var newDiagnostics);

        return Verifier.Verify(driver)
                       .UseDirectory(Path.Combine(AttributeReader.GetProjectDirectory(), "Snapshots"));
    }

    private static Compilation Create(string source)
        => CSharpCompilation.Create("compilation",
            new[] { CSharpSyntaxTree.ParseText(source) },
            new[] {
                    MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location),
            },
            new CSharpCompilationOptions(OutputKind.ConsoleApplication));
}
