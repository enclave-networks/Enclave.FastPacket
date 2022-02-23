﻿// <auto-generated />

using System;
using System.Buffers.Binary;

namespace Enclave.FastPacket
{
    readonly ref partial struct ArpPacketSpan
    {
        private readonly Span<byte> _span;

        /// <summary>
        /// Defines the minimum possible size of this packet, given all
        /// known fixed sizes.
        /// </summary>
        public const int MinimumSize = sizeof(ushort) + sizeof(ushort) + sizeof(byte) + sizeof(byte) + sizeof(ushort) + Enclave.FastPacket.HardwareAddress.Size + 4 + Enclave.FastPacket.HardwareAddress.Size + 4;

        /// <summary>
        /// Create a new instance of <see cref="ArpPacketSpan"/>.
        /// </summary>
        public ArpPacketSpan(Span<byte> packetData)
        {
            _span = packetData;
        }

        /// <summary>
        /// Gets the raw underlying buffer for this packet.
        /// </summary>
        public Span<byte> GetRawData() => _span;

        
        
        /// <summary>
        /// Specifes the network link protocol type.
        /// </summary>
        public ushort HardwareType
        {
           get => BinaryPrimitives.ReadUInt16BigEndian(_span.Slice(0));
           set => BinaryPrimitives.WriteUInt16BigEndian(_span.Slice(0), value); 
        }
        
        
        /// <summary>
        /// Specifies the IP protocol for which the ARP request is intended. For IPv4 packets, this will be 0x800.
        /// </summary>
        public ushort ProtocolType
        {
           get => BinaryPrimitives.ReadUInt16BigEndian(_span.Slice(0 + sizeof(ushort)));
           set => BinaryPrimitives.WriteUInt16BigEndian(_span.Slice(0 + sizeof(ushort)), value); 
        }
        
        
        /// <summary>
        /// The length of the hardware address segment (will be 6).
        /// </summary>
        public byte HardwareAddressLength
        {
           get => _span[0 + sizeof(ushort) + sizeof(ushort)];
           set => _span[0 + sizeof(ushort) + sizeof(ushort)] = value; 
        }
        
        
        /// <summary>
        /// The length of the hardware address segment (will be 4 for IPv4 requests).
        /// </summary>
        public byte ProtocolAddressLength
        {
           get => _span[0 + sizeof(ushort) + sizeof(ushort) + sizeof(byte)];
           set => _span[0 + sizeof(ushort) + sizeof(ushort) + sizeof(byte)] = value; 
        }
        
        
        /// <summary>
        /// Specifies the arp operation (request or reply).
        /// </summary>
        public Enclave.FastPacket.ArpOperation Operation
        {
           get => (Enclave.FastPacket.ArpOperation)(BinaryPrimitives.ReadUInt16BigEndian(_span.Slice(0 + sizeof(ushort) + sizeof(ushort) + sizeof(byte) + sizeof(byte))));
           set => BinaryPrimitives.WriteUInt16BigEndian(_span.Slice(0 + sizeof(ushort) + sizeof(ushort) + sizeof(byte) + sizeof(byte)), (ushort)(value)); 
        }
        
        
        /// <summary>
        /// The hardware address of the sender.
        /// </summary>
        public Enclave.FastPacket.HardwareAddress SenderHardwareAddress
        {
           get => new Enclave.FastPacket.HardwareAddress(_span.Slice(0 + sizeof(ushort) + sizeof(ushort) + sizeof(byte) + sizeof(byte) + sizeof(ushort), Enclave.FastPacket.HardwareAddress.Size));
           set => value.CopyTo(_span.Slice(0 + sizeof(ushort) + sizeof(ushort) + sizeof(byte) + sizeof(byte) + sizeof(ushort), Enclave.FastPacket.HardwareAddress.Size)); 
        }
        
        
        /// <summary>
        /// The protocol address of the sender.
        /// </summary>
        public Enclave.FastPacket.ValueIpAddress SenderProtocolAddress
        {
           get => new Enclave.FastPacket.ValueIpAddress(_span.Slice(0 + sizeof(ushort) + sizeof(ushort) + sizeof(byte) + sizeof(byte) + sizeof(ushort) + Enclave.FastPacket.HardwareAddress.Size, 4));
           set => value.CopyTo(_span.Slice(0 + sizeof(ushort) + sizeof(ushort) + sizeof(byte) + sizeof(byte) + sizeof(ushort) + Enclave.FastPacket.HardwareAddress.Size, 4)); 
        }
        
        
        /// <summary>
        /// The hardware address of the target (blank for ARP requests).
        /// </summary>
        public Enclave.FastPacket.HardwareAddress TargetHardwareAddress
        {
           get => new Enclave.FastPacket.HardwareAddress(_span.Slice(0 + sizeof(ushort) + sizeof(ushort) + sizeof(byte) + sizeof(byte) + sizeof(ushort) + Enclave.FastPacket.HardwareAddress.Size + 4, Enclave.FastPacket.HardwareAddress.Size));
           set => value.CopyTo(_span.Slice(0 + sizeof(ushort) + sizeof(ushort) + sizeof(byte) + sizeof(byte) + sizeof(ushort) + Enclave.FastPacket.HardwareAddress.Size + 4, Enclave.FastPacket.HardwareAddress.Size)); 
        }
        
        
        /// <summary>
        /// The protocol address of the target.
        /// </summary>
        public Enclave.FastPacket.ValueIpAddress TargetProtocolAddress
        {
           get => new Enclave.FastPacket.ValueIpAddress(_span.Slice(0 + sizeof(ushort) + sizeof(ushort) + sizeof(byte) + sizeof(byte) + sizeof(ushort) + Enclave.FastPacket.HardwareAddress.Size + 4 + Enclave.FastPacket.HardwareAddress.Size, 4));
           set => value.CopyTo(_span.Slice(0 + sizeof(ushort) + sizeof(ushort) + sizeof(byte) + sizeof(byte) + sizeof(ushort) + Enclave.FastPacket.HardwareAddress.Size + 4 + Enclave.FastPacket.HardwareAddress.Size, 4)); 
        }
        
    }
}