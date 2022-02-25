using Microsoft.CodeAnalysis;

namespace Enclave.FastPacket.Generator.ValueProviders;

internal class BoolFromNumberValueProvider : IValueProvider
{
    private readonly IValueProvider _wrappedValueProvider;

    public BoolFromNumberValueProvider(INamedTypeSymbol boolType, IValueProvider wrappedValueProvider)
    {
        TypeSymbol = boolType;
        TypeReferenceName = boolType.GetFullyQualifiedReference();
        _wrappedValueProvider = wrappedValueProvider;
    }

    public INamedTypeSymbol TypeSymbol { get; set; }

    public bool CanSet => _wrappedValueProvider.CanSet;

    public string TypeReferenceName { get; set; }

    public string GetPropGetExpression(string spanName, string positionExpression)
    {
        return $"({_wrappedValueProvider.GetPropGetExpression(spanName, positionExpression)}) > 0";
    }

    public string GetPropSetExpression(string spanName, string positionExpression, string valueExpression)
    {
        return _wrappedValueProvider.GetPropSetExpression(spanName, positionExpression, "(value ? 1 : 0)");
    }
}
