using System;
using System.IO;
using Xunit;
using Xunit.Sdk;
using BlazeSnes.Core;

namespace BlazeSnes.Core.Test {
    public class CartridgeTest {
        [Fact]
        public void ReadRom() {
            const string path = @"../../../../assets/roms/helloworld/sample1.smc"; // TODO: もう少し賢くなるでしょ...
            using(var reader = new StreamReader(path)) {
                if (Cartridge.TryParse(reader, out var dst)) {
                    // TODO: 期待値確認
                } else {
                    throw new XunitException($"Cartridge.TryParse failed. path={path}");
                }
            }
        }
    }
}
