using System;
using Microsoft.CodeAnalysis;

namespace Enclave.FastPacket.Generator
{
    internal class CustomTypeValueProvider : IValueProvider
    {
        private readonly ISizeProvider _sizeProvider;

        public CustomTypeValueProvider(INamedTypeSymbol typeSymbol, ISizeProvider sizeProvider)
        {
            TypeSymbol = typeSymbol;
            _sizeProvider = sizeProvider;
            TypeReferenceName = typeSymbol.GetFullyQualifiedReference();
        }

        public bool CanSet => true;

        public INamedTypeSymbol TypeSymbol { get; }

        public string TypeReferenceName { get; }

        public string GetPropGetExpression(string spanName, string positionExpression)
        {
            return $"new {TypeReferenceName}({spanName}.Slice({positionExpression}, {_sizeProvider.GetSizeExpression(spanName, positionExpression)}))";
        }

        public string GetPropSetExpression(string spanName, string positionExpression, string valueExpression)
        {
            return $"{valueExpression}.CopyTo({spanName}.Slice({positionExpression}, {_sizeProvider.GetSizeExpression(spanName, positionExpression)}))";
        }
    }
}
