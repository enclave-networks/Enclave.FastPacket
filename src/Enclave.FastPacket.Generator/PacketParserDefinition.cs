using System.Collections.Generic;

namespace Enclave.FastPacket.Generator;

internal class PacketParserDefinition
{
    public PacketParserDefinition(IList<IPacketProperty> propertySet, string minSizeExpression)
    {
        PropertySet = propertySet;
        MinSizeExpression = minSizeExpression;
    }

    public IList<IPacketProperty> PropertySet { get; }

    public string MinSizeExpression { get; }
}
