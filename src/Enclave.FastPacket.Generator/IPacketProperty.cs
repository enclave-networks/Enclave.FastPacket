using Enclave.FastPacket.Generator.PositionProviders;
using Enclave.FastPacket.Generator.ValueProviders;
using System.Collections.Generic;

namespace Enclave.FastPacket.Generator
{
    internal interface IPacketProperty
    {
        public string Name { get; }

        public IPositionProvider PositionProvider { get; }

        public ISizeProvider SizeProvider { get; }

        public IValueProvider ValueProvider { get; }

        IEnumerable<string> DocComments { get; }
    }
}
