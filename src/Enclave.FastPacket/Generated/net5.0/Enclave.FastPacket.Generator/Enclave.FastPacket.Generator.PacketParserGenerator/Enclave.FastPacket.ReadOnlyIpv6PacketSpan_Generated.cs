﻿// <auto-generated />

using System;
using System.Buffers.Binary;

namespace Enclave.FastPacket
{
    readonly ref partial struct ReadOnlyIpv6PacketSpan
    {
        private readonly ReadOnlySpan<byte> _span;

        /// <summary>
        /// Defines the minimum possible size of this packet, given all
        /// known fixed sizes.
        /// </summary>
        public const int MinimumSize = 4 + sizeof(ushort) + sizeof(byte) + sizeof(byte) + 16 + 16;

        /// <summary>
        /// Create a new instance of <see cref="ReadOnlyIpv6PacketSpan"/>.
        /// </summary>
        public ReadOnlyIpv6PacketSpan(ReadOnlySpan<byte> packetData)
        {
            _span = packetData;
        }

        /// <summary>
        /// Gets the raw underlying buffer for this packet.
        /// </summary>
        public ReadOnlySpan<byte> GetRawData() => _span;
        
        
        /// <summary>
        /// The IP version (6).
        /// </summary>
        public byte Version
        {
           get => (byte)((_span[0] & 0xF0u) >> 4);
        }
        
        
        /// <summary>
        /// The traffic class.
        /// </summary>
        public byte TrafficClass
        {
           get => (byte)((ushort)((BinaryPrimitives.ReadUInt16BigEndian(_span.Slice(0)) & 0xFF0u) >> 4));
        }
        
        
        /// <summary>
        /// The flow label, identifying a flow of packets between source and destination.
        /// </summary>
        public uint FlowLabel
        {
           get => (uint)(BinaryPrimitives.ReadUInt32BigEndian(_span.Slice(0)) & 0xFFFFFu);
        }
        
        
        /// <summary>
        /// The size of the payload in bytes.
        /// </summary>
        public ushort PayloadLength
        {
           get => BinaryPrimitives.ReadUInt16BigEndian(_span.Slice(0 + 4));
        }
        
        
        /// <summary>
        /// The 'next header' value.
        /// </summary>
        public Enclave.FastPacket.IpProtocol NextHeader
        {
           get => (Enclave.FastPacket.IpProtocol)(_span[0 + 4 + sizeof(ushort)]);
        }
        
        
        /// <summary>
        /// The hop limit, decremented by every forwarding node.
        /// </summary>
        public byte HopLimit
        {
           get => _span[0 + 4 + sizeof(ushort) + sizeof(byte)];
        }
        
        
        /// <summary>
        /// The source IPv6 address.
        /// </summary>
        public Enclave.FastPacket.ValueIpAddress Source
        {
           get => new Enclave.FastPacket.ValueIpAddress(_span.Slice(0 + 4 + sizeof(ushort) + sizeof(byte) + sizeof(byte), 16));
        }
        
        
        /// <summary>
        /// The destination IPv6 address.
        /// </summary>
        public Enclave.FastPacket.ValueIpAddress Destination
        {
           get => new Enclave.FastPacket.ValueIpAddress(_span.Slice(0 + 4 + sizeof(ushort) + sizeof(byte) + sizeof(byte) + 16, 16));
        }
        
        
        /// <summary>
        /// The payload.
        /// </summary>
        public System.ReadOnlySpan<byte> Payload
        {
           get => _span.Slice(0 + 4 + sizeof(ushort) + sizeof(byte) + sizeof(byte) + 16 + 16);
        }
        
        public override string ToString()
        {
            return $"Version: {Version}; TrafficClass: {TrafficClass}; FlowLabel: {FlowLabel}; PayloadLength: {PayloadLength}; NextHeader: {NextHeader}; HopLimit: {HopLimit}; Source: {Source}; Destination: {Destination}; Payload: {Payload.Length} bytes";
        }

        public int GetTotalSize()
        {
            return 0 + 4 + sizeof(ushort) + sizeof(byte) + sizeof(byte) + 16 + 16 + _span.Slice(0 + 4 + sizeof(ushort) + sizeof(byte) + sizeof(byte) + 16 + 16).Length;
        }
    }
}
