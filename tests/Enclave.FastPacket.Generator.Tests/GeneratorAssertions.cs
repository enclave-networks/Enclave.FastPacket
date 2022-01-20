using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

#nullable enable

namespace Enclave.FastPacket.Generator.Tests
{
    internal static class GeneratorAssertions
    {
        public static void AssertGeneratedFile(this GeneratorDriverRunResult result, string fileName, Action<SyntaxTree>? treeAssertion = null)
        {
            SyntaxTree? foundTree = null;

            foreach (var tree in result.GeneratedTrees)
            {
                if (tree.FilePath == $"Enclave.FastPacket.Generator\\Enclave.FastPacket.Generated\\{fileName}")
                {
                    foundTree = tree;
                    break;
                }
            }

            Assert.NotNull(foundTree);

            treeAssertion?.Invoke(foundTree!);
        }
    }
}
