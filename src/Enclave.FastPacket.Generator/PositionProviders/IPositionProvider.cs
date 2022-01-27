namespace Enclave.FastPacket.Generator.PositionProviders;

/// <summary>
/// Indicates that the provider uses a constant position.
/// </summary>
internal interface IConstantPositionProvider : IPositionProvider
{
    string GetConstantPositionExpression();
}

internal interface IPositionProvider
{
    /// <summary>
    /// Get an expression that results in the position of a field in a packet.
    /// </summary>
    string GetPositionExpression(string spanName);
}
