namespace Enclave.FastPacket;

/// <summary>
/// A visitor result.
/// </summary>
/// <typeparam name="TVisitorState">The state type.</typeparam>
public readonly ref struct VisitResult<TVisitorState>
{
    /// <summary>
    /// Create a new <see cref="VisitResult{TVisitorState}"/>.
    /// </summary>
    /// <param name="visitorState">The state of the visitor.</param>
    /// <param name="lengthConsumed">The number of bytes consumed from the initial payload.</param>
    /// <param name="lengthRemaining">The number of bytes remaining in the initial payload.</param>
    public VisitResult(in TVisitorState visitorState, int lengthConsumed, int lengthRemaining)
    {
        VisitorState = visitorState;
        LengthConsumed = lengthConsumed;
        LengthRemaining = lengthRemaining;
    }

    /// <summary>
    /// The visitor state at the end of the visit process.
    /// </summary>
    public TVisitorState VisitorState { get; }

    /// <summary>
    /// The number of bytes consumed from the initial payload.
    /// </summary>
    public int LengthConsumed { get; }

    /// <summary>
    /// The number of bytes remaining in the initial payload.
    /// </summary>
    public int LengthRemaining { get; }
}
