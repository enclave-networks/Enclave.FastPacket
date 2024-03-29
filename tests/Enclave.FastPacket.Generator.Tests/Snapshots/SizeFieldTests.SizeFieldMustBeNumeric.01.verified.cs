﻿//HintName: T.ValueItem_Generated.cs
// <auto-generated />

using System;
using System.Buffers.Binary;

namespace T
{
    readonly ref partial struct ValueItem
    {
        private readonly Span<byte> _span;

        /// <summary>
        /// Defines the minimum possible size of this packet, given all
        /// known fixed sizes.
        /// </summary>
        public const int MinimumSize = sizeof(byte);

        /// <summary>
        /// Create a new instance of <see cref="ValueItem"/>.
        /// </summary>
        public ValueItem(Span<byte> packetData)
        {
            _span = packetData;
        }

        /// <summary>
        /// Gets the raw underlying buffer for this packet.
        /// </summary>
        public Span<byte> GetRawData() => _span;
        
        
        public bool DataSize
        {
           get => (_span[0]) > 0;
           set => _span[0] = (value ? 1 : 0); 
        }
        
        
        public System.Span<byte> Value
        {
           get => _span.Slice(0 + sizeof(byte), 0);
        }
        
        /// <summary>
        /// Get a string representation of this packet.
        /// </summary>
        public override string ToString()
        {
            return $"DataSize: {DataSize}; Value: {Value.Length} bytes";
        }
        
        /// <summary>
        /// Get the computed total size of this packet, including any dynamically-sized fields and trailing payloads.
        /// </summary>
        public int GetTotalSize()
        {
            return 0 + sizeof(byte) + 0;
        }
    }
}
