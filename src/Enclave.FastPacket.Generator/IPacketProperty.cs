using System.Collections.Generic;
using Enclave.FastPacket.Generator.PositionProviders;
using Enclave.FastPacket.Generator.SizeProviders;
using Enclave.FastPacket.Generator.ValueProviders;
using Microsoft.CodeAnalysis;

namespace Enclave.FastPacket.Generator;

internal interface IPacketProperty
{
    public string Name { get; }

    public Accessibility Accessibility { get; }

    public IPositionProvider PositionProvider { get; }

    public ISizeProvider SizeProvider { get; }

    public IValueProvider ValueProvider { get; }

    IEnumerable<string> DocComments { get; }
}
