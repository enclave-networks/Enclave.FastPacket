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
        
        
        public bool Value1
        {
           get => ((byte)((_span[0 + sizeof(ushort)] & 0x80u) >> 7)) > 0;
           set => _span[0 + sizeof(ushort)] = (byte)((((value ? 1 : 0) << 7) & 0x80u) | (byte)(_span[0 + sizeof(ushort)] & ~0x80u)); 
        }
        
        
        public bool UnionVal2
        {
           get => ((byte)((_span[0 + sizeof(ushort)] & 0x40u) >> 6)) > 0;
           set => _span[0 + sizeof(ushort)] = (byte)((((value ? 1 : 0) << 6) & 0x40u) | (byte)(_span[0 + sizeof(ushort)] & ~0x40u)); 
        }
        
        
        public ushort Value2
        {
           get => BinaryPrimitives.ReadUInt16BigEndian(_span.Slice(0 + sizeof(ushort) + 1));
           set => BinaryPrimitives.WriteUInt16BigEndian(_span.Slice(0 + sizeof(ushort) + 1), value); 
        }
        
        public override string ToString()
        {
            return $"Value: {Value}; Value1: {Value1}; UnionVal2: {UnionVal2}; Value2: {Value2}";
        }

        public int GetTotalSize()
        {
            return 0 + sizeof(ushort) + 1 + sizeof(ushort);
        }
    }
}