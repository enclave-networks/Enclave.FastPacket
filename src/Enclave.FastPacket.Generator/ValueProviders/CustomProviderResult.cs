using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Enclave.FastPacket.Generator
{

    internal class CustomProviderResult
    {
        public CustomProviderResult(IValueProvider? valueProvider, ISizeProvider? sizeProvider, ImmutableList<DiagnosticDescriptor> diagnostics)
        {
            Value = valueProvider;
            Size = sizeProvider;
            Diagnostics = diagnostics;
        }

        public IValueProvider? Value { get; }

        public ISizeProvider? Size { get; }

        public ImmutableList<DiagnosticDescriptor> Diagnostics { get; }
    }
}
