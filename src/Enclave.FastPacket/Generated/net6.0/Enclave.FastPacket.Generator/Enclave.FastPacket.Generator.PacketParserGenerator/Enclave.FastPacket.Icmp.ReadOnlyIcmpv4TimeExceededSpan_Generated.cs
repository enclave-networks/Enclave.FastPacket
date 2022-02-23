﻿// <auto-generated />

using System;
using System.Buffers.Binary;

namespace Enclave.FastPacket.Icmp
{
    readonly ref partial struct ReadOnlyIcmpv4TimeExceededSpan
    {
        private readonly ReadOnlySpan<byte> _span;

        /// <summary>
        /// Defines the minimum possible size of this packet, given all
        /// known fixed sizes.
        /// </summary>
        public const int MinimumSize = sizeof(byte) + sizeof(byte) + sizeof(ushort) + sizeof(uint);

        /// <summary>
        /// Create a new instance of <see cref="ReadOnlyIcmpv4TimeExceededSpan"/>.
        /// </summary>
        public ReadOnlyIcmpv4TimeExceededSpan(ReadOnlySpan<byte> packetData)
        {
            _span = packetData;
        }

        /// <summary>
        /// Gets the raw underlying buffer for this packet.
        /// </summary>
        public ReadOnlySpan<byte> GetRawData() => _span;

        
        
        public Enclave.FastPacket.Icmpv4Types Type
        {
           get => (Enclave.FastPacket.Icmpv4Types)(_span[0]);
        }
        
        
        public Enclave.FastPacket.Icmp.Icmpv4TimeExceededCodes Code
        {
           get => (Enclave.FastPacket.Icmp.Icmpv4TimeExceededCodes)(_span[0 + sizeof(byte)]);
        }
        
        
        public ushort Checksum
        {
           get => BinaryPrimitives.ReadUInt16BigEndian(_span.Slice(0 + sizeof(byte) + sizeof(byte)));
        }
        
        
        private uint Unused
        {
           get => BinaryPrimitives.ReadUInt32BigEndian(_span.Slice(0 + sizeof(byte) + sizeof(byte) + sizeof(ushort)));
        }
        
        
        public System.ReadOnlySpan<byte> IpHeaderAndDatagram
        {
           get => _span.Slice(0 + sizeof(byte) + sizeof(byte) + sizeof(ushort) + sizeof(uint));
        }
        
    }
}