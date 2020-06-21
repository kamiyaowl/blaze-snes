using System;
using System.IO;
using BlazeSnes.Core.Cpu;
using Xunit;
using Xunit.Sdk;

namespace BlazeSnes.Core.Test.Cpu {
    public class ProcessorStatusTest {

        /// <summary>
        /// 初期状態がすべてfalseになっている確認
        /// </summary>
        [Fact]
        public void Initial () {
            var p = new ProcessorStatus();
            Assert.False(p.Value.HasFlag(ProcessorStatusFlags.E));
            Assert.False(p.Value.HasFlag(ProcessorStatusFlags.N));
            Assert.False(p.Value.HasFlag(ProcessorStatusFlags.V));
            Assert.False(p.Value.HasFlag(ProcessorStatusFlags.M));
            Assert.False(p.Value.HasFlag(ProcessorStatusFlags.X));
            Assert.False(p.Value.HasFlag(ProcessorStatusFlags.D));
            Assert.False(p.Value.HasFlag(ProcessorStatusFlags.I));
            Assert.False(p.Value.HasFlag(ProcessorStatusFlags.Z));
            Assert.False(p.Value.HasFlag(ProcessorStatusFlags.C));
        }

        /// <summary>
        /// Flag変数経由で値の設定が可能か確認
        /// </summary>
        [Fact]
        public void SetFromFlag () {
            var p = new ProcessorStatus();
            p.Value |= ProcessorStatusFlags.E;
            p.Value |= ProcessorStatusFlags.V;
            p.Value |= ProcessorStatusFlags.X;
            p.Value |= ProcessorStatusFlags.I;
            p.Value |= ProcessorStatusFlags.C;

            Assert.True(p.Value.HasFlag(ProcessorStatusFlags.E));
            Assert.False(p.Value.HasFlag(ProcessorStatusFlags.N));
            Assert.True(p.Value.HasFlag(ProcessorStatusFlags.V));
            Assert.False(p.Value.HasFlag(ProcessorStatusFlags.M));
            Assert.True(p.Value.HasFlag(ProcessorStatusFlags.X));
            Assert.False(p.Value.HasFlag(ProcessorStatusFlags.D));
            Assert.True(p.Value.HasFlag(ProcessorStatusFlags.I));
            Assert.False(p.Value.HasFlag(ProcessorStatusFlags.Z));
            Assert.True(p.Value.HasFlag(ProcessorStatusFlags.C));
        }
        /// <summary>
        /// 値経由で値の設定が可能か確認
        /// </summary>
        [Fact]
        public void SetFromValue () {
            var p = new ProcessorStatus();
            p.Value |= (ProcessorStatusFlags)0x8055;

            Assert.True(p.Value.HasFlag(ProcessorStatusFlags.E));
            Assert.False(p.Value.HasFlag(ProcessorStatusFlags.N));
            Assert.True(p.Value.HasFlag(ProcessorStatusFlags.V));
            Assert.False(p.Value.HasFlag(ProcessorStatusFlags.M));
            Assert.True(p.Value.HasFlag(ProcessorStatusFlags.X));
            Assert.False(p.Value.HasFlag(ProcessorStatusFlags.D));
            Assert.True(p.Value.HasFlag(ProcessorStatusFlags.I));
            Assert.False(p.Value.HasFlag(ProcessorStatusFlags.Z));
            Assert.True(p.Value.HasFlag(ProcessorStatusFlags.C));
        }
        /// <summary>
        /// Enum/Literal両方使って交互に上げ下げしてみる
        /// </summary>
        [Fact]
        public void Mixed () {
            var p = new ProcessorStatus();
            // All set
            p.Value |= (ProcessorStatusFlags)0x00aa;
            p.Value |= ProcessorStatusFlags.E;
            p.Value |= ProcessorStatusFlags.V;
            p.Value |= ProcessorStatusFlags.X;
            p.Value |= ProcessorStatusFlags.I;
            p.Value |= ProcessorStatusFlags.C;

            Assert.True(p.Value.HasFlag(ProcessorStatusFlags.E));
            Assert.True(p.Value.HasFlag(ProcessorStatusFlags.N));
            Assert.True(p.Value.HasFlag(ProcessorStatusFlags.V));
            Assert.True(p.Value.HasFlag(ProcessorStatusFlags.M));
            Assert.True(p.Value.HasFlag(ProcessorStatusFlags.X));
            Assert.True(p.Value.HasFlag(ProcessorStatusFlags.D));
            Assert.True(p.Value.HasFlag(ProcessorStatusFlags.I));
            Assert.True(p.Value.HasFlag(ProcessorStatusFlags.Z));
            Assert.True(p.Value.HasFlag(ProcessorStatusFlags.C));

            // All clear
            p.Value &= (ProcessorStatusFlags)0x00aa;
            p.Value &= ~(ProcessorStatusFlags.N);
            p.Value &= ~(ProcessorStatusFlags.M);
            p.Value &= ~(ProcessorStatusFlags.D);
            p.Value &= ~(ProcessorStatusFlags.Z);

            Assert.False(p.Value.HasFlag(ProcessorStatusFlags.E));
            Assert.False(p.Value.HasFlag(ProcessorStatusFlags.N));
            Assert.False(p.Value.HasFlag(ProcessorStatusFlags.V));
            Assert.False(p.Value.HasFlag(ProcessorStatusFlags.M));
            Assert.False(p.Value.HasFlag(ProcessorStatusFlags.X));
            Assert.False(p.Value.HasFlag(ProcessorStatusFlags.D));
            Assert.False(p.Value.HasFlag(ProcessorStatusFlags.I));
            Assert.False(p.Value.HasFlag(ProcessorStatusFlags.Z));
            Assert.False(p.Value.HasFlag(ProcessorStatusFlags.C));
        }

    }
}