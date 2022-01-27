using System;
using Microsoft.CodeAnalysis;

namespace Enclave.FastPacket.Generator.SizeProviders;

internal class CustomTypeMethodSizeProvider : ISizeProvider
{
    public CustomTypeMethodSizeProvider(INamedTypeSymbol typeSymbol)
    {
        TypeReferenceName = typeSymbol.GetFullyQualifiedReference();
    }

    public string TypeReferenceName { get; }

    public string GetSizeExpression(string spanName, string positionExpression)
        => $"{TypeReferenceName}.GetSize({spanName}.Slice({positionExpression}))";
}
