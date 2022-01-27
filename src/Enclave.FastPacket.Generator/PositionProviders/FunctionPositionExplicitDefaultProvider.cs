using Microsoft.CodeAnalysis;

namespace Enclave.FastPacket.Generator.PositionProviders;

internal class FunctionPositionExplicitDefaultProvider : IPositionProvider
{
    private readonly int _explicitPosition;

    public FunctionPositionExplicitDefaultProvider(IMethodSymbol positionMethod, int explicitPosition)
    {
        Method = positionMethod;
        _explicitPosition = explicitPosition;
        FullReferenceName = positionMethod.GetFullyQualifiedReference();
    }

    public IMethodSymbol Method { get; }

    public string FullReferenceName { get; }

    public string GetPositionExpression(string spanName)
    {
        // Automatic position calculation is just based on the position expression of the previous property,
        // plus the size (to take us to the start of this field).
        return $"{FullReferenceName}({spanName}, {_explicitPosition})";
    }
}
