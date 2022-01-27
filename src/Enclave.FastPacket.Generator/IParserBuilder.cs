using Microsoft.CodeAnalysis;

namespace Enclave.FastPacket.Generator;

internal interface IParserBuilder
{
    string Generate(PacketParserDefinition packetDef, GenerationOptions definitionTypeOptions, INamedTypeSymbol structSymbol, PacketPropertyFactory packetFieldFactory);
}
