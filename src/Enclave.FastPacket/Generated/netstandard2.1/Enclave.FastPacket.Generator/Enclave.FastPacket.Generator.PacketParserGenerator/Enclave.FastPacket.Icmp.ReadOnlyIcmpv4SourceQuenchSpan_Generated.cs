﻿// <auto-generated />

using System;
using System.Buffers.Binary;

namespace Enclave.FastPacket.Icmp
{
    readonly ref partial struct ReadOnlyIcmpv4SourceQuenchSpan
    {
        private readonly ReadOnlySpan<byte> _span;

        /// <summary>
        /// Defines the minimum possible size of this packet, given all
        /// known fixed sizes.
        /// </summary>
        public const int MinimumSize = sizeof(byte) + sizeof(byte) + sizeof(ushort) + sizeof(uint);

        /// <summary>
        /// Create a new instance of <see cref="ReadOnlyIcmpv4SourceQuenchSpan"/>.
        /// </summary>
        public ReadOnlyIcmpv4SourceQuenchSpan(ReadOnlySpan<byte> packetData)
        {
            _span = packetData;
        }

        /// <summary>
        /// Gets the raw underlying buffer for this packet.
        /// </summary>
        public ReadOnlySpan<byte> GetRawData() => _span;

        
        
        /// <summary>
        /// Indicates the type of the ICMP packet.
        /// </summary>
        public Enclave.FastPacket.Icmpv4Types Type
        {
           get => (Enclave.FastPacket.Icmpv4Types)(_span[0]);
        }
        
        
        /// <summary>
        /// The code.
        /// </summary>
        public byte Code
        {
           get => _span[0 + sizeof(byte)];
        }
        
        
        /// <summary>
        /// The checksum.
        /// </summary>
        public ushort Checksum
        {
           get => BinaryPrimitives.ReadUInt16BigEndian(_span.Slice(0 + sizeof(byte) + sizeof(byte)));
        }
        
        
        private uint Unused
        {
           get => BinaryPrimitives.ReadUInt32BigEndian(_span.Slice(0 + sizeof(byte) + sizeof(byte) + sizeof(ushort)));
        }
        
        
        /// <summary>
        /// The failed IP header and datagram.
        /// </summary>
        public System.ReadOnlySpan<byte> IpHeaderAndDatagram
        {
           get => _span.Slice(0 + sizeof(byte) + sizeof(byte) + sizeof(ushort) + sizeof(uint));
        }
        
    }
}