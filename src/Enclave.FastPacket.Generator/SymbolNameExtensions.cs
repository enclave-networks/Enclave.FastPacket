using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Enclave.FastPacket.Generator;

internal static class SymbolNameExtensions
{
    public static string GetFullyQualifiedReference(this ISymbol symbol)
    {
        var st = ImmutableStack<string>.Empty;

        if (symbol is INamedTypeSymbol)
        {
            return symbol.ToDisplayString();
        }

        st = st.Push(symbol.Name);

        ISymbol? currentSymbol = symbol.ContainingType ?? (ISymbol?)symbol.ContainingNamespace;

        if (currentSymbol is ISymbol)
        {
            st = st.Push(currentSymbol.Name);

            while (currentSymbol?.ContainingNamespace is object && currentSymbol.ContainingNamespace.Name.Length > 0)
            {
                currentSymbol = currentSymbol.ContainingNamespace;

                st = st.Push(currentSymbol.Name);
            }
        }

        return string.Join(".", st);
    }

    public static string GetFullyQualifiedGeneratedFileName(this INamedTypeSymbol symbol)
    {
        var st = ImmutableStack<string>.Empty;

        st.Push("_Generated.cs");
        st = st.Push(symbol.Name);

        ISymbol? currentSymbol = symbol.ContainingType ?? (ISymbol?)symbol.ContainingNamespace;

        if (currentSymbol is ISymbol)
        {
            st = st.Push(currentSymbol.Name);

            while (currentSymbol?.ContainingNamespace is object && currentSymbol.ContainingNamespace.Name.Length > 0)
            {
                currentSymbol = currentSymbol.ContainingNamespace;

                st = st.Push(currentSymbol.Name);
            }
        }

        return string.Join(".", st);
    }

    public static string GetFullNamespace(this ISymbol symbol)
    {
        var st = ImmutableStack<string>.Empty;

        ISymbol? currentSymbol = symbol.ContainingType ?? (ISymbol?)symbol.ContainingNamespace;

        if (currentSymbol is ISymbol)
        {
            st = st.Push(currentSymbol.Name);

            while (currentSymbol?.ContainingNamespace is object && currentSymbol.ContainingNamespace.Name.Length > 0)
            {
                currentSymbol = currentSymbol.ContainingNamespace;

                st = st.Push(currentSymbol.Name);
            }
        }

        return string.Join(".", st);
    }
}
