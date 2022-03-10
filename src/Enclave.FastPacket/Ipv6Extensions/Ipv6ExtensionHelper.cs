using System;

namespace Enclave.FastPacket.Ipv6Extensions;

/// <summary>
/// Defines an interface for a visitor type for IPv6 Extensions, that is invoked for each extension structure found in an IPv6 packet.
/// </summary>
/// <typeparam name="TState">A state object.</typeparam>
public interface IIpv6ExtensionVisitor<TState>
{
    /// <summary>
    /// Invoked when a Hop by Hop or Destination extension structure is found.
    /// </summary>
    /// <param name="state">The current visitor state.</param>
    /// <param name="protocol">The protocol value for this extension, as reported by the previous NextHeader value.</param>
    /// <param name="span">The decoded packet.</param>
    /// <param name="stopVisiting">If set to true by the invoked method, stop the visitor early.</param>
    /// <returns>The new visitor state.</returns>
    TState VisitIpv6HopByHopAndDestinationExtension(in TState state, IpProtocol protocol, ReadOnlyIpv6HopByHopAndDestinationExtensionSpan span, ref bool stopVisiting);

    /// <summary>
    /// Invoked when an IPv6 Routing extension is found.
    /// </summary>
    /// <param name="state">The current visitor state.</param>
    /// <param name="protocol">The protocol value for this extension, as reported by the previous NextHeader value.</param>
    /// <param name="span">The decoded packet.</param>
    /// <param name="stopVisiting">If set to true by the invoked method, stop the visitor early.</param>
    /// <returns>The new visitor state.</returns>
    TState VisitIpv6RoutingExtension(in TState state, IpProtocol protocol, ReadOnlyIpv6RoutingExtensionSpan span, ref bool stopVisiting);

    /// <summary>
    /// Invoked when an IPv6 fragment extension is found.
    /// </summary>
    /// <param name="state">The current visitor state.</param>
    /// <param name="protocol">The protocol value for this extension, as reported by the previous NextHeader value.</param>
    /// <param name="span">The decoded packet.</param>
    /// <param name="stopVisiting">If set to true by the invoked method, stop the visitor early.</param>
    /// <returns>The new visitor state.</returns>
    TState VisitIpv6FragmentExtension(in TState state, IpProtocol protocol, ReadOnlyIpv6FragmentExtensionSpan span, ref bool stopVisiting);
}

/// <summary>
/// Provides a visitor pattern for IPv6 Extensions.
/// </summary>
public readonly ref struct Ipv6ExtensionVisitor
{
    private readonly ReadOnlyIpv6PacketSpan _outerPacket;

    /// <summary>
    /// Create a new instance of <see cref="Ipv6ExtensionVisitor"/>.
    /// </summary>
    /// <param name="outerPacket">The IPv6 Packet that contains the extensions.</param>
    public Ipv6ExtensionVisitor(ReadOnlyIpv6PacketSpan outerPacket)
    {
        _outerPacket = outerPacket;
    }

    /// <summary>
    /// Get the total size of all the IPv6 Extensions that are present.
    /// </summary>
    public int GetSize()
    {
        var visitResult = Visit(NextProtocolCapturingVisitor.Instance, _outerPacket.NextHeader);

        return visitResult.LengthConsumed;
    }

    /// <summary>
    /// Get the "actual" protocol type and payload, after all IPv6 Extensions.
    /// </summary>
    public void GetActualPayload(out IpProtocol payloadProtocol, out ReadOnlySpan<byte> payload)
    {
        var visitResult = Visit(NextProtocolCapturingVisitor.Instance, _outerPacket.NextHeader);

        payloadProtocol = visitResult.VisitorState;

        if (visitResult.LengthConsumed > 0)
        {
            payload = _outerPacket.Payload.Slice(visitResult.LengthConsumed);
        }
        else
        {
            payload = _outerPacket.Payload;
        }
    }

    /// <summary>
    /// Visit each IPv6 extension in the packet.
    /// </summary>
    /// <typeparam name="TVisitor">The visitor implementation.</typeparam>
    /// <typeparam name="TVisitorState">The visitor state.</typeparam>
    /// <param name="visitor">An instance of a visitor that is invoked when different packets are provided.</param>
    /// <param name="state">An state value that is passed byref, and can be used to pass state around through the calls to the visitor.</param>
    /// <returns>A visit result, indicating how much data was consumed by the visitor.</returns>
    public VisitResult<TVisitorState> Visit<TVisitor, TVisitorState>(TVisitor visitor, in TVisitorState state)
        where TVisitor : IIpv6ExtensionVisitor<TVisitorState>
    {
        var nextProtocol = _outerPacket.NextHeader;
        var remainingData = _outerPacket.Payload;
        var consumed = 0;
        var currentState = state;
        var stopVisiting = false;

        while (!stopVisiting)
        {
            var (extNext, isExtension, extSize) = TryVisitNextBlock(nextProtocol, visitor, ref currentState, remainingData, ref stopVisiting);

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
        return new VisitResult<TVisitorState>(currentState, consumed, remainingData.Length);
    }

    private sealed class NextProtocolCapturingVisitor : IIpv6ExtensionVisitor<IpProtocol>
    {
        public static NextProtocolCapturingVisitor Instance { get; } = new NextProtocolCapturingVisitor();

        private NextProtocolCapturingVisitor()
        {
        }

        public IpProtocol VisitIpv6FragmentExtension(in IpProtocol state, IpProtocol protocol, ReadOnlyIpv6FragmentExtensionSpan span, ref bool stopVisiting)
        {
            return span.NextHeader;
        }

        public IpProtocol VisitIpv6HopByHopAndDestinationExtension(in IpProtocol state, IpProtocol protocol, ReadOnlyIpv6HopByHopAndDestinationExtensionSpan span, ref bool stopVisiting)
        {
            return span.NextHeader;
        }

        public IpProtocol VisitIpv6RoutingExtension(in IpProtocol state, IpProtocol protocol, ReadOnlyIpv6RoutingExtensionSpan span, ref bool stopVisiting)
        {
            return span.NextHeader;
        }
    }

    private static (IpProtocol NextProt, bool IsExtension, int TotalSize) TryVisitNextBlock<TVisitor, TVisitorState>(
        IpProtocol nextProtocol,
        TVisitor visitor,
        ref TVisitorState visitorState,
        ReadOnlySpan<byte> remainingBuffer,
        ref bool stopVisiting)
        where TVisitor : IIpv6ExtensionVisitor<TVisitorState>
    {
        switch (nextProtocol)
        {
            case IpProtocol.IPv6HopByHopOptions:
            case IpProtocol.IPv6DestinationOptions:
            {
                // Interrogate the extension data.
                var ext = new ReadOnlyIpv6HopByHopAndDestinationExtensionSpan(remainingBuffer);

                visitorState = visitor.VisitIpv6HopByHopAndDestinationExtension(visitorState, nextProtocol, ext, ref stopVisiting);

                return (ext.NextHeader, true, ext.GetTotalSize());
            }

            case IpProtocol.IPv6RoutingHeader:
            {
                var ext = new ReadOnlyIpv6RoutingExtensionSpan(remainingBuffer);

                visitorState = visitor.VisitIpv6RoutingExtension(visitorState, nextProtocol, ext, ref stopVisiting);

                return (ext.NextHeader, true, ext.GetTotalSize());
            }

            case IpProtocol.IPv6FragmentHeader:
            {
                var ext = new ReadOnlyIpv6FragmentExtensionSpan(remainingBuffer);

                visitorState = visitor.VisitIpv6FragmentExtension(visitorState, nextProtocol, ext, ref stopVisiting);

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