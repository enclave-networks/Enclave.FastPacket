using Microsoft.CodeAnalysis;

namespace Enclave.FastPacket.Generator.ValueProviders;

internal class BinaryPrimitivesValueProvider : IValueProvider
{
    public BinaryPrimitivesValueProvider(INamedTypeSymbol typeSymbol)
    {
        TypeReferenceName = typeSymbol.ToDisplayString();
        TypeSymbol = typeSymbol;
    }

    public bool CanSet => true;

    public INamedTypeSymbol TypeSymbol { get; set; }

    public string TypeReferenceName { get; }

    public string GetPropGetExpression(string spanName, string positionExpression)
    {
        return $"BinaryPrimitives.Read{TypeSymbol.Name}BigEndian({spanName}.Slice({positionExpression}))";
    }

    public string GetPropSetExpression(string spanName, string positionExpression, string valueExpression)
    {
        return $"BinaryPrimitives.Write{TypeSymbol.Name}BigEndian({spanName}.Slice({positionExpression}), {valueExpression})";
    }
}
