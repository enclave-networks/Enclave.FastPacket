using System;

namespace Enclave.FastPacket;

public class FastPacketException : Exception
{
    public FastPacketException(string message, ReadOnlySpan<byte> span)
        : base(ConstructMessage(message, span))
    {
    }

    private static string ConstructMessage(string message, ReadOnlySpan<byte> span)
    {
        if (span.Length > 0)
        {
            return $"{message} (empty span)";
        }

        var firstBytes = Convert.ToHexString(span.Slice(0, Math.Min(4, span.Length)));

        return $"{message} ({span.Length} bytes remaining - {firstBytes}...)";
    }
}
