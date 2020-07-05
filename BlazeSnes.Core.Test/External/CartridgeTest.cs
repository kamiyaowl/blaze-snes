using System;
using System.IO;

using BlazeSnes.Core.Cpu;
using BlazeSnes.Core.External;

using Xunit;
using Xunit.Sdk;

namespace BlazeSnes.Core.Test.External {
    public class CartridgeTest {
        static readonly string SAMPLE_PATH = @"../../../../assets/roms/helloworld/sample1.smc"; // TODO: もう少し賢くなるでしょ...

        /// <summary>
        /// GameTitle, CheckSumで期待値が取得できるか確認する
        /// </summary>
        [Fact]
        public void ReadSampleRom() {
            using (var fs = new FileStream(SAMPLE_PATH, FileMode.Open)) {
                var c = new Cartridge(fs);
                Assert.Equal("SAMPLE1              ", c.GameTitle);
                Assert.Equal(0x737f, c.CheckSumComplement);
                Assert.Equal(0x8c80, c.CheckSum);
                Assert.Equal(0xa20e, c.ResetAddrInEmulation); // HelloのSampleはEmulationのResetしか定義していない
                Assert.Equal(0x0000, c.BreakAddrInEmulation); // HelloのSampleはEmulationのResetしか定義していない
            }
        }
    }
}