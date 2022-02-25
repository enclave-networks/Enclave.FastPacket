using Microsoft.CodeAnalysis;

namespace Enclave.FastPacket.Generator.ValueProviders;

internal class SingleByteValueProvider : IValueProvider
{
    public SingleByteValueProvider(INamedTypeSymbol typeSymbol)
    {
        TypeSymbol = typeSymbol;
    }

    public bool CanSet => true;

    public INamedTypeSymbol TypeSymbol { get; set; }

    public string TypeReferenceName => "byte";

    public string GetPropGetExpression(string spanName, string positionExpression)
    {
        return $"{spanName}[{positionExpression}]";
    }

    public string GetPropSetExpression(string spanName, string positionExpression, string valueExpression)
    {
        return $"{spanName}[{positionExpression}] = {valueExpression}";
    }
}
