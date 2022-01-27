using System;
using Microsoft.CodeAnalysis;

namespace Enclave.FastPacket.Generator.SizeProviders;

internal class CustomTypeConstantSizeProvider : IConstantSizeProvider
{
    public CustomTypeConstantSizeProvider(INamedTypeSymbol typeSymbol)
    {
        TypeReferenceName = typeSymbol.GetFullyQualifiedReference();
    }

    public string TypeReferenceName { get; }

    public string GetConstantSizeExpression()
        => $"{TypeReferenceName}.Size";

    public string GetSizeExpression(string spanName, string positionExpression)
        => GetConstantSizeExpression();
}
