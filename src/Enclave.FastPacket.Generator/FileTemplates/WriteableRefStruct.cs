﻿// <auto-generated />

using System;
using System.Buffers.Binary;

namespace {{Namespace}}
{
    readonly ref partial struct {{TypeName}}
    {
        private readonly Span<byte> _span;

        /// <summary>
        /// Defines the minimum possible size of this packet, given all
        /// known fixed sizes.
        /// </summary>
        public const int MinimumSize = {{ MinSizeExpression }};

        /// <summary>
        /// Create a new instance of <see cref="{{TypeName}}"/>.
        /// </summary>
        public {{TypeName}}(Span<byte> packetData)
        {
            _span = packetData;
        }

        /// <summary>
        /// Gets the raw underlying buffer for this packet.
        /// </summary>
        public Span<byte> GetRawData() => _span;
        {{ for prop in Props }}
        {{ for comment in (getPropComments prop) }}
        /// {{ comment }}{{ end }}
        {{ getPropAccessibility prop }} {{ getTypeReferenceName prop }} {{ getPropName prop }}
        {
           get => {{ getPropGetExpr prop "_span" }};{{ if (canSet prop) }}
           set => {{ getPropSetExpr prop "_span" "value" }}; {{ end }}
        }
        {{ end }}{{ if AddToStringMethod }}
        /// <summary>
        /// Get a string representation of this packet.
        /// </summary>
        public override string ToString()
        {
            return {{ getToStringFormat }};
        }
        {{ end }}
        /// <summary>
        /// Get the computed total size of this packet, including any dynamically-sized fields and trailing payloads.
        /// </summary>
        public int GetTotalSize()
        {
            return {{ getTotalSizeExpression "_span" }};
        }
    }
}
