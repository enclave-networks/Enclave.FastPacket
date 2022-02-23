namespace Enclave.FastPacket;

/// <summary>
/// Represents possible ARP operations.
/// </summary>
public enum ArpOperation : ushort
{
    /// <summary>
    /// None (not a valid value).
    /// </summary>
    None = 0,

    /// <summary>
    /// An ARP request.
    /// </summary>
    Request = 1,

    /// <summary>
    /// An ARP reply.
    /// </summary>
    Reply = 2,
}
