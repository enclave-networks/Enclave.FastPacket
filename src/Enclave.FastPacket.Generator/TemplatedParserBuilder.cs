using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Enclave.FastPacket.Generator.ValueProviders;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Scriban;
using Scriban.Runtime;

namespace Enclave.FastPacket.Generator;

internal class TemplatedParserBuilder : IParserBuilder
{
    private Template _template;

    public TemplatedParserBuilder(string templateName)
    {
        _template = Template.Parse(PacketParserGenerator.GetTemplateContent(templateName));
    }

    public string Generate(PacketParserDefinition packetDef, GenerationOptions definitionTypeOptions, INamedTypeSymbol structSymbol, PacketFieldFactory parserGenerator)
    {
        var sc = new ScriptObject();

        var addToStringMethod = true;

        foreach (var toStringMember in structSymbol.GetMembers("ToString"))
        {
            if (toStringMember is IMethodSymbol toStringMethod &&
                toStringMethod.DeclaredAccessibility == Accessibility.Public &&
                toStringMethod.IsOverride &&
                toStringMethod.Parameters.Length == 0)
            {
                // Type already has a defined ToString, don't override it.
                addToStringMethod = false;
                break;
            }
        }

        sc.Import(new
        {
            Namespace = structSymbol.GetFullNamespace(),
            TypeName = structSymbol.Name,
            Props = packetDef.PropertySet.ToList(),
            MinSizeExpression = packetDef.MinSizeExpression,
            AddToStringMethod = addToStringMethod,
        },
        renamer: m => m.Name);

        Func<string, string> getTotalSizeExpression = (string spanName) =>
        {
            if (packetDef.PropertySet.Count == 0)
            {
                return "0";
            }

            var lastPacket = packetDef.PropertySet[packetDef.PropertySet.Count - 1];
            var positionExpression = lastPacket.PositionProvider.GetPositionExpression(spanName);
            var sizeExpression = lastPacket.SizeProvider.GetSizeExpression(spanName, positionExpression);

            return $"{positionExpression} + {sizeExpression}";
        };

        Func<IPacketField, string, string> getPropGetExpression = (IPacketField prop, string spanName)
            => prop.ValueProvider.GetPropGetExpression(spanName, prop.PositionProvider.GetPositionExpression(spanName));

        Func<IPacketField, string, string, string> getPropSetExpression = (IPacketField prop, string spanName, string valueExpr)
            => prop.ValueProvider.GetPropSetExpression(spanName, prop.PositionProvider.GetPositionExpression(spanName), valueExpr);

        Func<IPacketField, string> getTypeReferenceName = (IPacketField prop) =>
        {
            if (prop.ValueProvider.TypeSymbol.Equals(parserGenerator.SpanByteType, SymbolEqualityComparer.Default) &&
                definitionTypeOptions.IsReadOnly)
            {
                return parserGenerator.ReadOnlySpanByteType.GetFullyQualifiedReference();
            }

            return prop.ValueProvider.TypeReferenceName;
        };

        Func<IPacketField, string> getPropName = (IPacketField prop)
            => prop.Name;

        Func<IPacketField, string> getPropAccessibility = (IPacketField prop)
            => SyntaxFacts.GetText(prop.Accessibility);

        Func<IPacketField, IEnumerable<string>> getPropComments = (IPacketField prop)
            => prop.DocComments;

        Func<IPacketField, bool> canSet = (IPacketField prop)
            => prop.ValueProvider.CanSet;

        Func<string> getToStringFormat = () =>
        {
            // Start with a certain capacity in the builder.
            var builder = new StringBuilder(packetDef.PropertySet.Count * 15);

            builder.Append("$\"");

            var anySoFar = false;

            for (int idx = 0; idx < packetDef.PropertySet.Count; idx++)
            {
                var prop = packetDef.PropertySet[idx];

                if (anySoFar)
                {
                    builder.Append("; ");
                }

                if (prop.Accessibility != Accessibility.Public)
                {
                    // Don't include private props in the ToString.
                    continue;
                }

                anySoFar = true;

                // Add the interpolated string content.
                builder.Append(prop.Name);

                if (prop.ValueProvider is ISpanValueProvider)
                {
                    builder.Append(": {");
                    builder.Append(prop.Name);
                    builder.Append(".Length} bytes");
                }
                else
                {
                    builder.Append(": {");
                    builder.Append(prop.Name);
                    builder.Append('}');
                }
            }

            builder.Append('"');

            return builder.ToString();
        };

        sc.Import("getTotalSizeExpression", getTotalSizeExpression);
        sc.Import("getPropGetExpr", getPropGetExpression);
        sc.Import("getPropSetExpr", getPropSetExpression);
        sc.Import("getTypeReferenceName", getTypeReferenceName);
        sc.Import("getPropName", getPropName);
        sc.Import("getPropAccessibility", getPropAccessibility);
        sc.Import("getPropComments", getPropComments);
        sc.Import("canSet", canSet);
        sc.Import("getToStringFormat", getToStringFormat);

        return _template.Render(new TemplateContext(sc));
    }
}
