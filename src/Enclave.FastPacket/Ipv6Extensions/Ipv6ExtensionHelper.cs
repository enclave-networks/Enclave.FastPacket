using System;

namespace Enclave.FastPacket;

public readonly ref struct VisitResult<TVisitorState>
{
    public VisitResult(ref TVisitorState visitorState, int lengthConsumed, int lengthRemaining)
    {
        VisitorState = visitorState;
        LengthConsumed = lengthConsumed;
        LengthRemaining = lengthRemaining;
    }

    public TVisitorState VisitorState { get; }

    public int LengthConsumed { get; }

    public int LengthRemaining { get; }
}

public interface IIpv6ExtensionVisitor<TState>
{
    void VisitIpv6HopByHopAndDestinationExtension(ref TState state, IpProtocol protocol, ReadOnlyIpv6HopByHopAndDestinationExtensionSpan span);

    void VisitIpv6RoutingExtension(ref TState state, IpProtocol protocol, ReadOnlyIpv6RoutingExtensionSpan span);

    void VisitIpv6FragmentExtension(ref TState state, IpProtocol protocol, ReadOnlyIpv6FragmentExtensionSpan span);
}

public readonly ref struct Ipv6ExtensionVisitor
{
    private readonly ReadOnlyIpv6PacketSpan _outerPacket;
    private readonly ReadOnlySpan<byte> _availableBuffer;

    public Ipv6ExtensionVisitor(ReadOnlyIpv6PacketSpan outerPacket, ReadOnlySpan<byte> availableBuffer)
    {
        _outerPacket = outerPacket;
        _availableBuffer = availableBuffer;
    }

    public int GetSize()
    {
        var finalProtocol = IpProtocol.IPv6NoNextHeader;
        var visitResult = Visit(NextProtocolCapturingVisitor.Instance, ref finalProtocol);

        return visitResult.LengthConsumed;
    }

    public void GetActualPayload(out ReadOnlySpan<byte> payload, out IpProtocol payloadProtocol)
    {
        var finalProtocol = _outerPacket.NextHeader;
        var visitResult = Visit(NextProtocolCapturingVisitor.Instance, ref finalProtocol);

        payloadProtocol = finalProtocol;
        payload = _availableBuffer.Slice(visitResult.LengthConsumed);
    }

    public VisitResult<TVisitorState> Visit<TVisitor, TVisitorState>(TVisitor visitor, ref TVisitorState state)
        where TVisitor : IIpv6ExtensionVisitor<TVisitorState>
    {
        var nextProtocol = _outerPacket.NextHeader;
        var remainingData = _availableBuffer;
        var consumed = 0;

        while (true)
        {
            var (extNext, isExtension, extSize) = TryVisitNextBlock(nextProtocol, visitor, ref state, remainingData);

            if (isExtension)
            {
                nextProtocol = extNext;
                remainingData = remainingData.Slice(extSize);
                consumed += extSize;
            }
            else
            {
                break;
            }
        }

        // If there's no next header, it means there's no data payload,
        // so return false.
        return new VisitResult<TVisitorState>(ref state, consumed, remainingData.Length);
    }

    private sealed class NextProtocolCapturingVisitor : IIpv6ExtensionVisitor<IpProtocol>
    {
        public static NextProtocolCapturingVisitor Instance { get; } = new NextProtocolCapturingVisitor();

        private NextProtocolCapturingVisitor()
        {
        }

        public void VisitIpv6FragmentExtension(ref IpProtocol state, IpProtocol protocol, ReadOnlyIpv6FragmentExtensionSpan span)
        {
            state = span.NextHeader;
        }

        public void VisitIpv6HopByHopAndDestinationExtension(ref IpProtocol state, IpProtocol protocol, ReadOnlyIpv6HopByHopAndDestinationExtensionSpan span)
        {
            state = span.NextHeader;
        }

        public void VisitIpv6RoutingExtension(ref IpProtocol state, IpProtocol protocol, ReadOnlyIpv6RoutingExtensionSpan span)
        {
            state = span.NextHeader;
        }
    }

    private static (IpProtocol NextProt, bool IsExtension, int TotalSize) TryVisitNextBlock<TVisitor, TVisitorState>(
        IpProtocol nextProtocol,
        TVisitor visitor,
        ref TVisitorState visitorState,
        ReadOnlySpan<byte> remainingBuffer)
        where TVisitor : IIpv6ExtensionVisitor<TVisitorState>
    {
        switch (nextProtocol)
        {
            case IpProtocol.IPv6HopByHopOptions:
            case IpProtocol.IPv6DestinationOptions:
            {
                // Interrogate the extension data.
                var ext = new ReadOnlyIpv6HopByHopAndDestinationExtensionSpan(remainingBuffer);

                visitor.VisitIpv6HopByHopAndDestinationExtension(ref visitorState, nextProtocol, ext);

                return (ext.NextHeader, true, ext.GetTotalSize());
            }

            case IpProtocol.IPv6RoutingHeader:
            {
                var ext = new ReadOnlyIpv6RoutingExtensionSpan(remainingBuffer);

                visitor.VisitIpv6RoutingExtension(ref visitorState, nextProtocol, ext);

                return (ext.NextHeader, true, ext.GetTotalSize());
            }

            case IpProtocol.IPv6FragmentHeader:
            {
                var ext = new ReadOnlyIpv6FragmentExtensionSpan(remainingBuffer);

                visitor.VisitIpv6FragmentExtension(ref visitorState, nextProtocol, ext);

                return (ext.NextHeader, true, ext.GetTotalSize());
            }

            default:
            {
                // No extensions left to understand.
                // This payload segment will be the data.
                return (nextProtocol, false, remainingBuffer.Length);
            }
        }
    }
}