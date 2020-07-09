using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using BlazeSnes.Core.Cpu;
using BlazeSnes.Core.External;

using Xunit;
using Xunit.Sdk;

namespace BlazeSnes.Core.Test.External {
    public class CartridgeTest {
        static readonly string SAMPLE_PATH = @"../../../../assets/roms/helloworld/sample1.smc";

        /// <summary>
        /// テストするROMのパス一覧
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<object[]> GetRomPathes() {
            yield return new object[]{ SAMPLE_PATH };
        }

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

        /// <summary>
        /// ROMの検査に成功して、binaryを展開できていることを確認します
        /// </summary>
        /// <param name="path"></param>
        [Theory, MemberData(nameof(GetRomPathes))]
        public void ReadRom(string path) {
            Cartridge cartridge;
            // read
            using (var fs = new FileStream(path, FileMode.Open)) {
                cartridge = new Cartridge(fs);
            }
            // verify
            var romBinary = File.ReadAllBytes(path);
            Assert.Equal(romBinary, cartridge.RomData);
        }
    }
}