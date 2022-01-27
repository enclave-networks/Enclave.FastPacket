using System.Globalization;

namespace Enclave.FastPacket.Generator.PositionProviders;

internal class ExplicitPositionProvider : IConstantPositionProvider
{
    private readonly int _explicitPosition;

    public ExplicitPositionProvider(int explicitPosition)
    {
        _explicitPosition = explicitPosition;
    }

    public string GetConstantPositionExpression()
    {
        return _explicitPosition.ToString(CultureInfo.InvariantCulture);
    }

    public string GetPositionExpression(string spanName)
    {
        return GetConstantPositionExpression();
    }
}
