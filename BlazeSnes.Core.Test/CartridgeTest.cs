using System;
using System.IO;
using Xunit;
using Xunit.Sdk;
using BlazeSnes.Core;

namespace BlazeSnes.Core.Test {
    public class CartridgeTest {
        [Fact]
        public void ReadSampleRom() {
            const string path = @"../../../../assets/roms/helloworld/sample1.smc"; // TODO: もう少し賢くなるでしょ...
            using(var fs = new FileStream(path, FileMode.Open)) {
                var c = new Cartridge(fs);
                Assert.Equal(c.GameTitle, "SAMPLE1              ");
                Assert.Equal(c.CheckSumComplement, 0x737f);
                Assert.Equal(c.CheckSum, 0x8c80);
            }
        }
    }
}
