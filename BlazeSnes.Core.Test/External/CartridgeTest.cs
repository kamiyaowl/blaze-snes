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
        /// 最低限のROM Dataを作ります。CheckSumにしか値がセットされていません。
        /// </summary>
        /// <param name="isLowRom"></param>
        /// <param name="hasHeader"></param>
        /// <returns></returns>
        public static byte[] CreateRomData(bool isLowRom, bool hasHeader) {
            var headerOffset = hasHeader ? Cartridge.EXTRA_HEADER_SIZE : 0;
            var src = new byte[headerOffset + (isLowRom ? 0x8000 : 0x10000)];
            var checkSumBaseAddr = headerOffset + (isLowRom ? Cartridge.LOROM_OFFSET : Cartridge.HIROM_OFFSET) + 0x2e;
            src[headerOffset + checkSumBaseAddr] = 0xff;
            src[headerOffset + checkSumBaseAddr + 1] = 0xff;
            return src;
        }

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

        /// <summary>
        /// ConvertToLocalAddr のテスト期待値
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<object[]> GetConvertToLocalAddrParams() {
            // LoRom
            yield return new object[]{ true, 0x00_8000, Cartridge.TargetDevice.Rom, 0x000000 };
            //TODO: 一通り作る #78
            // HiRom
            yield return new object[]{ false, 0x00_8000, Cartridge.TargetDevice.Rom, 0x000000 };
            //TODO: 一通り作る #78
        }

        /// <summary>
        /// Cartridge内のアドレス変換を検証します
        /// </summary>
        /// <param name="isLowRom"></param>
        /// <param name="addr"></param>
        /// <param name="expectTarget"></param>
        /// <param name="expectLocalAddr"></param>
        [Theory, MemberData(nameof(GetConvertToLocalAddrParams))]
        public void ConvertToLocalAddr(bool isLowRom, uint addr, Cartridge.TargetDevice expectTarget, uint expectLocalAddr) {
            // CheckSum/CheckSumComplementが一致するようなデータを作る
            Cartridge cartridge;
            var romData = CreateRomData(isLowRom, false);
            using(var stream = new MemoryStream(romData)) {
                cartridge = new Cartridge(stream);
            }
            // 一応ROM Typeが一致することを確認
            Assert.Equal(isLowRom, cartridge.IsLoRom);
            Assert.False(cartridge.HasHeaderOffset);
            // 変換する関数だけ叩いて期待値チェック
            var (target, localAddr) = cartridge.ConvertToLocalAddr(addr);
            Assert.Equal(expectTarget, target);
            Assert.Equal(expectLocalAddr, localAddr);
        }
    }
}