namespace Enclave.FastPacket.Generator.SizeProviders;

internal class SpanRemainingLengthSizeProvider : ISizeProvider
{
    public static ISizeProvider Instance { get; } = new SpanRemainingLengthSizeProvider();

    public string GetSizeExpression(string spanName, string positionExpression)
        => $"{spanName}.Slice({positionExpression}).Length";
}
