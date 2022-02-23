﻿// <auto-generated />

using System;
using System.Buffers.Binary;

namespace Enclave.FastPacket.Icmp
{
    readonly ref partial struct ReadOnlyIcmpv4TimestampSpan
    {
        private readonly ReadOnlySpan<byte> _span;

        /// <summary>
        /// Defines the minimum possible size of this packet, given all
        /// known fixed sizes.
        /// </summary>
        public const int MinimumSize = sizeof(byte) + sizeof(byte) + sizeof(ushort) + sizeof(ushort) + sizeof(ushort) + sizeof(uint) + sizeof(uint) + sizeof(uint);

        /// <summary>
        /// Create a new instance of <see cref="ReadOnlyIcmpv4TimestampSpan"/>.
        /// </summary>
        public ReadOnlyIcmpv4TimestampSpan(ReadOnlySpan<byte> packetData)
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
        
        
        /// <summary>
        /// The identifier.
        /// </summary>
        public ushort Identifier
        {
           get => BinaryPrimitives.ReadUInt16BigEndian(_span.Slice(0 + sizeof(byte) + sizeof(byte) + sizeof(ushort)));
        }
        
        
        /// <summary>
        /// The sequence number.
        /// </summary>
        public ushort SequenceNumber
        {
           get => BinaryPrimitives.ReadUInt16BigEndian(_span.Slice(0 + sizeof(byte) + sizeof(byte) + sizeof(ushort) + sizeof(ushort)));
        }
        
        
        /// <summary>
        /// The originating timestamp.
        /// </summary>
        public uint OrginateTimeStamp
        {
           get => BinaryPrimitives.ReadUInt32BigEndian(_span.Slice(0 + sizeof(byte) + sizeof(byte) + sizeof(ushort) + sizeof(ushort) + sizeof(ushort)));
        }
        
        
        /// <summary>
        /// The receive timestamp.
        /// </summary>
        public uint ReceiveTimeStamp
        {
           get => BinaryPrimitives.ReadUInt32BigEndian(_span.Slice(0 + sizeof(byte) + sizeof(byte) + sizeof(ushort) + sizeof(ushort) + sizeof(ushort) + sizeof(uint)));
        }
        
        
        /// <summary>
        /// The transmit timestamp.
        /// </summary>
        public uint TransmitTimeStamp
        {
           get => BinaryPrimitives.ReadUInt32BigEndian(_span.Slice(0 + sizeof(byte) + sizeof(byte) + sizeof(ushort) + sizeof(ushort) + sizeof(ushort) + sizeof(uint) + sizeof(uint)));
        }
        
    }
}