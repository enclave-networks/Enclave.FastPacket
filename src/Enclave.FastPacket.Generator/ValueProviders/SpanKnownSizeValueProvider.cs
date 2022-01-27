using System;
using Enclave.FastPacket.Generator.SizeProviders;
using Microsoft.CodeAnalysis;

namespace Enclave.FastPacket.Generator.ValueProviders;

internal class SpanKnownSizeValueProvider : IValueProvider
{
    private readonly ISizeProvider _sizeProvider;

    public SpanKnownSizeValueProvider(INamedTypeSymbol typeSymbol, ISizeProvider sizeProvider)
    {
        TypeSymbol = typeSymbol;
        _sizeProvider = sizeProvider;
        TypeReferenceName = typeSymbol.GetFullyQualifiedReference();
    }

    public bool CanSet => false;

    public INamedTypeSymbol TypeSymbol { get; }

    public string TypeReferenceName { get; }

    public string GetPropGetExpression(string spanName, string positionExpression)
    {
        return $"{spanName}.Slice({positionExpression}, {_sizeProvider.GetSizeExpression(spanName, positionExpression)})";
    }

    public string GetPropSetExpression(string spanName, string positionExpression, string valueExpression)
    {
        throw new NotImplementedException("Cannot set on a span");
    }
}
