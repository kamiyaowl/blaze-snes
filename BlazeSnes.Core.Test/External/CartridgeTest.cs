using System;
using System.IO;
using BlazeSnes.Core.External;
using BlazeSnes.Core.Cpu;
using Xunit;
using Xunit.Sdk;

namespace BlazeSnes.Core.Test.External {
    public class CartridgeTest {
        static readonly string SAMPLE_PATH = @"../../../../assets/roms/helloworld/sample1.smc"; // TODO: もう少し賢くなるでしょ...

        /// <summary>
        /// GameTitle, CheckSumで期待値が取得できるか確認する
        /// </summary>
        [Fact]
        public void ReadSampleRom () {
            using (var fs = new FileStream (SAMPLE_PATH, FileMode.Open)) {
                var c = new Cartridge (fs);
                Assert.Equal ("SAMPLE1              ", c.GameTitle);
                Assert.Equal (0x737f, c.CheckSumComplement);
                Assert.Equal (0x8c80, c.CheckSum);
            }
        }

        /// <summary>
        /// 期待通りのReset Vectorが取得できるか確認する
        /// </summary>
        [Fact]
        public void ReadSampleInterruptVector () {
            using (var fs = new FileStream (SAMPLE_PATH, FileMode.Open)) {
                var c = new Cartridge (fs);
                var vector = c.CreateInterruptVector();
                Assert.Equal (0x0000, vector.CopAddr(false));
                Assert.Equal (0x0000, vector.CopAddr(true));
                Assert.Equal (0x0000, vector.BreakAddr(false));
                Assert.Equal (0x0000, vector.BreakAddr(true));
                Assert.Equal (0x0000, vector.AbortAddr(false));
                Assert.Equal (0x0000, vector.AbortAddr(true));
                Assert.Equal (0x0000, vector.NmiAddr(false));
                Assert.Equal (0x0000, vector.NmiAddr(true));
                Assert.Equal (0x0000, vector.ResetAddr(false));
                Assert.Equal (0xa20e, vector.ResetAddr(true)); // HelloのSampleはEmulationのResetしか定義していない
                Assert.Equal (0x0000, vector.IrqAddr(false));
                Assert.Equal (0x0000, vector.IrqAddr(true));
            }

        }
    }
}