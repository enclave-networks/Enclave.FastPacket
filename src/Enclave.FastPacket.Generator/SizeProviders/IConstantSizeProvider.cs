namespace Enclave.FastPacket.Generator.SizeProviders
{
    /// <summary>
    /// Indicates that a provider gives a constant size known at compile time, with no evaluation required.
    /// </summary>
    internal interface IConstantSizeProvider : ISizeProvider
    {
        string GetConstantSizeExpression();
    }
}
