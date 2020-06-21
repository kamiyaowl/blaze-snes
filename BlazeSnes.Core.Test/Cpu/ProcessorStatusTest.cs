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
            Assert.False(p.Flag.HasFlag(ProcessorStatus.Flags.E));
            Assert.False(p.Flag.HasFlag(ProcessorStatus.Flags.N));
            Assert.False(p.Flag.HasFlag(ProcessorStatus.Flags.V));
            Assert.False(p.Flag.HasFlag(ProcessorStatus.Flags.M));
            Assert.False(p.Flag.HasFlag(ProcessorStatus.Flags.X));
            Assert.False(p.Flag.HasFlag(ProcessorStatus.Flags.D));
            Assert.False(p.Flag.HasFlag(ProcessorStatus.Flags.I));
            Assert.False(p.Flag.HasFlag(ProcessorStatus.Flags.Z));
            Assert.False(p.Flag.HasFlag(ProcessorStatus.Flags.C));
        }

        /// <summary>
        /// Flag変数経由で値の設定が可能か確認
        /// </summary>
        [Fact]
        public void SetFromFlag () {
            var p = new ProcessorStatus();
            p.Flag |= ProcessorStatus.Flags.E;
            p.Flag |= ProcessorStatus.Flags.V;
            p.Flag |= ProcessorStatus.Flags.X;
            p.Flag |= ProcessorStatus.Flags.I;
            p.Flag |= ProcessorStatus.Flags.C;

            Assert.True(p.Flag.HasFlag(ProcessorStatus.Flags.E));
            Assert.False(p.Flag.HasFlag(ProcessorStatus.Flags.N));
            Assert.True(p.Flag.HasFlag(ProcessorStatus.Flags.V));
            Assert.False(p.Flag.HasFlag(ProcessorStatus.Flags.M));
            Assert.True(p.Flag.HasFlag(ProcessorStatus.Flags.X));
            Assert.False(p.Flag.HasFlag(ProcessorStatus.Flags.D));
            Assert.True(p.Flag.HasFlag(ProcessorStatus.Flags.I));
            Assert.False(p.Flag.HasFlag(ProcessorStatus.Flags.Z));
            Assert.True(p.Flag.HasFlag(ProcessorStatus.Flags.C));
        }
        /// <summary>
        /// Value変数経由で値の設定が可能か確認
        /// </summary>
        [Fact]
        public void SetFromValue () {
            var p = new ProcessorStatus();
            p.Value |= 0x8055;

            Assert.True(p.Flag.HasFlag(ProcessorStatus.Flags.E));
            Assert.False(p.Flag.HasFlag(ProcessorStatus.Flags.N));
            Assert.True(p.Flag.HasFlag(ProcessorStatus.Flags.V));
            Assert.False(p.Flag.HasFlag(ProcessorStatus.Flags.M));
            Assert.True(p.Flag.HasFlag(ProcessorStatus.Flags.X));
            Assert.False(p.Flag.HasFlag(ProcessorStatus.Flags.D));
            Assert.True(p.Flag.HasFlag(ProcessorStatus.Flags.I));
            Assert.False(p.Flag.HasFlag(ProcessorStatus.Flags.Z));
            Assert.True(p.Flag.HasFlag(ProcessorStatus.Flags.C));
        }
        /// <summary>
        /// Value変数経由で値の設定が可能か確認
        /// </summary>
        [Fact]
        public void MixedTest () {
            var p = new ProcessorStatus();
            // All set
            p.Value |= 0x00aa;
            p.Flag |= ProcessorStatus.Flags.E;
            p.Flag |= ProcessorStatus.Flags.V;
            p.Flag |= ProcessorStatus.Flags.X;
            p.Flag |= ProcessorStatus.Flags.I;
            p.Flag |= ProcessorStatus.Flags.C;

            Assert.True(p.Flag.HasFlag(ProcessorStatus.Flags.E));
            Assert.True(p.Flag.HasFlag(ProcessorStatus.Flags.N));
            Assert.True(p.Flag.HasFlag(ProcessorStatus.Flags.V));
            Assert.True(p.Flag.HasFlag(ProcessorStatus.Flags.M));
            Assert.True(p.Flag.HasFlag(ProcessorStatus.Flags.X));
            Assert.True(p.Flag.HasFlag(ProcessorStatus.Flags.D));
            Assert.True(p.Flag.HasFlag(ProcessorStatus.Flags.I));
            Assert.True(p.Flag.HasFlag(ProcessorStatus.Flags.Z));
            Assert.True(p.Flag.HasFlag(ProcessorStatus.Flags.C));

            // All clear
            p.Value &= 0x00aa;
            p.Flag &= ~(ProcessorStatus.Flags.N);
            p.Flag &= ~(ProcessorStatus.Flags.M);
            p.Flag &= ~(ProcessorStatus.Flags.D);
            p.Flag &= ~(ProcessorStatus.Flags.Z);

            Assert.False(p.Flag.HasFlag(ProcessorStatus.Flags.E));
            Assert.False(p.Flag.HasFlag(ProcessorStatus.Flags.N));
            Assert.False(p.Flag.HasFlag(ProcessorStatus.Flags.V));
            Assert.False(p.Flag.HasFlag(ProcessorStatus.Flags.M));
            Assert.False(p.Flag.HasFlag(ProcessorStatus.Flags.X));
            Assert.False(p.Flag.HasFlag(ProcessorStatus.Flags.D));
            Assert.False(p.Flag.HasFlag(ProcessorStatus.Flags.I));
            Assert.False(p.Flag.HasFlag(ProcessorStatus.Flags.Z));
            Assert.False(p.Flag.HasFlag(ProcessorStatus.Flags.C));
        }

    }
}