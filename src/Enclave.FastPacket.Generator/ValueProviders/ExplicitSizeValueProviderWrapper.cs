using System.Globalization;
using Microsoft.CodeAnalysis;

namespace Enclave.FastPacket.Generator
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
