using Enclave.FastPacket.Generator.PositionProviders;
using Enclave.FastPacket.Generator.ValueProviders;
using System;
using System.Collections.Generic;

namespace Enclave.FastPacket.Generator
{
    internal class VirtualUnionProperty : IPacketProperty
    {
        public VirtualUnionProperty(
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

        public IValueProvider ValueProvider => throw new InvalidOperationException("Cannot directly access the value of a union");

        public IEnumerable<string> DocComments { get; }
    }
}
