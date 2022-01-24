using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Enclave.FastPacket.Generator.Tests
{
    public class BitmaskTests
    {
        [Fact]
        public void DoMask()
        {
            byte existingValue = 0b1011_0100;

            byte newValue = 0b0000_0110;

            ulong mask = 0b1111_0000;

            var maskOffset = BitOperations.TrailingZeroCount(mask);

            var valueToOffset = newValue << maskOffset;

            var valueToWrite = valueToOffset | (int)(existingValue & ~mask);
        }
    }
}
