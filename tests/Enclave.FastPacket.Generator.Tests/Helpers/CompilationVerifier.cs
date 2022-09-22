using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Data;
using System.Reflection;

namespace Enclave.FastPacket.Generator.Tests.Helpers;

public class FluentVerify
{
    private readonly string _source;
    private IReadOnlyList<Action<Diagnostic>> _diagnostics = Array.Empty<Action<Diagnostic>>();

    public static FluentVerify ForSource(string source)
    {
        return new FluentVerify(source);
    }

    private FluentVerify(string source)
    {
        _source = source;
    }

    public FluentVerify WithDiagnostics(params Action<Diagnostic>[] diagnosticAsserts)
    {
        _diagnostics = diagnosticAsserts;

        return this;
    }

    public FluentVerify WithDiagnostics(params DiagnosticDescriptor[] diagnosticDescriptors)
    {
        static Action<Diagnostic> AssertDiag(DiagnosticDescriptor expected)
        {
            return (Diagnostic actual) => Assert.Equal(expected.Id, actual.Id);
        }

        _diagnostics = diagnosticDescriptors.Select(AssertDiag).ToArray();

        return this;
    }

    public Task Verify()
    {
        var compilation = Create(_source);

        var generator = new PacketParserGenerator();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        // Once through for the generated code diagnostics, where we update the compilation.
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var newCompilation, out var newDiagnostics);

        Assert.Collection(newDiagnostics, _diagnostics.ToArray());

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
