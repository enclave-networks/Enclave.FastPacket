using Microsoft.CodeAnalysis;

namespace Enclave.FastPacket.Generator;

public static class Diagnostics
{
    private const string CommonPositionFunctionMessage =
        "Position functions must be public static methods in the definition type, with the following signature: `public static int {0}(ReadOnlySpan<byte> packetData, int defaultPosition)`.";

    private const string CommonSizeFunctionMessage =
        "Position functions must be public static methods in the definition type, with the following signature: `public static int {0}(ReadOnlySpan<byte> packetData, int position)`.";

    public static readonly DiagnosticDescriptor TypeIsNotPartial = new DiagnosticDescriptor(id: "FASTPACKET001",
                                                                                             title: "Decorated type must be partial",
                                                                                             messageFormat: "Decorated type {0} must be partial",
                                                                                             category: "FastPacket",
                                                                                             DiagnosticSeverity.Error,
                                                                                             isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor PositionFunctionIsNotFound = new DiagnosticDescriptor(id: "FASTPACKET002",
                                                                                             title: "Specified position function was not found",
                                                                                             messageFormat: "The specified position function {0} is not found in this class. " + CommonPositionFunctionMessage,
                                                                                             category: "FastPacket",
                                                                                             DiagnosticSeverity.Error,
                                                                                             isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor PositionFunctionIsNotPublicStatic = new DiagnosticDescriptor(id: "FASTPACKET003",
                                                                                             title: "Specified position function is not public static",
                                                                                             messageFormat: "The specified position function {0} is not public static. " + CommonPositionFunctionMessage,
                                                                                             category: "FastPacket",
                                                                                             DiagnosticSeverity.Error,
                                                                                             isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor PositionFunctionUnexpectedSignature = new DiagnosticDescriptor(id: "FASTPACKET004",
                                                                                             title: "Specified position function has the wrong signature",
                                                                                             messageFormat: "The specified position function {0} has the wrong signature. " + CommonPositionFunctionMessage,
                                                                                             category: "FastPacket",
                                                                                             DiagnosticSeverity.Error,
                                                                                             isEnabledByDefault: true);


    public static readonly DiagnosticDescriptor CustomFieldTypeNoConstructor = new DiagnosticDescriptor(id: "FASTPACKET005",
                                                                                             title: "Custom field type has no suitable constructor",
                                                                                             messageFormat: "The specified custom field type {0} does not have a suitable constructor; expecting `public {0}(ReadOnlySpan<byte> contents)`",
                                                                                             category: "FastPacket",
                                                                                             DiagnosticSeverity.Error,
                                                                                             isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor CustomFieldTypeNoCopyTo = new DiagnosticDescriptor(id: "FASTPACKET006",
                                                                                             title: "Custom field type has no CopyToMethod",
                                                                                             messageFormat: "The specified custom field type {0} does not have a suitable CopyTo method; expecting `public void CopyTo(Span<byte> destination)`",
                                                                                             category: "FastPacket",
                                                                                             DiagnosticSeverity.Error,
                                                                                             isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor CustomFieldTypeMustProvideLength = new DiagnosticDescriptor(id: "FASTPACKET007",
                                                                                             title: "Custom field type does not provide its own size, so you must provide the size",
                                                                                             messageFormat: "The specified custom field type {0} does not provide a `public const int Size`, or a " +
                                                                                                            "`public static int GetSize(ReadOnlySpan<byte> fieldBuffer)` method, so you must specify the " +
                                                                                                            "`Size` property on `PacketFieldAttribute`",
                                                                                             category: "FastPacket",
                                                                                             DiagnosticSeverity.Error,
                                                                                             isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor SpanInMiddleOfPacketMustHaveSize = new DiagnosticDescriptor(id: "FASTPACKET008",
                                                                                             title: "If specifying a span of bytes in the middle of a packet, you must provide a size",
                                                                                             messageFormat: "The specified field {0} declares a variable sized block of data in the middle of a packet, so you must specify the " +
                                                                                                            "`Size` or `SizeFunction` property on `PacketFieldAttribute`",
                                                                                             category: "FastPacket",
                                                                                             DiagnosticSeverity.Error,
                                                                                             isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor EnumUnderlyingTypeInvalid = new DiagnosticDescriptor(id: "FASTPACKET009",
                                                                                             title: "The specified type is not a valid backing type for an enum",
                                                                                             messageFormat: "The field {0} indicates an enum backing type of {1}, but {1} is not a valid numeric enum backing type",
                                                                                             category: "FastPacket",
                                                                                             DiagnosticSeverity.Error,
                                                                                             isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor SizeFunctionIsNotFound = new DiagnosticDescriptor(id: "FASTPACKET010",
                                                                                             title: "Specified size function was not found",
                                                                                             messageFormat: "The specified size function {0} is not found in this class. " + CommonSizeFunctionMessage,
                                                                                             category: "FastPacket",
                                                                                             DiagnosticSeverity.Error,
                                                                                             isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor SizeFunctionIsNotPublicStatic = new DiagnosticDescriptor(id: "FASTPACKET011",
                                                                                             title: "Specified size function is not public static",
                                                                                             messageFormat: "The specified size function {0} is not public static. " + CommonSizeFunctionMessage,
                                                                                             category: "FastPacket",
                                                                                             DiagnosticSeverity.Error,
                                                                                             isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor SizeFunctionUnexpectedSignature = new DiagnosticDescriptor(id: "FASTPACKET012",
                                                                                             title: "Specified size function has the wrong signature",
                                                                                             messageFormat: "The specified size function {0} has the wrong signature. " + CommonSizeFunctionMessage,
                                                                                             category: "FastPacket",
                                                                                             DiagnosticSeverity.Error,
                                                                                             isEnabledByDefault: true);


    public static readonly DiagnosticDescriptor UnionsShouldHaveDeclaredSize = new DiagnosticDescriptor(id: "FASTPACKET013",
                                                                                             title: "Union structures in packets should declare a size using the PacketField attribute",
                                                                                             messageFormat: "The union structure {0} must declare a size with either the 'Size' or 'SizeFunction' properties of a PacketFieldAttribute on the attached to the structure",
                                                                                             category: "FastPacket",
                                                                                             DiagnosticSeverity.Error,
                                                                                             isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor InternalError = new DiagnosticDescriptor(id: "FASTPACKET255",
                                                                                             title: "The FastPacket generator encountered an unexpected error",
                                                                                             messageFormat: "The FastPacket generator encountered an unexpected error while generating code for the {0} type. Error was: {1}.",
                                                                                             category: "FastPacket",
                                                                                             DiagnosticSeverity.Error,
                                                                                             isEnabledByDefault: true);
}
