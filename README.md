# Enclave.FastPacket

The FastPacket project provides efficient, zero-allocation mechanisms for reading and writing individual network packets.

> The library is currently pre-release as we work on tests and more protocol support.

This project consists of two projects/packages:

- Enclave.FastPacket, a high-performance library for reading and writing ethernet packets and a number of common protocols.
- Enclave.FastPacket.Generator, a .NET source generator that we use to generate the decoders in Enclave.FastPacket from their definition.

## Motivations

FastPacket aims to support real-time analysis of network traffic, and underpins the Enclave (https://enclave.io) packet analysis
behaviour needed for state-tracking of connections.

We aim for the highest-possible performance, trying to remove the cost of reading any value that you aren't actually interested in.

We also want to simplify defining packets, by allowing us to base how we lay out packets on the way RFCs and documentation represent them, rather than
how C# code typically requires it.

## Benchmarks

Here's an output of some of our benchmarks that compare the (aforementioned excellent) PacketDotNet library with FastPacket for parsing some
common packet information such as UDP/TCP ports and payload size (you can find these in the `bench` folder).

All these benchmarks start from reading a raw ethernet frame.

```markdown
BenchmarkDotNet=v0.12.1, OS=Windows 10.0.22000
Intel Core i7-9750H CPU 2.60GHz, 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=6.0.101
  [Host]     : .NET Core 6.0.1 (CoreCLR 6.0.121.56705, CoreFX 6.0.121.56705), X64 RyuJIT
  DefaultJob : .NET Core 6.0.1 (CoreCLR 6.0.121.56705, CoreFX 6.0.121.56705), X64 RyuJIT  

|                                  Method |      Mean |    Error |   StdDev |  Gen 0 |  Gen 1 | Gen 2 | Allocated |
|---------------------------------------- |----------:|---------:|---------:|-------:|-------:|------:|----------:|
| GetEthernetHardwareAddress_PacketDotNet |  45.14 ns | 0.654 ns | 0.611 ns | 0.0523 |      - |     - |     328 B |
|   GetEthernetHardwareAddress_FastPacket |  10.05 ns | 0.135 ns | 0.126 ns |      - |      - |     - |         - |
|                GetUdpPorts_PacketDotNet | 216.20 ns | 4.270 ns | 4.746 ns | 0.1428 | 0.0002 |     - |     896 B |
|                  GetUdpPorts_FastPacket |  13.14 ns | 0.194 ns | 0.181 ns |      - |      - |     - |         - |
|                GetTcpPorts_PacketDotNet | 194.48 ns | 3.679 ns | 3.778 ns | 0.1376 | 0.0002 |     - |     864 B |
|                  GetTcpPorts_FastPacket |  13.25 ns | 0.199 ns | 0.166 ns |      - |      - |     - |         - |
|          GetTcpPayloadSize_PacketDotNet | 226.71 ns | 2.318 ns | 2.055 ns | 0.1490 | 0.0002 |     - |     936 B |
|            GetTcpPayloadSize_FastPacket |  18.63 ns | 0.218 ns | 0.194 ns |      - |      - |     - |         - |
|             CheckForTcpAck_PacketDotNet | 192.36 ns | 3.523 ns | 5.053 ns | 0.1376 | 0.0002 |     - |     864 B |
|               CheckForTcpAck_FastPacket |  12.54 ns | 0.065 ns | 0.055 ns |      - |      - |     - |         - |
```

# How It Works

FastPacket is backed by a a C# Source Generator (`Enclave.FastPacket.Generator`) that generates efficient packet decoders
from a simple type definition, that read directly from the underlying buffer.

The source generator approach lets us standardise on efficient patterns for reading packets, without needing a large amount of repetitive
code everywhere.  It also lets *you* create decoders based on the same principles, without writing out bitmasks, offset calculations and big-endian
reads by hand.

Finally, the use of a source generator means we can effectively remove any virtual method calls, which helps with our performance a bit more again.

With FastPacket, we define packet *definitions*, and let the generator create the actual decoder.

For example, this simple packet definition:

```csharp
// Define the structure of an ethernet packet
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

// Define a type that should receive the implementation of the definition.
// Note that the implementation is a `readonly ref partial struct`, that's important!
[PacketImplementation(typeof(EthernetPacketDefinition))]
public readonly ref partial struct EthernetPacketSpan
{
}
```

will populate the reset of the `EthernetPacketSpan` type:

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

At build time, the C# compiler will collapse all those constant values in the generated type into a single constant value as efficiently as it possibly
can, so the eventual IL ASM after the JIT has run is very efficient (and largely inlined).

## History

For some time in Enclave, we were using the excellent [PacketDotNet](https://github.com/dotpcap/packetnet) library to analyse ethernet packets "on the wire" as they were moving through a
network adapter, but we saw that the number of memory allocations and overhead of reading packets with PacketDotNet was causing some slow downs
when we needed to analyse "some" of the content of every packet.

So, we started a small, hand-rolled library internally to read individual fields of packets directly using `ReadOnlySpan<byte>`,
rather than the `ByteArraySegment` approach used in PacketDotNet.

This then spun out into wanting a standard way to define packet structures, so the source generator was born, and decided it was useful enough to others
to open-source.

## Source Generator Features

The FastPacket Source Generator gives us a number of features that make it easier to define packet structures.

### Numeric Fields

FastPacket natively supports numeric fields that are:

- Invidual bytes
- 16, 32 and 64 unsigned integers (ushort, uint and ulong)
- 16, 32 and 64 bit signed integers (short, int and long)

Currently, FastPacket assumes that all numeric fields are big-endian, since that is the general network byte order. In future we may add support for handling little-endian values,
but for now, assume that all byte order is taken care of as big-endian.

### Automatic Size/Position Determination

By default, FastPacket generates automatic position/size constants for you as much as it can, based on the order of properties
in the type.

For example, if you define the following packet and an implementation:

```csharp
struct MyPacketDefinition
{
    public byte Field1 { get; set; }

    public byte Field2 { get; set; }

    public ushort Field2 { get; set; }
}
```

The generated properties are like so:
```
    public byte Field1
    {
        get => _span[0];
        set => _span[0] = value; 
    }        
        
    public byte Field2
    {
        get => _span[0 + sizeof(byte)];
        set => _span[0 + sizeof(byte)] = value; 
    }        
        
    public ushort Field3
    {
        get => BinaryPrimitives.ReadUInt16BigEndian(_span.Slice(0 + sizeof(byte) + sizeof(byte)));
        set => BinaryPrimitives.WriteUInt16BigEndian(_span.Slice(0 + sizeof(byte) + sizeof(byte)), value); 
    }
```

Wherever possible, we use constant computation to determine the position of fields in packets.

### ReadOnly Implementations

You often may not have a writeable `Span<byte>` containing packet data, or you may want to prevent editing of packet fields.

If you add `IsReadOnly = true` to the `PacketImplementation` attribute, then the created implementation will not be modifiable,
and will only depend on `ReadOnlySpan<byte>`.

```csharp
struct MyPacketDefinition
{
    public ushort Value1 { get; set; }
}

[PacketImplementation(typeof(MyPacketDefinition), IsReadOnly = true)]
public readonly ref partial struct MyPacket
{
}
```

### Customising Fields

If you need to adjust individual fields, you can apply the `PacketField` attribute:

```csharp
struct MyPacketDefinition
{
    // Customise this block to be 10 bytes in size.
    [PacketField(Size = 10)]
    public ReadOnlySpan<byte> DataBlock { get; set; }
}
```

This lets you do things like:

- Change the 'actual' size of a field.
- Change the computed position of a field.

### Blob Fields

You often want a field of data in a packet that represents a blob of data in some form.

You can add a `Span<byte>` or `ReadOnlySpan<byte>` field into your packet and FastPacket will give you the appropriate slice of data from the packet
based on the size of fields preceding it.

If a blob field is the last property in the definition, we assume that it uses the remaining content of the packet's buffer, as with a lot of network payloads.

If a blob field is in the middle of the packet, you will need to tell us what size it is (either explicitly or with a `SizeFunction`), otherwise we can't use it.

```csharp
struct MyPacketDefinition
{
    public byte Field1 { get; set; }

    // Have to declare a size.
    [PacketField(Size = 12)]
    public ReadOnlySpan<byte> Options { get; set; }

    // Don't need to declare a size (it's just the rest of the buffer)
    public ReadOnlySpan<byte> Payload { get; set; }
}
```

### Function-Based Size Determination

It's fairly common to determine the size of a particular field based on some other computed value, often a value in the same packet.

To that end, if you need to calculate the size of a field dynamically, you can do that by specifiying a static `SizeFunction` within the definition type.

```csharp
struct MyPacketDefinition
{
    public ushort DataLength { get; set; }

    [PacketField(SizeFunction = nameof(GetDataSize))]
    public ReadOnlySpan<byte> Data { get; set; }

    public static int GetDataSize(ReadOnlySpan<byte> packet)
    {
        // I can safely access the `DataLength` property of my own definition!
        // That works just fine provided you are reading a field *before* the one you are computing the size of.
        // In future we'll add an analyzer to prevent accidental stack overflows.
        return new MyPacket(packet).DataLength;
    }
}

[PacketImplementation(typeof(MyPacketDefinition), IsReadOnly = true)]
readonly ref partial MyPacket struct
{
}
```

Size functions are defined either as:

```csharp
public static int FuncName(ReadOnlySpan<byte> packet)
```

or

```csharp
public static int FuncName(ReadOnlySpan<byte> packet, int position)`
```

If you use the second version, the position parameter tells you the known location of the field in question in case the length is stored somewhere in that field.

### Enum Support

Enums are just naturally supported in FastPacket; define your enum and then use that enum as a property type:

```csharp
public enum FieldCodes : byte
{
    Code1 = 0x01,
    Code2 = 0x02
}

struct MyPacketDefinition
{
    public FieldCodes Code { get; set; }
}
```

The `Code` property here will read a single byte from the packet and cast it to your enum for you.

Note that I've specified a base numeric type for the enum; FastPacket can use this to figure out how much space your enum takes up in the packet.

If you aren't able to change the base type (perhaps it is an internal runtime enum), you can specify it on the property:

```csharp
struct MyPacketDefinition
{
    [PacketField(EnumBackingType = typeof(byte))]
    public ProtocolType Type { get; set; }
}
```

### Custom Types

Packet types are often more complex than just `ushort, byte, int, etc`. We may have custom structures we want to represent.

For an example, take a hardware, or MAC address. Hardware addresses are 6 bytes, but we will have other behaviour attached to them,
so we don't want to just represent them as blobs of data.

You can use *any* custom type as a FastPacket field, as long as it implements the following:

- A public constructor that takes a single `ReadOnlySpan<byte>` parameter.
- A public `CopyTo` method that acccepts a s single `Span<byte>` parameter.
- A public int constant called `Size` that defines how big the custom type is.
  *or*
  A public static method called `GetSize` that accepts a `ReadOnlySpan<byte>` that starts at the beginning of the field, and returns the size of the structure.

For example, this is a valid custom type, that can be used in any FastPacket definition:

```csharp
public struct MyCustomType
{
    private byte _b1;
    private byte _b2;

    public const int Size = 2;

    public MyCustomType(ReadOnlySpan<byte> data)
    {
        _b1 = data[0];
        _b2 = data[1];
    }

    public void CopyTo(Span<byte> data)
    {
        data[0] = _b1;
        data[1] = _b2;
    }
}
```

> The Enclave.FastPacket library has already defined `HardwareAddress` and `ValueIpAddress` types designed for reducing allocations commonly
> seen with the `PhysicalAddress` and `IpAddress` types in the runtime.

### Unions and Bit-fields

FastPacket allows us to represent 'unions', i.e. fields that share the same space, within a packet.

```csharp
struct MyPacketDefinition
{
    // Any struct inside a definition is considered a union.
    // We need to define an explicit or computed size for a union right now, but that might change in future:
    [PacketField(Size = sizeof(uint))
    struct U1
    {
        public uint RawValue { get; set; }

        public CustomValue Value { get; set; }
    }
}
```

Once that definition gets converted to an implementation, those properties will be flattened into a single entry that looks a bit like:

```
[PacketImplementation(typeof(MyPacketDefinition))]
readonly ref partial struct MyPacketImplementation
{
    // In the generated code...

    public uint RawValue { get; set; }

    public CustomValue Value { get; set; }
}
```

However, both `RawValue` and `Value` occupy the same range of bytes in the packet as each other.

The real power of unions doesn't really reveal itself however until you start using bit-fields. Let's look at a snipped part of the TCP header
definition, from the RFC:

```
   |                    Acknowledgment Number                      |
   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
   |  Data |           |U|A|P|R|S|F|                               |
   | Offset| Reserved  |R|C|S|S|Y|I|            Window             |
   |       |           |G|K|H|T|N|N|                               |
   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
   |           Checksum            |         Urgent Pointer        |
````

The Data Offset, Reserved and Flags use up two bytes of space, but different parts of those two bytes mean different things. Let's
look at how we can represent this easily with FastPacket:

```csharp
internal ref struct TcpPacketDefinition
{
    // Snip...

    public uint AckNumber { get; set; }

    [PacketField(Size = sizeof(ushort))]
    public struct UFlags
    {
        [PacketFieldBits(0, 3)]
        public byte DataOffset { get; set; }

        [PacketFieldBits(10, 15)]
        public TcpFlags Flags { get; set; }
    }

    public ushort WindowSize { get; set; }

    public ushort Checksum { get; set; }

    public ushort UrgentPointer { get; set; }

    // Snip ...
}
```

The `PacketFieldBits` attribute lets us specify which *bits* a given field occupies within the union. Bits are specified as an inclusive range of bits within the
union, using MSB-0 numbering to match a lot of RFC documentation.

The FastPacket generator automatically generates the necessary bitmasks and shifts to convert the MSB-0 numbering and means that each value you read is only
the bits that you've specified.

# Remaining Pre-release tasks

- [x] Refactor project layout to make management easier.
- [x] Consider better ways to interact with 'knowing' what packet type is below the current position.
- [x] Define standard 'patterns' for adding support for new protocols.
- [x] Consider(?) a source generator for converting a simple declarative syntax into efficient packet structs.
- [ ] Add support for field size coming from a named preceding property.
- [x] Add support for field size coming from a local function in the definition.
- [x] Add support for partial-byte fields.
- [ ] Support string extraction (ReadOnlySpan<char>?). A UTF8 reader may be better.
- [ ] Implement a layer 7 protocol inspector (HTTP?) to prove extraction all the way down the stack.
- [ ] Add more thorough test cases.

# Contributors

Got a protocol you want to support? Raise an issue for it so we can discuss, then we should be able to fold a packet definition
(and tests) for that packet into the library fairly easily.

## Build

.NET6 SDK required.

Run `dotnet build` to build.

## Tests

Tests are in `/tests`. Run tests with `dotnet test`.