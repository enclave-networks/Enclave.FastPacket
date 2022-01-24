using Microsoft.CodeAnalysis;

namespace Enclave.FastPacket.Generator
{
    public struct PacketFieldOptions
    {
        public int? Size { get; set; }

        public int? Position { get; set; }

        public string? PositionFunction { get; set; }

        public INamedTypeSymbol? EnumBackingType { get; set; }

        public ulong? Bitmask { get; set; }

        public string? SizeFunction { get; set; }
    }
}
