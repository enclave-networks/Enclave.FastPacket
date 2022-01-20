using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Scriban;
using Scriban.Runtime;

namespace Enclave.FastPacket.Generator
{
    internal class TemplatedParserBuilder : IParserBuilder
    {
        private Template _template;

        public TemplatedParserBuilder(string templateName)
        {
            _template = Template.Parse(PacketParserGenerator.GetTemplateContent(templateName));
        }

        public string Generate(PacketParserDefinition packetDef, GenerationOptions definitionTypeOptions, INamedTypeSymbol structSymbol, PacketPropertyFactory parserGenerator)
        {
            var sc = new ScriptObject();
            sc.Import(new
            {
                Namespace = structSymbol.GetFullNamespace(),
                TypeName = structSymbol.Name,
                Props = packetDef.PropertySet.ToList(),
            },
            renamer: m => m.Name);

            Func<IPacketProperty, string, string> getPropGetExpression = (IPacketProperty prop, string spanName)
                => prop.ValueProvider.GetPropGetExpression(spanName, prop.PositionProvider.GetPositionExpression(spanName));

            Func<IPacketProperty, string, string, string> getPropSetExpression = (IPacketProperty prop, string spanName, string valueExpr)
                => prop.ValueProvider.GetPropSetExpression(spanName, prop.PositionProvider.GetPositionExpression(spanName), valueExpr);

            Func<IPacketProperty, string> getTypeReferenceName = (IPacketProperty prop) =>
            {
                if (prop.ValueProvider.TypeSymbol.Equals(parserGenerator.SpanByteType, SymbolEqualityComparer.Default) &&
                    definitionTypeOptions.IsReadOnly)
                {
                    return parserGenerator.ReadOnlySpanByteType.GetFullyQualifiedReference();
                }

                return prop.ValueProvider.TypeReferenceName;
            };

            Func<IPacketProperty, string> getPropName = (IPacketProperty prop)
                => prop.Name;

            Func<IPacketProperty, IEnumerable<string>> getPropComments = (IPacketProperty prop)
                => prop.DocComments;

            Func<IPacketProperty, bool> canSet = (IPacketProperty prop)
                => prop.ValueProvider.CanSet;

            sc.Import("getPropGetExpr", getPropGetExpression);
            sc.Import("getPropSetExpr", getPropSetExpression);
            sc.Import("getTypeReferenceName", getTypeReferenceName);
            sc.Import("getPropName", getPropName);
            sc.Import("getPropComments", getPropComments);
            sc.Import("canSet", canSet);

            return _template.Render(new TemplateContext(sc));
        }
    }
}
