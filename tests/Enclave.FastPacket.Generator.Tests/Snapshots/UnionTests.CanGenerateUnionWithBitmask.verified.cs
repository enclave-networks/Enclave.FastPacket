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
        public const int MinimumSize = sizeof(ushort) + 1 + sizeof(ushort);

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
        
        
        public ushort Value
        {
           get => BinaryPrimitives.ReadUInt16BigEndian(_span.Slice(0));
           set => BinaryPrimitives.WriteUInt16BigEndian(_span.Slice(0), value); 
        }
        
        
        public byte UnionVal1
        {
           get => (byte)((_span[0 + sizeof(ushort)] & 0xF0u) >> 4);
           set => _span[0 + sizeof(ushort)] = (byte)(((value << 4) & 0xF0u) | (byte)(_span[0 + sizeof(ushort)] & ~0xF0u)); 
        }
        
        
        public byte UnionVal2
        {
           get => (byte)(_span[0 + sizeof(ushort)] & 0xFu);
           set => _span[0 + sizeof(ushort)] = (byte)((value & 0xFu) | (byte)(_span[0 + sizeof(ushort)] & ~0xFu)); 
        }
        
        
        public ushort Value2
        {
           get => BinaryPrimitives.ReadUInt16BigEndian(_span.Slice(0 + sizeof(ushort) + 1));
           set => BinaryPrimitives.WriteUInt16BigEndian(_span.Slice(0 + sizeof(ushort) + 1), value); 
        }
        
        /// <summary>
        /// Get a string representation of this packet.
        /// </summary>
        public override string ToString()
        {
            return $"Value: {Value}; UnionVal1: {UnionVal1}; UnionVal2: {UnionVal2}; Value2: {Value2}";
        }
        
        /// <summary>
        /// Get the computed total size of this packet, including any dynamically-sized fields and trailing payloads.
        /// </summary>
        public int GetTotalSize()
        {
            return 0 + sizeof(ushort) + 1 + sizeof(ushort);
        }
    }
}
