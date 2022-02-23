using System;
using System.Globalization;
using System.Text;

namespace Enclave.FastPacket;

/// <summary>
/// A general exception type thrown by the FastPacket library.
/// </summary>
public class FastPacketException : Exception
{
    /// <summary>
    /// Create a new instance of <see cref="FastPacketException"/>.
    /// </summary>
    /// <param name="message">A message.</param>
    /// <param name="span">The buffer span that caused the problem.</param>
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

        var displaySpan = span.Slice(0, Math.Min(4, span.Length));

        string firstBytes;

#if NET5_0_OR_GREATER
        firstBytes = Convert.ToHexString(displaySpan);
#else
        var strBuilder = new StringBuilder();

        foreach (var b in displaySpan)
        {
            strBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0:X2}", b);
        }

        firstBytes = strBuilder.ToString();
#endif

        return $"{message} ({span.Length} bytes remaining - {firstBytes}...)";
    }
}
