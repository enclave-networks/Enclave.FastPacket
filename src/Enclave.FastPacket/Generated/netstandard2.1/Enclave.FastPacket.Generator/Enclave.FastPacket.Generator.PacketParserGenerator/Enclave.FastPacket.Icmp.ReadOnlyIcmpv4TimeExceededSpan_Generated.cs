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
        
        
        /// <summary>
        /// Indicates the type of the ICMP packet.
        /// </summary>
        public Enclave.FastPacket.Icmpv4Types Type
        {
           get => (Enclave.FastPacket.Icmpv4Types)(_span[0]);
        }
        
        
        /// <summary>
        /// The time-exceeded code.
        /// </summary>
        public Enclave.FastPacket.Icmp.Icmpv4TimeExceededCodes Code
        {
           get => (Enclave.FastPacket.Icmp.Icmpv4TimeExceededCodes)(_span[0 + sizeof(byte)]);
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
        
        /// <summary>
        /// Get a string representation of this packet.
        /// </summary>
        public override string ToString()
        {
            return $"Type: {Type}; Code: {Code}; Checksum: {Checksum}; ; IpHeaderAndDatagram: {IpHeaderAndDatagram.Length} bytes";
        }

        /// <summary>
        /// Get the computed total size of this packet, including any dynamically-sized fields and trailing payloads.
        /// </summary>
        public int GetTotalSize()
        {
            return 0 + sizeof(byte) + sizeof(byte) + sizeof(ushort) + sizeof(uint) + _span.Slice(0 + sizeof(byte) + sizeof(byte) + sizeof(ushort) + sizeof(uint)).Length;
        }
    }
}
