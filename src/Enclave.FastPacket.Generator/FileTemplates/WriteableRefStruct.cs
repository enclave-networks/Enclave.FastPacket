﻿// <auto-generated />

using System;
using System.Buffers.Binary;

namespace {{Namespace}}
{
    readonly ref partial struct {{TypeName}}
    {
        private readonly Span<byte> _span;

        public {{TypeName}}(Span<byte> packetData)
        {
            _span = packetData;
        }

        public Span<byte> GetRawData() => _span;

        {{ for prop in Props }}
        {{ for comment in (getPropComments prop) }}
        /// {{ comment }}{{ end }}
        public {{ getTypeReferenceName prop }} {{ getPropName prop }}
        {
           get => {{ getPropGetExpr prop "_span" }};{{ if (canSet prop) }}
           set => {{ getPropSetExpr prop "_span" "value" }}; {{ end }}
        }
        {{ end }}
    }
}
