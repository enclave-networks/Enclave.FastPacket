using System.Collections.Generic;

namespace Enclave.FastPacket.Generator;

internal class PacketParserDefinition
{
    public PacketParserDefinition(IList<IPacketField> propertySet, string minSizeExpression)
    {
        PropertySet = propertySet;
        MinSizeExpression = minSizeExpression;
    }

    public IList<IPacketField> PropertySet { get; }

    public string MinSizeExpression { get; }
}
