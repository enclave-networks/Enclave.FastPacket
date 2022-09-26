using System;
using System.Collections.Generic;
using Enclave.FastPacket.Generator.PositionProviders;
using Enclave.FastPacket.Generator.SizeProviders;
using Enclave.FastPacket.Generator.ValueProviders;
using Microsoft.CodeAnalysis;

namespace Enclave.FastPacket.Generator;

internal class VirtualUnionField : IPacketField
{
    public VirtualUnionField(
        string name,
        IPositionProvider positionProvider,
        ISizeProvider sizeProvider,
        IEnumerable<string> docComments)
    {
        Name = name;
        PositionProvider = positionProvider;
        SizeProvider = sizeProvider;
        DocComments = docComments;
    }

    public string Name { get; }

    public IPositionProvider PositionProvider { get; }

    public ISizeProvider SizeProvider { get; }

    public IValueProvider ValueProvider => throw new InvalidOperationException("Unions cannot have their own value providers");

    public IEnumerable<string> DocComments { get; }

    public Accessibility Accessibility => throw new InvalidOperationException("Cannot directly access the accessibility of a union");

    public IPropertySymbol DeclaredProperty => throw new NotImplementedException();

    public Location DiagnosticsLocation => throw new NotImplementedException();

    public PacketFieldOptions Options => throw new NotImplementedException();
}
