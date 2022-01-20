using System;
using System.Buffers.Binary;
using System.Net;
using System.Net.Sockets;

namespace Enclave.FastPacket;

/// <summary>
/// Represents an Ipv4 or Ipv6 address as a value type (as opposed to the <see cref="IPAddress"/> reference type).
/// </summary>
public readonly struct ValueIpAddress : IEquatable<ValueIpAddress>
{
    private readonly long _addr1;
    private readonly long _addr2;
    private readonly AddressFamily _addrFamily;

    public const int Ipv4Length = 4;
    public const int Ipv6Length = 16;

    public static ValueIpAddress CreateIpv4(ReadOnlySpan<byte> address)
    {
        if (BinaryPrimitives.TryReadInt32BigEndian(address, out var uintAddr))
        {
            return new ValueIpAddress(AddressFamily.InterNetwork, uintAddr, 0);
        }
        else
        {
            throw new FastPacketException("Not enough bytes present to read an Ipv4 address", address);
        }
    }

    public static ValueIpAddress CreateIpv6(ReadOnlySpan<byte> address)
    {
        if (BinaryPrimitives.TryReadInt64BigEndian(address, out var addr1) &&
            BinaryPrimitives.TryReadInt64BigEndian(address.Slice(sizeof(long)), out var addr2))
        {
            return new ValueIpAddress(AddressFamily.InterNetworkV6, addr1, addr2);
        }
        else
        {
            throw new FastPacketException("Not enough bytes present to read an Ipv4 address", address);
        }
    }

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

    public ValueIpAddress(ReadOnlySpan<byte> data)
    {
        if (data.Length > 4)
        {
            BinaryPrimitives.TryReadInt64BigEndian(data, out _addr1);
            BinaryPrimitives.TryReadInt64BigEndian(data.Slice(sizeof(long)), out _addr2);
            _addrFamily = AddressFamily.InterNetworkV6;
        }
        else
        {
            BinaryPrimitives.TryReadInt32BigEndian(data, out var uintAddr);
            _addr1 = uintAddr;
            _addr2 = 0;
            _addrFamily = AddressFamily.InterNetwork;
        }
    }

    private ValueIpAddress(AddressFamily addrFamily, long addr1, long addr2)
    {
        _addr1 = addr1;
        _addr2 = addr2;
        _addrFamily = addrFamily;
    }

    public override bool Equals(object? obj)
    {
        return obj is ValueIpAddress address && Equals(address);
    }

    public bool Equals(ValueIpAddress other)
    {
        return _addr1 == other._addr1 &&
               _addr2 == other._addr2 &&
               _addrFamily == other._addrFamily;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_addr1, _addr2);
    }

    public IPAddress ToIpAddress()
    {
        if (_addrFamily == AddressFamily.InterNetworkV6)
        {
            Span<byte> destSpan = stackalloc byte[16];

            BinaryPrimitives.WriteInt64BigEndian(destSpan, _addr1);
            BinaryPrimitives.WriteInt64BigEndian(destSpan.Slice(sizeof(long)), _addr2);

            return new IPAddress(destSpan);
        }
        else
        {
            Span<byte> destSpan = stackalloc byte[4];

            BinaryPrimitives.WriteInt32BigEndian(destSpan, (int)_addr1);

            return new IPAddress(destSpan);
        }
    }

    public void CopyTo(Span<byte> destination)
    {
        if (_addrFamily == AddressFamily.InterNetworkV6)
        {
            if (destination.Length < 16)
            {
                throw new FastPacketException("Destination too small for ipv6", destination);
            }

            BinaryPrimitives.WriteInt64BigEndian(destination, _addr1);
            BinaryPrimitives.WriteInt64BigEndian(destination.Slice(sizeof(long)), _addr2);
        }
        else
        {
            if (destination.Length < 4)
            {
                throw new FastPacketException("Destination too small for ipv4", destination);
            }

            BinaryPrimitives.WriteInt32BigEndian(destination, (int)_addr1);
        }
    }

    public override string ToString()
    {
        return ToIpAddress().ToString();
    }

    public static bool operator ==(ValueIpAddress left, ValueIpAddress right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ValueIpAddress left, ValueIpAddress right)
    {
        return !(left == right);
    }
}
