using Enclave.FastPacket.Generator.ValueProviders;
using Microsoft.CodeAnalysis;

namespace Enclave.FastPacket.Generator.SizeProviders
{
    internal class SizeOfSizeProvider : ISizeProvider
    {
        public SizeOfSizeProvider(INamedTypeSymbol typeSymbol)
        {
            TypeReferenceName = typeSymbol.GetFullyQualifiedReference();
            TypeSymbol = typeSymbol;
        }

        public INamedTypeSymbol TypeSymbol { get; }

        public string TypeReferenceName { get; }

        public string GetSizeExpression(string spanName, string positionExpression)
        {
            return $"sizeof({TypeReferenceName})";
        }
    }
}
