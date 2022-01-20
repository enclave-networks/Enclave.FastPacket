using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Enclave.FastPacket.Generator
{

    internal class ConstantPositionProvider : IPositionProvider
    {
        public ConstantPositionProvider(int position)
        {
            Position = position;
        }

        public int Position { get; set; }

        public virtual string GetPositionExpression(string spanName)
        {
            return Position.ToString(CultureInfo.InvariantCulture);
        }
    }
}
