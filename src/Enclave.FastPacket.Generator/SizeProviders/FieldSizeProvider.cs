using Enclave.FastPacket.Generator.PositionProviders;
using Enclave.FastPacket.Generator.ValueProviders;
using Microsoft.CodeAnalysis;

namespace Enclave.FastPacket.Generator.SizeProviders;

internal class FieldSizeProvider : ILateBoundFieldReferencingProvider, ISizeProvider
{
    private bool _bound;

    public FieldSizeProvider(string propertyName)
    {
        PropertyName = propertyName;
    }

    public void BindField(
        GeneratorExecutionContext execContext,
        IPacketField thisProperty,
        int thisPropertyPosition,
        IPacketField referencedPacketProperty,
        int referencedPropertyPosition)
    {
        _bound = true;

        if (referencedPacketProperty.ValueProvider is not INumericValueProvider)
        {
            _bound = false;
            execContext.ReportDiagnostic(Diagnostic.Create(Diagnostics.SizeFieldNotValidType, thisProperty.DiagnosticsLocation, referencedPacketProperty.Name));
        }

        if (referencedPropertyPosition > thisPropertyPosition && referencedPacketProperty.PositionProvider is not IConstantPositionProvider)
        {
            _bound = false;
            // The referenced field appears after this property in the property order, and it does not have a fixed position.
            // That would likely cause a reference loop.
            execContext.ReportDiagnostic(Diagnostic.Create(Diagnostics.SizeFieldAppearsAfter, thisProperty.DiagnosticsLocation, referencedPacketProperty.Name));
        }
    }

    public void FieldNotFound(GeneratorExecutionContext execContext, IPacketField thisProperty)
    {
        execContext.ReportDiagnostic(Diagnostic.Create(Diagnostics.SizeFieldNotFound, thisProperty.DiagnosticsLocation, PropertyName));
    }

    public string PropertyName { get; }

    public string GetSizeExpression(string spanName, string positionExpression)
    {
        // If we bound OK, then use the property name, else use '0' to prevent compile errors in the generated code.
        return _bound ? PropertyName : "0";
    }
}
