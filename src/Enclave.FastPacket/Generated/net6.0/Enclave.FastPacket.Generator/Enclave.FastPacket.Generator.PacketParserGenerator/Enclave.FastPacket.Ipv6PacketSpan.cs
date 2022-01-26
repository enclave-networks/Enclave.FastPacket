﻿// <auto-generated />

using System;
using System.Buffers.Binary;

namespace Enclave.FastPacket
{
    readonly ref partial struct Ipv6PacketSpan
    {
        private readonly Span<byte> _span;

        /// <summary>
        /// Defines the minimum possible size of this packet, given all
        /// known fixed sizes.
        /// </summary>
        public const int MinimumSize = 4 + sizeof(ushort) + sizeof(byte) + sizeof(byte) + 16 + 16;

        public Ipv6PacketSpan(Span<byte> packetData)
        {
            _span = packetData;
        }

        public Span<byte> GetRawData() => _span;

        
        
        public byte Version
        {
           get => (byte)((_span[0] & 0xF0u) >> 4);
           set => _span[0] = (byte)(((value << 4) & 0xF0u) | (byte)(_span[0] & ~0xF0u)); 
        }
        
        
        public byte TrafficClass
        {
           get => (byte)((ushort)((BinaryPrimitives.ReadUInt16BigEndian(_span.Slice(0)) & 0xFF0u) >> 4));
           set => BinaryPrimitives.WriteUInt16BigEndian(_span.Slice(0), (ushort)((((ushort)(value) << 4) & 0xFF0u) | (ushort)(BinaryPrimitives.ReadUInt16BigEndian(_span.Slice(0)) & ~0xFF0u))); 
        }
        
        
        public uint FlowLabel
        {
           get => (uint)(BinaryPrimitives.ReadUInt32BigEndian(_span.Slice(0)) & 0xFFFFFu);
           set => BinaryPrimitives.WriteUInt32BigEndian(_span.Slice(0), (uint)((value & 0xFFFFFu) | (uint)(BinaryPrimitives.ReadUInt32BigEndian(_span.Slice(0)) & ~0xFFFFFu))); 
        }
        
        
        public ushort PayloadLength
        {
           get => BinaryPrimitives.ReadUInt16BigEndian(_span.Slice(0 + 4));
           set => BinaryPrimitives.WriteUInt16BigEndian(_span.Slice(0 + 4), value); 
        }
        
        
        public System.Net.Sockets.ProtocolType NextHeader
        {
           get => (System.Net.Sockets.ProtocolType)(_span[0 + 4 + sizeof(ushort)]);
           set => _span[0 + 4 + sizeof(ushort)] = (byte)(value); 
        }
        
        
        public byte HopLimit
        {
           get => _span[0 + 4 + sizeof(ushort) + sizeof(byte)];
           set => _span[0 + 4 + sizeof(ushort) + sizeof(byte)] = value; 
        }
        
        
        public Enclave.FastPacket.ValueIpAddress SourceAddress
        {
           get => new Enclave.FastPacket.ValueIpAddress(_span.Slice(0 + 4 + sizeof(ushort) + sizeof(byte) + sizeof(byte), 16));
           set => value.CopyTo(_span.Slice(0 + 4 + sizeof(ushort) + sizeof(byte) + sizeof(byte), 16)); 
        }
        
        
        public Enclave.FastPacket.ValueIpAddress DestinationAddress
        {
           get => new Enclave.FastPacket.ValueIpAddress(_span.Slice(0 + 4 + sizeof(ushort) + sizeof(byte) + sizeof(byte) + 16, 16));
           set => value.CopyTo(_span.Slice(0 + 4 + sizeof(ushort) + sizeof(byte) + sizeof(byte) + 16, 16)); 
        }
        
        
        public System.Span<byte> Payload
        {
           get => _span.Slice(0 + 4 + sizeof(ushort) + sizeof(byte) + sizeof(byte) + 16 + 16);
        }
        
    }
}