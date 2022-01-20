using Microsoft.CodeAnalysis;

namespace Enclave.FastPacket.Generator
{
    internal readonly struct GenerationOptions
    {
        public static bool TryGetFromAttribute(AttributeData attrib, out GenerationOptions opt)
        {
            INamedTypeSymbol? definitionTypeSymbol = null;
            var isReadOnly = false;
            var cacheAll = false;

            // Only constructor argument right now is the backing type.
            if (attrib.ConstructorArguments.Length > 0)
            {
                if (attrib.ConstructorArguments[0].Value is INamedTypeSymbol typeSymbol)
                {
                    definitionTypeSymbol = typeSymbol;
                }
            }

            foreach (var additionalArg in attrib.NamedArguments)
            {
                if (additionalArg.Key == nameof(PacketImplementationAttribute.IsReadOnly))
                {
                    if (additionalArg.Value.Value is bool attrIsReadOnly)
                    {
                        isReadOnly = attrIsReadOnly;
                    }
                }
                else if (additionalArg.Key == nameof(PacketImplementationAttribute.CacheAll))
                {
                    if (additionalArg.Value.Value is bool attrCacheAll)
                    {
                        cacheAll = attrCacheAll;
                    }
                }
            }

            if (definitionTypeSymbol is not null)
            {
                opt = new GenerationOptions(definitionTypeSymbol, isReadOnly);
                return true;
            }

            opt = default;
            return false;
        }

        private GenerationOptions(INamedTypeSymbol definitionType, bool isReadOnly)
        {
            DefinitionType = definitionType;
            IsReadOnly = isReadOnly;
        }

        public INamedTypeSymbol DefinitionType { get; }

        public bool IsReadOnly { get; }
    }
}
