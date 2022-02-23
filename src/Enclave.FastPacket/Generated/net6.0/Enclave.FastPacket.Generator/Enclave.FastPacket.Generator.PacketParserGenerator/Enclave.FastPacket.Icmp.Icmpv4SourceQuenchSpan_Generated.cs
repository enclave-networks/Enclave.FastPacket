﻿// <auto-generated />

using System;
using System.Buffers.Binary;

namespace Enclave.FastPacket.Icmp
{
    readonly ref partial struct Icmpv4SourceQuenchSpan
    {
        private readonly Span<byte> _span;

        /// <summary>
        /// Defines the minimum possible size of this packet, given all
        /// known fixed sizes.
        /// </summary>
        public const int MinimumSize = sizeof(byte) + sizeof(byte) + sizeof(ushort) + sizeof(uint);

        /// <summary>
        /// Create a new instance of <see cref="Icmpv4SourceQuenchSpan"/>.
        /// </summary>
        public Icmpv4SourceQuenchSpan(Span<byte> packetData)
        {
            _span = packetData;
        }

        /// <summary>
        /// Gets the raw underlying buffer for this packet.
        /// </summary>
        public Span<byte> GetRawData() => _span;

        
        
        public Enclave.FastPacket.Icmpv4Types Type
        {
           get => (Enclave.FastPacket.Icmpv4Types)(_span[0]);
           set => _span[0] = (byte)(value); 
        }
        
        
        public byte Code
        {
           get => _span[0 + sizeof(byte)];
           set => _span[0 + sizeof(byte)] = value; 
        }
        
        
        public ushort Checksum
        {
           get => BinaryPrimitives.ReadUInt16BigEndian(_span.Slice(0 + sizeof(byte) + sizeof(byte)));
           set => BinaryPrimitives.WriteUInt16BigEndian(_span.Slice(0 + sizeof(byte) + sizeof(byte)), value); 
        }
        
        
        private uint Unused
        {
           get => BinaryPrimitives.ReadUInt32BigEndian(_span.Slice(0 + sizeof(byte) + sizeof(byte) + sizeof(ushort)));
           set => BinaryPrimitives.WriteUInt32BigEndian(_span.Slice(0 + sizeof(byte) + sizeof(byte) + sizeof(ushort)), value); 
        }
        
        
        public System.Span<byte> IpHeaderAndDatagram
        {
           get => _span.Slice(0 + sizeof(byte) + sizeof(byte) + sizeof(ushort) + sizeof(uint));
        }
        
    }
}
