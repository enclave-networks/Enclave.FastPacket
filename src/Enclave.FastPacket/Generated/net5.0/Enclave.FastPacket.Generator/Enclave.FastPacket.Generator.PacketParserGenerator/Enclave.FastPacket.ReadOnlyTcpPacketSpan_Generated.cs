﻿// <auto-generated />

using System;
using System.Buffers.Binary;

namespace Enclave.FastPacket
{
    readonly ref partial struct ReadOnlyTcpPacketSpan
    {
        private readonly ReadOnlySpan<byte> _span;

        /// <summary>
        /// Defines the minimum possible size of this packet, given all
        /// known fixed sizes.
        /// </summary>
        public const int MinimumSize = sizeof(ushort) + sizeof(ushort) + sizeof(uint) + sizeof(uint) + 2 + sizeof(ushort) + sizeof(ushort) + sizeof(ushort);

        /// <summary>
        /// Create a new instance of <see cref="ReadOnlyTcpPacketSpan"/>.
        /// </summary>
        public ReadOnlyTcpPacketSpan(ReadOnlySpan<byte> packetData)
        {
            _span = packetData;
        }

        /// <summary>
        /// Gets the raw underlying buffer for this packet.
        /// </summary>
        public ReadOnlySpan<byte> GetRawData() => _span;
        
        
        /// <summary>
        /// The source port number.
        /// </summary>
        public ushort SourcePort
        {
           get => BinaryPrimitives.ReadUInt16BigEndian(_span.Slice(0));
        }
        
        
        /// <summary>
        /// The destination port number.
        /// </summary>
        public ushort DestinationPort
        {
           get => BinaryPrimitives.ReadUInt16BigEndian(_span.Slice(0 + sizeof(ushort)));
        }
        
        
        /// <summary>
        /// The sequence number.
        /// </summary>
        public uint SequenceNumber
        {
           get => BinaryPrimitives.ReadUInt32BigEndian(_span.Slice(0 + sizeof(ushort) + sizeof(ushort)));
        }
        
        
        /// <summary>
        /// The next sequence number the sender is expecting.
        /// </summary>
        public uint AckNumber
        {
           get => BinaryPrimitives.ReadUInt32BigEndian(_span.Slice(0 + sizeof(ushort) + sizeof(ushort) + sizeof(uint)));
        }
        
        
        /// <summary>
        /// The size of the TCP header in 32-bit words. (min 5).
        /// </summary>
        public byte DataOffset
        {
           get => (byte)((_span[0 + sizeof(ushort) + sizeof(ushort) + sizeof(uint) + sizeof(uint)] & 0xF0u) >> 4);
        }
        
        
        /// <summary>
        /// The TCP flags.
        /// </summary>
        public Enclave.FastPacket.TcpFlags Flags
        {
           get => (Enclave.FastPacket.TcpFlags)((ushort)(BinaryPrimitives.ReadUInt16BigEndian(_span.Slice(0 + sizeof(ushort) + sizeof(ushort) + sizeof(uint) + sizeof(uint))) & 0xFFFu));
        }
        
        
        /// <summary>
        /// The size of the receive window.
        /// </summary>
        public ushort WindowSize
        {
           get => BinaryPrimitives.ReadUInt16BigEndian(_span.Slice(0 + sizeof(ushort) + sizeof(ushort) + sizeof(uint) + sizeof(uint) + 2));
        }
        
        
        /// <summary>
        /// The TCP header checksum.
        /// </summary>
        public ushort Checksum
        {
           get => BinaryPrimitives.ReadUInt16BigEndian(_span.Slice(0 + sizeof(ushort) + sizeof(ushort) + sizeof(uint) + sizeof(uint) + 2 + sizeof(ushort)));
        }
        
        
        /// <summary>
        /// If the urgent flag is set, this is an offset from the sequence number indicating the last urgent byte.
        /// </summary>
        public ushort UrgentPointer
        {
           get => BinaryPrimitives.ReadUInt16BigEndian(_span.Slice(0 + sizeof(ushort) + sizeof(ushort) + sizeof(uint) + sizeof(uint) + 2 + sizeof(ushort) + sizeof(ushort)));
        }
        
        
        /// <summary>
        /// The options block.
        /// </summary>
        public System.ReadOnlySpan<byte> Options
        {
           get => _span.Slice(0 + sizeof(ushort) + sizeof(ushort) + sizeof(uint) + sizeof(uint) + 2 + sizeof(ushort) + sizeof(ushort) + sizeof(ushort), Enclave.FastPacket.TcpPacketDefinition.GetOptionSize(_span));
        }
        
        
        /// <summary>
        /// The payload.
        /// </summary>
        public System.ReadOnlySpan<byte> Payload
        {
           get => _span.Slice(0 + sizeof(ushort) + sizeof(ushort) + sizeof(uint) + sizeof(uint) + 2 + sizeof(ushort) + sizeof(ushort) + sizeof(ushort) + Enclave.FastPacket.TcpPacketDefinition.GetOptionSize(_span));
        }
        
        public override string ToString()
        {
            return $"SourcePort: {SourcePort}; DestinationPort: {DestinationPort}; SequenceNumber: {SequenceNumber}; AckNumber: {AckNumber}; DataOffset: {DataOffset}; Flags: {Flags}; WindowSize: {WindowSize}; Checksum: {Checksum}; UrgentPointer: {UrgentPointer}; Options: {Options.Length} bytes; Payload: {Payload.Length} bytes";
        }

        public int GetTotalSize()
        {
            return 0 + sizeof(ushort) + sizeof(ushort) + sizeof(uint) + sizeof(uint) + 2 + sizeof(ushort) + sizeof(ushort) + sizeof(ushort) + Enclave.FastPacket.TcpPacketDefinition.GetOptionSize(_span) + _span.Slice(0 + sizeof(ushort) + sizeof(ushort) + sizeof(uint) + sizeof(uint) + 2 + sizeof(ushort) + sizeof(ushort) + sizeof(ushort) + Enclave.FastPacket.TcpPacketDefinition.GetOptionSize(_span)).Length;
        }
    }
}
