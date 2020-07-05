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
        public void Initial() {
            var p = new ProcessorStatus();
            Assert.False(p.HasFlag(ProcessorStatusFlag.E));
            Assert.False(p.HasFlag(ProcessorStatusFlag.N));
            Assert.False(p.HasFlag(ProcessorStatusFlag.V));
            Assert.False(p.HasFlag(ProcessorStatusFlag.M));
            Assert.False(p.HasFlag(ProcessorStatusFlag.X));
            Assert.False(p.HasFlag(ProcessorStatusFlag.D));
            Assert.False(p.HasFlag(ProcessorStatusFlag.I));
            Assert.False(p.HasFlag(ProcessorStatusFlag.Z));
            Assert.False(p.HasFlag(ProcessorStatusFlag.C));
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

            Assert.True(p.HasFlag(ProcessorStatusFlag.E));
            Assert.False(p.HasFlag(ProcessorStatusFlag.N));
            Assert.True(p.HasFlag(ProcessorStatusFlag.V));
            Assert.False(p.HasFlag(ProcessorStatusFlag.M));
            Assert.True(p.HasFlag(ProcessorStatusFlag.X));
            Assert.False(p.HasFlag(ProcessorStatusFlag.D));
            Assert.True(p.HasFlag(ProcessorStatusFlag.I));
            Assert.False(p.HasFlag(ProcessorStatusFlag.Z));
            Assert.True(p.HasFlag(ProcessorStatusFlag.C));
        }
        /// <summary>
        /// 値経由で値の設定が可能か確認
        /// </summary>
        [Fact]
        public void SetFromValue() {
            var p = new ProcessorStatus();
            p.Value |= (ProcessorStatusFlag)0x8055;

            Assert.True(p.HasFlag(ProcessorStatusFlag.E));
            Assert.False(p.HasFlag(ProcessorStatusFlag.N));
            Assert.True(p.HasFlag(ProcessorStatusFlag.V));
            Assert.False(p.HasFlag(ProcessorStatusFlag.M));
            Assert.True(p.HasFlag(ProcessorStatusFlag.X));
            Assert.False(p.HasFlag(ProcessorStatusFlag.D));
            Assert.True(p.HasFlag(ProcessorStatusFlag.I));
            Assert.False(p.HasFlag(ProcessorStatusFlag.Z));
            Assert.True(p.HasFlag(ProcessorStatusFlag.C));
        }
        /// <summary>
        /// Enum/Literal両方使って交互に上げ下げしてみる
        /// </summary>
        [Fact]
        public void UpdateFlag() {
            var p = new ProcessorStatus();
            // All set
            p.Value |= (ProcessorStatusFlag)0x00aa;
            p.UpdateFlag(ProcessorStatusFlag.E, true);
            p.UpdateFlag(ProcessorStatusFlag.V, true);
            p.UpdateFlag(ProcessorStatusFlag.X, true);
            p.UpdateFlag(ProcessorStatusFlag.I, true);
            p.UpdateFlag(ProcessorStatusFlag.C, true);

            Assert.True(p.HasFlag(ProcessorStatusFlag.E));
            Assert.True(p.HasFlag(ProcessorStatusFlag.N));
            Assert.True(p.HasFlag(ProcessorStatusFlag.V));
            Assert.True(p.HasFlag(ProcessorStatusFlag.M));
            Assert.True(p.HasFlag(ProcessorStatusFlag.X));
            Assert.True(p.HasFlag(ProcessorStatusFlag.D));
            Assert.True(p.HasFlag(ProcessorStatusFlag.I));
            Assert.True(p.HasFlag(ProcessorStatusFlag.Z));
            Assert.True(p.HasFlag(ProcessorStatusFlag.C));

            // All clear
            p.Value &= (ProcessorStatusFlag)0x00aa;
            p.UpdateFlag(ProcessorStatusFlag.N, false);
            p.UpdateFlag(ProcessorStatusFlag.M, false);
            p.UpdateFlag(ProcessorStatusFlag.D, false);
            p.UpdateFlag(ProcessorStatusFlag.Z, false);

            Assert.False(p.HasFlag(ProcessorStatusFlag.E));
            Assert.False(p.HasFlag(ProcessorStatusFlag.N));
            Assert.False(p.HasFlag(ProcessorStatusFlag.V));
            Assert.False(p.HasFlag(ProcessorStatusFlag.M));
            Assert.False(p.HasFlag(ProcessorStatusFlag.X));
            Assert.False(p.HasFlag(ProcessorStatusFlag.D));
            Assert.False(p.HasFlag(ProcessorStatusFlag.I));
            Assert.False(p.HasFlag(ProcessorStatusFlag.Z));
            Assert.False(p.HasFlag(ProcessorStatusFlag.C));
        }
    }
}