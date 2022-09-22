using System.Collections.Generic;
using Enclave.FastPacket.Generator.PositionProviders;
using Enclave.FastPacket.Generator.SizeProviders;
using Enclave.FastPacket.Generator.ValueProviders;
using Microsoft.CodeAnalysis;

namespace Enclave.FastPacket.Generator;

internal class PacketField : IPacketField
{
    public PacketField(
        string name,
        Accessibility accessibility,
        Location diagnosticsLocation,
        PacketFieldOptions options,
        IPositionProvider positionProvider,
        ISizeProvider sizeProvider,
        IValueProvider valueProvider,
        IEnumerable<string> docComments)
    {
        Name = name;
        Accessibility = accessibility;
        DiagnosticsLocation = diagnosticsLocation;
        Options = options;
        PositionProvider = positionProvider;
        SizeProvider = sizeProvider;
        ValueProvider = valueProvider;
        DocComments = docComments;
    }

    public string Name { get; }

    public Accessibility Accessibility { get; }

    public Location DiagnosticsLocation { get; }

    public PacketFieldOptions Options { get; }

    public IPositionProvider PositionProvider { get; set; }

    public ISizeProvider SizeProvider { get; set; }

    public IValueProvider ValueProvider { get; set; }

    public IEnumerable<string> DocComments { get; }
}
