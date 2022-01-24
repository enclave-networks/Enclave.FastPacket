using Microsoft.CodeAnalysis;

namespace Enclave.FastPacket.Generator.SizeProviders
{
    internal class SizeOfSizeProvider : IConstantSizeProvider
    {
        public SizeOfSizeProvider(INamedTypeSymbol typeSymbol)
        {
            TypeReferenceName = typeSymbol.GetFullyQualifiedReference();
            TypeSymbol = typeSymbol;
        }

        public INamedTypeSymbol TypeSymbol { get; }

        public string TypeReferenceName { get; }

        public string GetConstantSizeExpression() => $"sizeof({TypeReferenceName})";

        public string GetSizeExpression(string spanName, string positionExpression) => GetConstantSizeExpression();
    }
}
