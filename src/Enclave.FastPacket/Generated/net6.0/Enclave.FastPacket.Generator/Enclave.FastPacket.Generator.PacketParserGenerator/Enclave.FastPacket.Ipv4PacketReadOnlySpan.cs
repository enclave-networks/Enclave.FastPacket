﻿// <auto-generated />

using System;
using System.Buffers.Binary;

namespace Enclave.FastPacket
{
    readonly ref partial struct Ipv4PacketReadOnlySpan
    {
        private readonly ReadOnlySpan<byte> _span;

        /// <summary>
        /// Defines the minimum possible size of this packet, given all
        /// known fixed sizes.
        /// </summary>
        public const int MinimumSize = 1 + sizeof(byte) + sizeof(ushort) + sizeof(ushort) + 2 + sizeof(byte) + sizeof(byte) + sizeof(ushort) + 4 + 4;

        public Ipv4PacketReadOnlySpan(ReadOnlySpan<byte> packetData)
        {
            _span = packetData;
        }

        public ReadOnlySpan<byte> GetRawData() => _span;

        
        
        public byte Version
        {
           get => (byte)((_span[0] & 0xF0u) >> 4);
        }
        
        
        public byte IHL
        {
           get => (byte)(_span[0] & 0xFu);
        }
        
        
        public byte Dscp
        {
           get => _span[0 + 1];
        }
        
        
        public ushort TotalLength
        {
           get => BinaryPrimitives.ReadUInt16BigEndian(_span.Slice(0 + 1 + sizeof(byte)));
        }
        
        
        public ushort Identification
        {
           get => BinaryPrimitives.ReadUInt16BigEndian(_span.Slice(0 + 1 + sizeof(byte) + sizeof(ushort)));
        }
        
        
        public Enclave.FastPacket.FragmentFlags FragmentFlags
        {
           get => (Enclave.FastPacket.FragmentFlags)((byte)((_span[0 + 1 + sizeof(byte) + sizeof(ushort) + sizeof(ushort)] & 0xE0u) >> 5));
        }
        
        
        public ushort FragmentValue
        {
           get => (ushort)(BinaryPrimitives.ReadUInt16BigEndian(_span.Slice(0 + 1 + sizeof(byte) + sizeof(ushort) + sizeof(ushort))) & 0x1FFFu);
        }
        
        
        public byte Ttl
        {
           get => _span[0 + 1 + sizeof(byte) + sizeof(ushort) + sizeof(ushort) + 2];
        }
        
        
        public System.Net.Sockets.ProtocolType Protocol
        {
           get => (System.Net.Sockets.ProtocolType)(_span[0 + 1 + sizeof(byte) + sizeof(ushort) + sizeof(ushort) + 2 + sizeof(byte)]);
        }
        
        
        public ushort HeaderChecksum
        {
           get => BinaryPrimitives.ReadUInt16BigEndian(_span.Slice(0 + 1 + sizeof(byte) + sizeof(ushort) + sizeof(ushort) + 2 + sizeof(byte) + sizeof(byte)));
        }
        
        
        public Enclave.FastPacket.ValueIpAddress Source
        {
           get => new Enclave.FastPacket.ValueIpAddress(_span.Slice(0 + 1 + sizeof(byte) + sizeof(ushort) + sizeof(ushort) + 2 + sizeof(byte) + sizeof(byte) + sizeof(ushort), 4));
        }
        
        
        public Enclave.FastPacket.ValueIpAddress Destination
        {
           get => new Enclave.FastPacket.ValueIpAddress(_span.Slice(0 + 1 + sizeof(byte) + sizeof(ushort) + sizeof(ushort) + 2 + sizeof(byte) + sizeof(byte) + sizeof(ushort) + 4, 4));
        }
        
        
        public System.ReadOnlySpan<byte> Options
        {
           get => _span.Slice(0 + 1 + sizeof(byte) + sizeof(ushort) + sizeof(ushort) + 2 + sizeof(byte) + sizeof(byte) + sizeof(ushort) + 4 + 4, Enclave.FastPacket.Ipv4Definition.GetOptionsSize(_span));
        }
        
        
        public System.ReadOnlySpan<byte> Payload
        {
           get => _span.Slice(0 + 1 + sizeof(byte) + sizeof(ushort) + sizeof(ushort) + 2 + sizeof(byte) + sizeof(byte) + sizeof(ushort) + 4 + 4 + Enclave.FastPacket.Ipv4Definition.GetOptionsSize(_span));
        }
        
    }
}
