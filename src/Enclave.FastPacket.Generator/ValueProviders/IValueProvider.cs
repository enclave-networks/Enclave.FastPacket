using Microsoft.CodeAnalysis;

namespace Enclave.FastPacket.Generator.ValueProviders
{
    internal interface ISizeProvider
    {
        /// <summary>
        /// Get an expression that results in the size of a field in a packet.
        /// </summary>
        string GetSizeExpression(string spanName, string positionExpression);
    }

    internal interface IValueProvider
    {
        INamedTypeSymbol TypeSymbol { get; }

        bool CanSet { get; }

        string TypeReferenceName { get; }

        string GetPropGetExpression(string spanName, string positionExpression);

        string GetPropSetExpression(string spanName, string positionExpression, string valueExpression);
    }
}
