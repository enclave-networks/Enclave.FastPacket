using Microsoft.CodeAnalysis;

namespace Enclave.FastPacket.Generator
{
    internal class CastingValueProvider : IValueProvider
    {
        private readonly IValueProvider _wrappedValueProvider;

        public CastingValueProvider(INamedTypeSymbol castingTo, IValueProvider wrappedValueProvider)
        {
            TypeSymbol = castingTo;
            TypeReferenceName = castingTo.GetFullyQualifiedReference();
            _wrappedValueProvider = wrappedValueProvider;
        }

        public INamedTypeSymbol TypeSymbol { get; set; }

        public bool CanSet => _wrappedValueProvider.CanSet;

        public string TypeReferenceName { get; set; }

        public string GetPropGetExpression(string spanName, string positionExpression)
        {
            return $"({TypeReferenceName})({_wrappedValueProvider.GetPropGetExpression(spanName, positionExpression)})";
        }

        public string GetPropSetExpression(string spanName, string positionExpression, string valueExpression)
        {
            return _wrappedValueProvider.GetPropSetExpression(spanName, positionExpression, $"({_wrappedValueProvider.TypeReferenceName})({valueExpression})");
        }
    }
}
