using System.Collections.Immutable;

namespace Enclave.FastPacket.Generator
{
    internal class PacketParserDefinition
    {
        public PacketParserDefinition(ImmutableList<IPacketProperty> propertySet)
        {
            PropertySet = propertySet;
        }

        public ImmutableList<IPacketProperty> PropertySet { get; }
    }
}
