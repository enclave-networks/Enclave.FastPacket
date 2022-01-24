using System;
using System.Globalization;
using Enclave.FastPacket.Generator.ValueProviders;
using Microsoft.CodeAnalysis;

namespace Enclave.FastPacket.Generator.SizeProviders
{
    internal class UnknownSizeProvider : ISizeProvider
    {
        public static ISizeProvider Instance { get; } = new UnknownSizeProvider();

        public string GetSizeExpression(string spanName, string positionExpression)
            => throw new InvalidOperationException("Size is unknown, so cannot use");
    }
}
