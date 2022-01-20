using System;
using System.Net.NetworkInformation;

namespace Enclave.FastPacket;

public readonly struct HardwareAddress : IEquatable<HardwareAddress>
{
    public const int Size = 6;

    private readonly byte _b0;
    private readonly byte _b1;
    private readonly byte _b2;
    private readonly byte _b3;
    private readonly byte _b4;
    private readonly byte _b5;

    public static HardwareAddress None { get; }

    public HardwareAddress(ReadOnlySpan<byte> address)
    {
        if (address.Length < Size)
        {
            // Create an empty address.
            _b0 = 0;
            _b1 = 0;
            _b2 = 0;
            _b3 = 0;
            _b4 = 0;
            _b5 = 0;
            return;
        }

        _b0 = address[0];
        _b1 = address[1];
        _b2 = address[2];
        _b3 = address[3];
        _b4 = address[4];
        _b5 = address[5];
    }

    public HardwareAddress(PhysicalAddress physicalAddress)
        : this(physicalAddress?.GetAddressBytes() ?? throw new ArgumentNullException(nameof(physicalAddress)))
    {
    }

    public void CopyTo(Span<byte> destination)
    {
        if (destination.Length < Size)
        {
            throw new FastPacketException("Not enough space present to copy", destination);
        }

        destination[0] = _b0;
        destination[1] = _b1;
        destination[2] = _b2;
        destination[3] = _b3;
        destination[4] = _b4;
        destination[5] = _b5;
    }

    public override bool Equals(object? obj)
    {
        return obj is HardwareAddress address && Equals(address);
    }

    public bool Equals(HardwareAddress other)
    {
        return _b0 == other._b0 &&
               _b1 == other._b1 &&
               _b2 == other._b2 &&
               _b3 == other._b3 &&
               _b4 == other._b4 &&
               _b5 == other._b5;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_b0, _b1, _b2, _b3, _b4, _b5);
    }

    public PhysicalAddress ToPhysicalAddress()
    {
        return new PhysicalAddress(new[] { _b0, _b1, _b2, _b3, _b4, _b5 });
    }

    public override string ToString()
    {
        return $"{_b0:x2}:{_b1:x2}:{_b2:x2}:{_b3:x2}:{_b4:x2}:{_b5:x2}";
    }

    public static bool operator ==(HardwareAddress left, HardwareAddress right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(HardwareAddress left, HardwareAddress right)
    {
        return !(left == right);
    }
}
