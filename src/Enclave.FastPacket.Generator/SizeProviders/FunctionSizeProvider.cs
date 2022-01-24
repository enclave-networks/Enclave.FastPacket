using Microsoft.CodeAnalysis;

namespace Enclave.FastPacket.Generator.SizeProviders
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
            if (Method.Parameters.Length > 1)
            {
                return $"{FullReferenceName}({spanName}, {positionExpression})";
            }
            else
            {
                return $"{FullReferenceName}({spanName})";
            }
        }
    }
}
