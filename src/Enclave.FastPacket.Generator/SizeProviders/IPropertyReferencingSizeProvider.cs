using Microsoft.CodeAnalysis;

namespace Enclave.FastPacket.Generator.SizeProviders;

/// <summary>
/// Indicates a provider that directly references another field on the packet,
/// and performs late bound validation against it.
/// </summary>
internal interface ILateBoundFieldReferencingProvider
{
    string PropertyName { get; }

    void FieldNotFound(GeneratorExecutionContext execContext, IPacketField thisField);

    void BindField(
        GeneratorExecutionContext execContext,
        IPacketField thisField,
        int thisFieldPosition,
        IPacketField referencedFieldProperty,
        int referencedFieldPosition);
}
