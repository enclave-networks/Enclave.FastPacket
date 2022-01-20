namespace Enclave.FastPacket.Generator
{
    internal interface IPositionProvider
    {
        /// <summary>
        /// Get an expression that results in the position of a field in a packet.
        /// </summary>
        string GetPositionExpression(string spanName);
    }
}
