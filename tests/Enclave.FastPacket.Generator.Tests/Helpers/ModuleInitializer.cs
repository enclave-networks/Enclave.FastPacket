using Microsoft.CodeAnalysis;
using System.Runtime.CompilerServices;
using VerifyTests;

namespace Enclave.FastPacket.Generator.Tests.Helpers;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        // Override the output converter so we can filter out files we're not interested in.
        VerifierSettings.RegisterFileConverter<GeneratorDriverRunResult>(ConvertRunResult);
        VerifierSettings.RegisterFileConverter<GeneratorDriver>(ConvertDriver);

        VerifySourceGenerators.Enable();
    }

    static ConversionResult ConvertRunResult(GeneratorDriverRunResult target, IReadOnlyDictionary<string, object> context)
    {
        var exceptions = new List<Exception>();
        var targets = new List<Target>();
        foreach (var result in target.Results)
        {
            if (result.Exception != null)
            {
                exceptions.Add(result.Exception);
            }

            targets.AddRange(result.GeneratedSources.Where(x => x.HintName.EndsWith("_Generated.cs")).Select(SourceToTarget));
        }

        if (exceptions.Count == 1)
        {
            throw exceptions.First();
        }

        if (exceptions.Count > 1)
        {
            throw new AggregateException(exceptions);
        }

        if (target.Diagnostics.Any())
        {
            var info = new
            {
                target.Diagnostics
            };
            return new(info, targets);
        }

        return new(null, targets);
    }

    static ConversionResult ConvertDriver(GeneratorDriver target, IReadOnlyDictionary<string, object> context)
    {
        return ConvertRunResult(target.GetRunResult(), context);
    }

    static Target SourceToTarget(GeneratedSourceResult source)
    {
        var data = $@"//HintName: {source.HintName}
{source.SourceText}";
        return new("cs", data);
    }
}
