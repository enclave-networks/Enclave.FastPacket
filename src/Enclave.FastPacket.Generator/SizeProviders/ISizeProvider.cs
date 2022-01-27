namespace Enclave.FastPacket.Generator.SizeProviders;

internal interface ISizeProvider
{
    /// <summary>
    /// Get an expression that results in the size of a field in a packet.
    /// </summary>
    string GetSizeExpression(string spanName, string positionExpression);
}
