using System.Globalization;
using Enclave.FastPacket.Generator.ValueProviders;
using Microsoft.CodeAnalysis;

namespace Enclave.FastPacket.Generator.SizeProviders
{
    internal class ExplicitSizeProvider : ISizeProvider
    {
        private readonly int _explicitSize;

        public ExplicitSizeProvider(int explicitSize)
        {
            _explicitSize = explicitSize;
        }

        public string GetSizeExpression(string spanName, string positionExpression)
            => _explicitSize.ToString(CultureInfo.InvariantCulture);
    }
}
