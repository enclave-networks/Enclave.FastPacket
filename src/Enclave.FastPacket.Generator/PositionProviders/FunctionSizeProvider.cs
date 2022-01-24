using Enclave.FastPacket.Generator.ValueProviders;
using Microsoft.CodeAnalysis;

namespace Enclave.FastPacket.Generator.PositionProviders
{
    internal class FunctionSizeProvider : ISizeProvider
    {
        public FunctionSizeProvider(IMethodSymbol positionMethod)
        {
            Method = positionMethod;
            FullReferenceName = positionMethod.GetFullyQualifiedReference();
        }

        public IMethodSymbol Method { get; }

        public string FullReferenceName { get; }

        public string GetSizeExpression(string spanName, string positionExpression)
        {
            return $"{FullReferenceName}({spanName}, {positionExpression})";
        }
    }
}
