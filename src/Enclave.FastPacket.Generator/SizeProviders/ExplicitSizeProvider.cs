using System.Globalization;
using Microsoft.CodeAnalysis;

namespace Enclave.FastPacket.Generator.SizeProviders;

internal class ExplicitSizeProvider : IConstantSizeProvider
{
    private readonly int _explicitSize;

    public ExplicitSizeProvider(int explicitSize)
    {
        _explicitSize = explicitSize;
    }

    public string GetConstantSizeExpression()
        => _explicitSize.ToString(CultureInfo.InvariantCulture);

    public string GetSizeExpression(string spanName, string positionExpression)
        => GetConstantSizeExpression();
}
