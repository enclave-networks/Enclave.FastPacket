using Microsoft.CodeAnalysis;

namespace Enclave.FastPacket.Generator.PositionProviders;

internal class FunctionPositionAutomaticDefaultProvider : AutomaticPositionProvider
{
    public FunctionPositionAutomaticDefaultProvider(IMethodSymbol positionMethod, IPacketField? previousProperty)
        : base(previousProperty)
    {
        Method = positionMethod;
        FullReferenceName = positionMethod.GetFullyQualifiedReference();
    }

    public IMethodSymbol Method { get; }

    public string FullReferenceName { get; }

    public override string GetPositionExpression(string spanName)
    {
        // Automatic position calculation is just based on the position expression of the previous property,
        // plus the size (to take us to the start of this field).
        return $"{FullReferenceName}({spanName}, {base.GetPositionExpression(spanName)})";
    }
}
