using Microsoft.CodeAnalysis;

namespace Enclave.FastPacket.Generator.ValueProviders
{
    internal interface IValueProvider
    {
        INamedTypeSymbol TypeSymbol { get; }

        bool CanSet { get; }

        string TypeReferenceName { get; }

        string GetPropGetExpression(string spanName, string positionExpression);

        string GetPropSetExpression(string spanName, string positionExpression, string valueExpression);
    }
}
