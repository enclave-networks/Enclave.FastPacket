using System;
using System.Buffers.Binary;
using System.Net;
using System.Net.Sockets;
using System.Numerics;

namespace Enclave.FastPacket;

/// <summary>
/// Represents an Ipv4 or Ipv6 address as a value type (as opposed to the <see cref="IPAddress"/> reference type).
/// </summary>
public readonly struct ValueIpAddress : IEquatable<ValueIpAddress>
{
    private readonly ulong _addr1;
    private readonly ulong _addr2;
    private readonly AddressFamily _addrFamily;

    /// <summary>
    /// Length in bytes of an IPv4 Address.
    /// </summary>
    public const int Ipv4Length = 4;

    /// <summary>
    /// Length in bytes of an IPv6 Address.
    /// </summary>
    public const int Ipv6Length = 16;

    /// <summary>
    /// An empty IP address.
    /// </summary>
    public static ValueIpAddress None { get; }

    /// <summary>
    /// Value representation of <see cref="IPAddress.Loopback"/>.
    /// </summary>
    public static ValueIpAddress Loopback { get; } = Create(IPAddress.Loopback);

    /// <summary>
    /// Value representation of <see cref="IPAddress.IPv6Loopback"/>.
    /// </summary>
    public static ValueIpAddress IPv6Loopback { get; } = Create(IPAddress.IPv6Loopback);

    /// <summary>
    /// Value representation of <see cref="IPAddress.Broadcast"/>.
    /// </summary>
    public static ValueIpAddress Broadcast { get; } = Create(IPAddress.Broadcast);

    /// <summary>
    /// Create an IPv4 address from a buffer.
    /// </summary>
    public static ValueIpAddress CreateIpv4(ReadOnlySpan<byte> address)
    {
        if (BinaryPrimitives.TryReadUInt32BigEndian(address, out var uintAddr))
        {
            return new ValueIpAddress(AddressFamily.InterNetwork, uintAddr, 0);
        }
        else
        {
            throw new FastPacketException("Not enough bytes present to read an Ipv4 address", address);
        }
    }

    /// <summary>
    /// Create an IPv6 address from a buffer.
    /// </summary>
    public static ValueIpAddress CreateIpv6(ReadOnlySpan<byte> address)
    {
        if (BinaryPrimitives.TryReadUInt64BigEndian(address, out var addr2) &&
            BinaryPrimitives.TryReadUInt64BigEndian(address.Slice(sizeof(ulong)), out var addr1))
        {
            return new ValueIpAddress(AddressFamily.InterNetworkV6, addr1, addr2);
        }
        else
        {
            throw new FastPacketException("Not enough bytes present to read an Ipv4 address", address);
        }
    }

    /// <summary>
    /// Create a new <see cref="ValueIpAddress"/> from a framework <see cref="IPAddress" />.
    /// </summary>
    public static ValueIpAddress Create(IPAddress address)
    {
        if (address is null)
        {
            throw new ArgumentNullException(nameof(address));
        }

        var addressBytes = address.GetAddressBytes();

        if (address.AddressFamily == AddressFamily.InterNetwork)
        {
            return CreateIpv4(addressBytes);
        }
        else if (address.AddressFamily == AddressFamily.InterNetworkV6)
        {
            return CreateIpv6(addressBytes);
        }

        throw new FastPacketException("Invalid address family.", addressBytes);
    }

    /// <summary>
    /// Create a new <see cref="ValueIpAddress"/> from a buffer.
    /// </summary>
    public ValueIpAddress(ReadOnlySpan<byte> data)
    {
        if (data.Length > 4)
        {
            BinaryPrimitives.TryReadUInt64BigEndian(data, out _addr2);
            BinaryPrimitives.TryReadUInt64BigEndian(data.Slice(sizeof(ulong)), out _addr1);
            _addrFamily = AddressFamily.InterNetworkV6;
        }
        else
        {
            BinaryPrimitives.TryReadUInt32BigEndian(data, out var uintAddr);
            _addr1 = uintAddr;
            _addr2 = 0;
            _addrFamily = AddressFamily.InterNetwork;
        }
    }

    private ValueIpAddress(AddressFamily addrFamily, ulong addr1, ulong addr2)
    {
        _addr1 = addr1;
        _addr2 = addr2;
        _addrFamily = addrFamily;
    }

    /// <summary>
    /// Address family of the address.
    /// </summary>
    public AddressFamily AddressFamily => _addrFamily;

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is ValueIpAddress address && Equals(address);
    }

    /// <inheritdoc />
    public bool Equals(ValueIpAddress other)
    {
        if (_addrFamily != other._addrFamily)
        {
            return false;
        }

        if (_addrFamily == AddressFamily.InterNetwork)
        {
            return _addr1 == other._addr1;
        }

        return _addr1 == other._addr1 &&
               _addr2 == other._addr2;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(_addr1, _addr2);
    }

    /// <summary>
    /// Get a normal runtime <see cref="IPAddress"/> from this value type.
    /// </summary>
    public IPAddress ToIpAddress()
    {
        if (_addrFamily == AddressFamily.InterNetworkV6)
        {
            Span<byte> destSpan = stackalloc byte[16];

            BinaryPrimitives.WriteUInt64BigEndian(destSpan, _addr2);
            BinaryPrimitives.WriteUInt64BigEndian(destSpan.Slice(sizeof(ulong)), _addr1);

            return new IPAddress(destSpan);
        }
        else
        {
            Span<byte> destSpan = stackalloc byte[4];

            BinaryPrimitives.WriteUInt32BigEndian(destSpan, (uint)_addr1);

            return new IPAddress(destSpan);
        }
    }

    /// <summary>
    /// Copy the contents of this IP address to the provided buffer.
    /// </summary>
    public void CopyTo(Span<byte> destination)
    {
        if (_addrFamily == AddressFamily.InterNetworkV6)
        {
            if (destination.Length < Ipv6Length)
            {
                throw new FastPacketException("Destination too small for ipv6", destination);
            }

            BinaryPrimitives.WriteUInt64BigEndian(destination, _addr2);
            BinaryPrimitives.WriteUInt64BigEndian(destination.Slice(sizeof(ulong)), _addr1);
        }
        else
        {
            if (destination.Length < Ipv4Length)
            {
                throw new FastPacketException("Destination too small for ipv4", destination);
            }

            BinaryPrimitives.WriteUInt32BigEndian(destination, (uint)_addr1);
        }
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return ToIpAddress().ToString();
    }

    /// <summary>
    /// Converts an IPv4 address to a unsigned 32-bit integer.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if <see cref="AddressFamily"/> is <see cref="AddressFamily.InterNetworkV6"/>.</exception>
    public uint ToUInt()
    {
        if (_addrFamily == AddressFamily.InterNetwork)
        {
            return unchecked((uint)_addr1);
        }

        throw new InvalidOperationException("Only IPv4 addresses can be converted to a UInt32");
    }

    /// <summary>
    /// Convert this IP address to a big-integer representation.
    /// </summary>
    public BigInteger ToBigInteger()
    {
        if (_addrFamily == AddressFamily.InterNetworkV6)
        {
            Span<byte> destSpan = stackalloc byte[16];

            BinaryPrimitives.WriteUInt64BigEndian(destSpan, _addr2);
            BinaryPrimitives.WriteUInt64BigEndian(destSpan.Slice(sizeof(ulong)), _addr1);

            return new BigInteger(destSpan, isUnsigned: true, isBigEndian: true);
        }
        else
        {
            return new BigInteger(_addr1);
        }
    }

    /// <summary>
    /// Equals operator.
    /// </summary>
    public static bool operator ==(ValueIpAddress left, ValueIpAddress right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Not-Equals operator.
    /// </summary>
    public static bool operator !=(ValueIpAddress left, ValueIpAddress right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Greater than operator.
    /// </summary>
    public static bool operator >(ValueIpAddress left, ValueIpAddress right)
    {
        if (left._addrFamily != right._addrFamily)
        {
            throw new InvalidOperationException("Comparison requires both addresses to be of the same address family.");
        }

        if (left._addrFamily == AddressFamily.InterNetwork)
        {
            return left._addr1 > right._addr1;
        }

        return left._addr2 > right._addr2 ||
               (left._addr2 == right._addr2 && left._addr1 > right._addr1);
    }

    /// <summary>
    /// Greater than or equal operator.
    /// </summary>
    public static bool operator >=(ValueIpAddress left, ValueIpAddress right)
    {
        if (left._addrFamily != right._addrFamily)
        {
            throw new InvalidOperationException("Comparison requires both addresses to be of the same address family.");
        }

        if (left._addrFamily == AddressFamily.InterNetwork)
        {
            return left._addr1 >= right._addr1;
        }

        return left._addr2 > right._addr2 ||
               (left._addr2 == right._addr2 && left._addr1 >= right._addr1);
    }

    /// <summary>
    /// Less than operator.
    /// </summary>
    public static bool operator <(ValueIpAddress left, ValueIpAddress right)
    {
        if (left._addrFamily != right._addrFamily)
        {
            throw new InvalidOperationException("Comparison requires both addresses to be of the same address family.");
        }

        if (left._addrFamily == AddressFamily.InterNetwork)
        {
            return left._addr1 < right._addr1;
        }

        return left._addr2 < right._addr2 ||
               (left._addr2 == right._addr2 && left._addr1 < right._addr1);
    }

    /// <summary>
    /// Less than or equal operator.
    /// </summary>
    public static bool operator <=(ValueIpAddress left, ValueIpAddress right)
    {
        if (left._addrFamily != right._addrFamily)
        {
            throw new InvalidOperationException("Comparison requires both addresses to be of the same address family.");
        }

        if (left._addrFamily == AddressFamily.InterNetwork)
        {
            return left._addr1 <= right._addr1;
        }

        return left._addr2 < right._addr2 ||
               (left._addr2 == right._addr2 && left._addr1 <= right._addr1);
    }
}
