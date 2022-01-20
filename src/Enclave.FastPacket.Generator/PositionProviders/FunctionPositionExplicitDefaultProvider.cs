using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Enclave.FastPacket.Generator
{

    internal class FunctionPositionExplicitDefaultProvider : ConstantPositionProvider
    {
        public FunctionPositionExplicitDefaultProvider(IMethodSymbol positionMethod, int explicitPosition)
            : base(explicitPosition)
        {
            Method = positionMethod;
            FullReferenceName = positionMethod.GetFullyQualifiedReference();
        }

        public IMethodSymbol Method { get; }

        public string FullReferenceName { get; }

        public override string GetPositionExpression(string spanName)
        {
            // Automatic position calculation is just based on the position expression of the previous property,
            // plus the size (to take us to the start of this field).
            return $"{FullReferenceName}({spanName}, {base.GetPositionExpression(spanName)})";
        }
    }
}
