﻿// <auto-generated />

using System;
using System.Buffers.Binary;

namespace Enclave.FastPacket
{
    readonly ref partial struct ReadOnlyUdpPacketSpan
    {
        private readonly ReadOnlySpan<byte> _span;

        /// <summary>
        /// Defines the minimum possible size of this packet, given all
        /// known fixed sizes.
        /// </summary>
        public const int MinimumSize = sizeof(ushort) + sizeof(ushort) + sizeof(ushort) + sizeof(ushort);

        /// <summary>
        /// Create a new instance of <see cref="ReadOnlyUdpPacketSpan"/>.
        /// </summary>
        public ReadOnlyUdpPacketSpan(ReadOnlySpan<byte> packetData)
        {
            _span = packetData;
        }

        /// <summary>
        /// Gets the raw underlying buffer for this packet.
        /// </summary>
        public ReadOnlySpan<byte> GetRawData() => _span;
        
        
        /// <summary>
        /// The source port.
        /// </summary>
        public ushort SourcePort
        {
           get => BinaryPrimitives.ReadUInt16BigEndian(_span.Slice(0));
        }
        
        
        /// <summary>
        /// The destination port.
        /// </summary>
        public ushort DestinationPort
        {
           get => BinaryPrimitives.ReadUInt16BigEndian(_span.Slice(0 + sizeof(ushort)));
        }
        
        
        /// <summary>
        /// The length in bytes of the header and data.
        /// </summary>
        public ushort Length
        {
           get => BinaryPrimitives.ReadUInt16BigEndian(_span.Slice(0 + sizeof(ushort) + sizeof(ushort)));
        }
        
        
        /// <summary>
        /// The checksum.
        /// </summary>
        public ushort Checksum
        {
           get => BinaryPrimitives.ReadUInt16BigEndian(_span.Slice(0 + sizeof(ushort) + sizeof(ushort) + sizeof(ushort)));
        }
        
        
        /// <summary>
        /// The payload.
        /// </summary>
        public System.ReadOnlySpan<byte> Payload
        {
           get => _span.Slice(0 + sizeof(ushort) + sizeof(ushort) + sizeof(ushort) + sizeof(ushort));
        }
        
        public override string ToString()
        {
            return $"SourcePort: {SourcePort}; DestinationPort: {DestinationPort}; Length: {Length}; Checksum: {Checksum}; Payload: {Payload.Length} bytes";
        }

        public int GetTotalSize()
        {
            return 0 + sizeof(ushort) + sizeof(ushort) + sizeof(ushort) + sizeof(ushort) + _span.Slice(0 + sizeof(ushort) + sizeof(ushort) + sizeof(ushort) + sizeof(ushort)).Length;
        }
    }
}
