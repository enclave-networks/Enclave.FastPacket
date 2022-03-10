using Enclave.FastPacket.Generator;

namespace Enclave.FastPacket;

internal ref struct Ipv6FragmentExtension
{
    /// <summary>
    /// The next header after this extension.
    /// </summary>
    [PacketField(EnumBackingType = typeof(byte))]
    public IpProtocol NextHeader { get; set; }

    private byte Reserved { get; set; }

    [PacketField(Size = sizeof(ushort))]
    public struct OffsetUnion
    {
        /// <summary>
        /// The offset position of this fragment.
        /// </summary>
        [PacketFieldBits(0, 12)]
        public ushort FragmentOffset { get; set; }

        /// <summary>
        /// A flag indicating whether any more fragments are available.
        /// </summary>
        [PacketFieldBits(15)]
        public bool MoreFragments { get; set; }
    }

    /// <summary>
    /// The fragment identification field.
    /// </summary>
    public uint Identification { get; set; }
}

/// <summary>
/// A read-write decoder for an IPv6 fragment extension header.
/// </summary>
[PacketImplementation(typeof(Ipv6FragmentExtension), IsReadOnly = true)]
public readonly ref partial struct ReadOnlyIpv6FragmentExtensionSpan
{
}
