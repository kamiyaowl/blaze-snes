using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using BlazeSnes.Core.Common;
using BlazeSnes.Core.Cpu;

using Xunit;
using Xunit.Sdk;

namespace BlazeSnes.Core.Test.Common {
    public class ExtensionTest {
        [Fact]
        public void DeepCloneForPrimitive() {
            var src = 5;
            var dst = src.DeepClone();
            src = 3;
            Assert.Equal(5, dst);
        }

        [Serializable]
        public struct TestStruct {
            public int A;
            public char B;
            public float C;
            public double D;
        }

        [Fact]
        public void DeepCloneForStruct() {
            var src = new TestStruct() {
                A = 1,
                B = '2',
                C = 3.45f,
                D = 6.789,
            };

            var dst = src.DeepClone();
            src.A = 2;
            src.B = '3';
            src.C = 4.56f;
            src.D = 7.890;
            Assert.Equal(1, dst.A);
            Assert.Equal('2', dst.B);
            Assert.Equal(3.45f, dst.C);
            Assert.Equal(6.789, dst.D);
        }

        [Serializable]
        public struct TestClass {
            public int A;
            public char B;
            public float C;
            public double D;
        }

        [Fact]
        public void DeepCloneForClass() {
            var src = new TestClass() {
                A = 1,
                B = '2',
                C = 3.45f,
                D = 6.789,
            };

            var dst = src.DeepClone();
            src.A = 2;
            src.B = '3';
            src.C = 4.56f;
            src.D = 7.890;
            Assert.Equal(1, dst.A);
            Assert.Equal('2', dst.B);
            Assert.Equal(3.45f, dst.C);
            Assert.Equal(6.789, dst.D);
        }

    }
}