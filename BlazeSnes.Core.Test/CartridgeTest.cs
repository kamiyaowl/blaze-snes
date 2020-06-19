using BlazeSnes.Core;
using System;
using System.IO;
using Xunit;
using Xunit.Sdk;

namespace BlazeSnes.Core.Test {
    public class CartridgeTest {
        [Fact]
        public void ReadSampleRom() {
            const string path = @"../../../../assets/roms/helloworld/sample1.smc"; // TODO: もう少し賢くなるでしょ...
            using (var fs = new FileStream(path, FileMode.Open)) {
                var c = new Cartridge(fs);
                Assert.Equal("SAMPLE1              ", c.GameTitle);
                Assert.Equal(0x737f, c.CheckSumComplement);
                Assert.Equal(0x8c80, c.CheckSum);
                Assert.Equal(0xa20e, c.ResetAddrInEmulation); // SampleではEmulation Resetしか定義してない
            }
        }
    }
}