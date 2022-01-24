using System.Collections.Generic;
using System.Collections.Immutable;

namespace Enclave.FastPacket.Generator
{
    internal class PacketParserDefinition
    {
        public PacketParserDefinition(IList<IPacketProperty> propertySet)
        {
            PropertySet = propertySet;
        }

        public IList<IPacketProperty> PropertySet { get; }
    }
}
