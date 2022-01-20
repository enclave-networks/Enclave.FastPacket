using System;
using Microsoft.CodeAnalysis;

namespace Enclave.FastPacket.Generator.ValueProviders
{
    internal class SpanRestOfDataValueProvider : IValueProvider
    {
        public SpanRestOfDataValueProvider(INamedTypeSymbol typeSymbol)
        {
            TypeSymbol = typeSymbol;
            TypeReferenceName = typeSymbol.GetFullyQualifiedReference();
        }

        public bool CanSet => false;

        public INamedTypeSymbol TypeSymbol { get; }

        public string TypeReferenceName { get; }

        public string GetPropGetExpression(string spanName, string positionExpression)
        {
            return $"{spanName}.Slice({positionExpression})";
        }

        public string GetPropSetExpression(string spanName, string positionExpression, string valueExpression)
        {
            throw new NotImplementedException("Cannot set on a span");
        }
    }
}
