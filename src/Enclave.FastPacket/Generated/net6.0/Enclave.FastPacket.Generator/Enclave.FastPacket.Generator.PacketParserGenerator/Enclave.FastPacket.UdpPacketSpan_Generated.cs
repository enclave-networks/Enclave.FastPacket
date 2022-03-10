﻿// <auto-generated />

using System;
using System.Buffers.Binary;

namespace Enclave.FastPacket
{
    readonly ref partial struct UdpPacketSpan
    {
        private readonly Span<byte> _span;

        /// <summary>
        /// Defines the minimum possible size of this packet, given all
        /// known fixed sizes.
        /// </summary>
        public const int MinimumSize = sizeof(ushort) + sizeof(ushort) + sizeof(ushort) + sizeof(ushort);

        /// <summary>
        /// Create a new instance of <see cref="UdpPacketSpan"/>.
        /// </summary>
        public UdpPacketSpan(Span<byte> packetData)
        {
            _span = packetData;
        }

        /// <summary>
        /// Gets the raw underlying buffer for this packet.
        /// </summary>
        public Span<byte> GetRawData() => _span;
        
        
        /// <summary>
        /// The source port.
        /// </summary>
        public ushort SourcePort
        {
           get => BinaryPrimitives.ReadUInt16BigEndian(_span.Slice(0));
           set => BinaryPrimitives.WriteUInt16BigEndian(_span.Slice(0), value); 
        }
        
        
        /// <summary>
        /// The destination port.
        /// </summary>
        public ushort DestinationPort
        {
           get => BinaryPrimitives.ReadUInt16BigEndian(_span.Slice(0 + sizeof(ushort)));
           set => BinaryPrimitives.WriteUInt16BigEndian(_span.Slice(0 + sizeof(ushort)), value); 
        }
        
        
        /// <summary>
        /// The length in bytes of the header and data.
        /// </summary>
        public ushort Length
        {
           get => BinaryPrimitives.ReadUInt16BigEndian(_span.Slice(0 + sizeof(ushort) + sizeof(ushort)));
           set => BinaryPrimitives.WriteUInt16BigEndian(_span.Slice(0 + sizeof(ushort) + sizeof(ushort)), value); 
        }
        
        
        /// <summary>
        /// The checksum.
        /// </summary>
        public ushort Checksum
        {
           get => BinaryPrimitives.ReadUInt16BigEndian(_span.Slice(0 + sizeof(ushort) + sizeof(ushort) + sizeof(ushort)));
           set => BinaryPrimitives.WriteUInt16BigEndian(_span.Slice(0 + sizeof(ushort) + sizeof(ushort) + sizeof(ushort)), value); 
        }
        
        
        /// <summary>
        /// The payload.
        /// </summary>
        public System.Span<byte> Payload
        {
           get => _span.Slice(0 + sizeof(ushort) + sizeof(ushort) + sizeof(ushort) + sizeof(ushort));
        }
        
        /// <summary>
        /// Get a string representation of this packet.
        /// </summary>
        public override string ToString()
        {
            return $"SourcePort: {SourcePort}; DestinationPort: {DestinationPort}; Length: {Length}; Checksum: {Checksum}; Payload: {Payload.Length} bytes";
        }

        /// <summary>
        /// Get the computed total size of this packet, including any dynamically-sized fields and trailing payloads.
        /// </summary>
        public int GetTotalSize()
        {
            return 0 + sizeof(ushort) + sizeof(ushort) + sizeof(ushort) + sizeof(ushort) + _span.Slice(0 + sizeof(ushort) + sizeof(ushort) + sizeof(ushort) + sizeof(ushort)).Length;
        }
    }
}
