using BlazeSnes.Core.Cpu;
using System;
using System.IO;
using Xunit;
using Xunit.Sdk;

namespace BlazeSnes.Core.Test.Cpu {
    public class ProcessorStatusTest {

        /// <summary>
        /// 初期状態がすべてfalseになっている確認
        /// </summary>
        [Fact]
        public void Initial() {
            var p = new ProcessorStatus();
            Assert.False(p.Value.HasFlag(ProcessorStatusFlag.E));
            Assert.False(p.Value.HasFlag(ProcessorStatusFlag.N));
            Assert.False(p.Value.HasFlag(ProcessorStatusFlag.V));
            Assert.False(p.Value.HasFlag(ProcessorStatusFlag.M));
            Assert.False(p.Value.HasFlag(ProcessorStatusFlag.X));
            Assert.False(p.Value.HasFlag(ProcessorStatusFlag.D));
            Assert.False(p.Value.HasFlag(ProcessorStatusFlag.I));
            Assert.False(p.Value.HasFlag(ProcessorStatusFlag.Z));
            Assert.False(p.Value.HasFlag(ProcessorStatusFlag.C));
        }

        /// <summary>
        /// Flag変数経由で値の設定が可能か確認
        /// </summary>
        [Fact]
        public void SetFromFlag() {
            var p = new ProcessorStatus();
            p.Value |= ProcessorStatusFlag.E;
            p.Value |= ProcessorStatusFlag.V;
            p.Value |= ProcessorStatusFlag.X;
            p.Value |= ProcessorStatusFlag.I;
            p.Value |= ProcessorStatusFlag.C;

            Assert.True(p.Value.HasFlag(ProcessorStatusFlag.E));
            Assert.False(p.Value.HasFlag(ProcessorStatusFlag.N));
            Assert.True(p.Value.HasFlag(ProcessorStatusFlag.V));
            Assert.False(p.Value.HasFlag(ProcessorStatusFlag.M));
            Assert.True(p.Value.HasFlag(ProcessorStatusFlag.X));
            Assert.False(p.Value.HasFlag(ProcessorStatusFlag.D));
            Assert.True(p.Value.HasFlag(ProcessorStatusFlag.I));
            Assert.False(p.Value.HasFlag(ProcessorStatusFlag.Z));
            Assert.True(p.Value.HasFlag(ProcessorStatusFlag.C));
        }
        /// <summary>
        /// 値経由で値の設定が可能か確認
        /// </summary>
        [Fact]
        public void SetFromValue() {
            var p = new ProcessorStatus();
            p.Value |= (ProcessorStatusFlag)0x8055;

            Assert.True(p.Value.HasFlag(ProcessorStatusFlag.E));
            Assert.False(p.Value.HasFlag(ProcessorStatusFlag.N));
            Assert.True(p.Value.HasFlag(ProcessorStatusFlag.V));
            Assert.False(p.Value.HasFlag(ProcessorStatusFlag.M));
            Assert.True(p.Value.HasFlag(ProcessorStatusFlag.X));
            Assert.False(p.Value.HasFlag(ProcessorStatusFlag.D));
            Assert.True(p.Value.HasFlag(ProcessorStatusFlag.I));
            Assert.False(p.Value.HasFlag(ProcessorStatusFlag.Z));
            Assert.True(p.Value.HasFlag(ProcessorStatusFlag.C));
        }
        /// <summary>
        /// Enum/Literal両方使って交互に上げ下げしてみる
        /// </summary>
        [Fact]
        public void Mixed() {
            var p = new ProcessorStatus();
            // All set
            p.Value |= (ProcessorStatusFlag)0x00aa;
            p.Value |= ProcessorStatusFlag.E;
            p.Value |= ProcessorStatusFlag.V;
            p.Value |= ProcessorStatusFlag.X;
            p.Value |= ProcessorStatusFlag.I;
            p.Value |= ProcessorStatusFlag.C;

            Assert.True(p.Value.HasFlag(ProcessorStatusFlag.E));
            Assert.True(p.Value.HasFlag(ProcessorStatusFlag.N));
            Assert.True(p.Value.HasFlag(ProcessorStatusFlag.V));
            Assert.True(p.Value.HasFlag(ProcessorStatusFlag.M));
            Assert.True(p.Value.HasFlag(ProcessorStatusFlag.X));
            Assert.True(p.Value.HasFlag(ProcessorStatusFlag.D));
            Assert.True(p.Value.HasFlag(ProcessorStatusFlag.I));
            Assert.True(p.Value.HasFlag(ProcessorStatusFlag.Z));
            Assert.True(p.Value.HasFlag(ProcessorStatusFlag.C));

            // All clear
            p.Value &= (ProcessorStatusFlag)0x00aa;
            p.Value &= ~(ProcessorStatusFlag.N);
            p.Value &= ~(ProcessorStatusFlag.M);
            p.Value &= ~(ProcessorStatusFlag.D);
            p.Value &= ~(ProcessorStatusFlag.Z);

            Assert.False(p.Value.HasFlag(ProcessorStatusFlag.E));
            Assert.False(p.Value.HasFlag(ProcessorStatusFlag.N));
            Assert.False(p.Value.HasFlag(ProcessorStatusFlag.V));
            Assert.False(p.Value.HasFlag(ProcessorStatusFlag.M));
            Assert.False(p.Value.HasFlag(ProcessorStatusFlag.X));
            Assert.False(p.Value.HasFlag(ProcessorStatusFlag.D));
            Assert.False(p.Value.HasFlag(ProcessorStatusFlag.I));
            Assert.False(p.Value.HasFlag(ProcessorStatusFlag.Z));
            Assert.False(p.Value.HasFlag(ProcessorStatusFlag.C));
        }

    }
}