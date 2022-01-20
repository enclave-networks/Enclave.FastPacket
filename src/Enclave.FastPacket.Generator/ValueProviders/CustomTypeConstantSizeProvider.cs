using System;
using Microsoft.CodeAnalysis;

namespace Enclave.FastPacket.Generator
{
    internal class CustomTypeConstantSizeProvider : ISizeProvider
    {
        public CustomTypeConstantSizeProvider(INamedTypeSymbol typeSymbol)
        {
            TypeReferenceName = typeSymbol.GetFullyQualifiedReference();
        }

        public string TypeReferenceName { get; }

        public string GetSizeExpression(string spanName, string positionExpression)
            => $"{TypeReferenceName}.Size";
    }
}
