# Enclave.FastPacket

The FastPacket project provides efficient, zero-allocation mechanisms for reading and writing network packets.

It aims to support real-time analysis of network traffic.

We also provide a C# Source Generator that generates highly efficient packet decoders from a simple class definition.

For example, this (simplified) definition:

```csharp
internal ref struct EthernetPacketDefinition
{
    /// <summary>
    /// The destination hardware (MAC) address.
    /// </summary>
    public HardwareAddress Destination { get; set; }

    /// <summary>
    /// The source hardware (MAC) address.
    /// </summary>
    public HardwareAddress Source { get; set; }

    /// <summary>
    /// The EtherType field.
    /// </summary>
    public EthernetType Type { get; set; }

    /// <summary>
    /// The Ethernet Payload.
    /// </summary>
    public ReadOnlySpan<byte> Payload { get; set; }
}

[PacketImplementation(typeof(EthernetPacketDefinition))]
public readonly ref partial struct EthernetPacketSpan
{
}
```

will populate the `EthernetPacketSpan` type:

```csharp
 readonly ref partial struct EthernetPacketSpan
{
    private readonly Span<byte> _span;

    public EthernetPacketSpan(Span<byte> packetData)
    {
        _span = packetData;
    }

    public Span<byte> GetRawData() => _span;

        
        
    /// <summary>
    /// The destination hardware (MAC) address.
    /// </summary>
    public Enclave.FastPacket.HardwareAddress Destination
    {
        get => new Enclave.FastPacket.HardwareAddress(_span.Slice(0, Enclave.FastPacket.HardwareAddress.Size));
        set => value.CopyTo(_span.Slice(0, Enclave.FastPacket.HardwareAddress.Size)); 
    }
        
        
    /// <summary>
    /// The source hardware (MAC) address.
    /// </summary>
    public Enclave.FastPacket.HardwareAddress Source
    {
        get => new Enclave.FastPacket.HardwareAddress(_span.Slice(0 + Enclave.FastPacket.HardwareAddress.Size, Enclave.FastPacket.HardwareAddress.Size));
        set => value.CopyTo(_span.Slice(0 + Enclave.FastPacket.HardwareAddress.Size, Enclave.FastPacket.HardwareAddress.Size)); 
    }
        
        
    /// <summary>
    /// The EtherType field.
    /// </summary>
    public Enclave.FastPacket.EthernetType Type
    {
        get => (Enclave.FastPacket.EthernetType)(BinaryPrimitives.ReadUInt16BigEndian(_span.Slice(0 + Enclave.FastPacket.HardwareAddress.Size + Enclave.FastPacket.HardwareAddress.Size)));
        set => BinaryPrimitives.WriteUInt16BigEndian(_span.Slice(0 + Enclave.FastPacket.HardwareAddress.Size + Enclave.FastPacket.HardwareAddress.Size), (ushort)(value)); 
    }
        
        
    /// <summary>
    /// The Ethernet Payload.
    /// </summary>
    public System.Span<byte> Payload
    {
        get => _span.Slice(0 + Enclave.FastPacket.HardwareAddress.Size + Enclave.FastPacket.HardwareAddress.Size + sizeof(ushort));
    }        
}
```

At build time, the C# compiler will collapse all constant values in the generated type into a single constant value as efficiently as it possible,
so the eventual IL ASM after the JIT has run is very efficient (and largely inlined).

## Pre-release tasks

- [x] Refactor project layout to make management easier.
- [x] Consider better ways to interact with 'knowing' what packet type is below the current position.
- [x] Define standard 'patterns' for adding support for new protocols.
- [x] Consider(?) a source generator for converting a simple declarative syntax into efficient packet structs.
- [ ] Add support for field size coming from a preceding property.
- [ ] Add support for field size coming from a local function in the definition.
- [ ] Add support for partial-byte fields.
- [ ] Implement a layer 7 protocol inspector (HTTP?) to prove extraction all the way down the stack.
- [ ] Add more thorough test cases.

Building
--------

.NET6 SDK required.

Run `dotnet build` to build.

Tests
-------

Tests are in `/tests`. Run tests with `dotnet test`.