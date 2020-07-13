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
            // LoRom 00-3f: 8000-ffff
            yield return new object[]{ true, 0x00_8000, Cartridge.TargetDevice.Rom, 0x000000 };
            yield return new object[]{ true, 0x00_800a, Cartridge.TargetDevice.Rom, 0x00000a };
            yield return new object[]{ true, 0x00_80ba, Cartridge.TargetDevice.Rom, 0x0000ba };
            yield return new object[]{ true, 0x00_8cba, Cartridge.TargetDevice.Rom, 0x000cba };
            yield return new object[]{ true, 0x00_ffff, Cartridge.TargetDevice.Rom, 0x007fff };
            yield return new object[]{ true, 0x01_8000, Cartridge.TargetDevice.Rom, 0x008000 };
            yield return new object[]{ true, 0x01_ffff, Cartridge.TargetDevice.Rom, 0x00ffff };
            yield return new object[]{ true, 0x02_8000, Cartridge.TargetDevice.Rom, 0x010000 };
            yield return new object[]{ true, 0x02_ffff, Cartridge.TargetDevice.Rom, 0x017fff };
            yield return new object[]{ true, 0x03_8000, Cartridge.TargetDevice.Rom, 0x018000 };
            yield return new object[]{ true, 0x03_ffff, Cartridge.TargetDevice.Rom, 0x01ffff };
            yield return new object[]{ true, 0x3d_8000, Cartridge.TargetDevice.Rom, 0x1e8000 };
            yield return new object[]{ true, 0x3d_ffff, Cartridge.TargetDevice.Rom, 0x1effff };
            yield return new object[]{ true, 0x3e_8000, Cartridge.TargetDevice.Rom, 0x1f0000 };
            yield return new object[]{ true, 0x3e_ffff, Cartridge.TargetDevice.Rom, 0x1f7fff };
            yield return new object[]{ true, 0x3f_8000, Cartridge.TargetDevice.Rom, 0x1f8000 };
            yield return new object[]{ true, 0x3f_fffe, Cartridge.TargetDevice.Rom, 0x1ffffe };
            yield return new object[]{ true, 0x3f_fcde, Cartridge.TargetDevice.Rom, 0x1ffcde };
            yield return new object[]{ true, 0x3f_ffff, Cartridge.TargetDevice.Rom, 0x1fffff };
            // LoRom 80-bf: 8000-ffff
            yield return new object[]{ true, 0x80_8000, Cartridge.TargetDevice.Rom, 0x000000 };
            yield return new object[]{ true, 0x80_800a, Cartridge.TargetDevice.Rom, 0x00000a };
            yield return new object[]{ true, 0x80_80ba, Cartridge.TargetDevice.Rom, 0x0000ba };
            yield return new object[]{ true, 0x80_8cba, Cartridge.TargetDevice.Rom, 0x000cba };
            yield return new object[]{ true, 0x80_ffff, Cartridge.TargetDevice.Rom, 0x007fff };
            yield return new object[]{ true, 0x81_8000, Cartridge.TargetDevice.Rom, 0x008000 };
            yield return new object[]{ true, 0x81_ffff, Cartridge.TargetDevice.Rom, 0x00ffff };
            yield return new object[]{ true, 0x82_8000, Cartridge.TargetDevice.Rom, 0x010000 };
            yield return new object[]{ true, 0x82_ffff, Cartridge.TargetDevice.Rom, 0x017fff };
            yield return new object[]{ true, 0x83_8000, Cartridge.TargetDevice.Rom, 0x018000 };
            yield return new object[]{ true, 0x83_ffff, Cartridge.TargetDevice.Rom, 0x01ffff };
            yield return new object[]{ true, 0xbd_8000, Cartridge.TargetDevice.Rom, 0x1e8000 };
            yield return new object[]{ true, 0xbd_ffff, Cartridge.TargetDevice.Rom, 0x1effff };
            yield return new object[]{ true, 0xbe_8000, Cartridge.TargetDevice.Rom, 0x1f0000 };
            yield return new object[]{ true, 0xbe_ffff, Cartridge.TargetDevice.Rom, 0x1f7fff };
            yield return new object[]{ true, 0xbf_8000, Cartridge.TargetDevice.Rom, 0x1f8000 };
            yield return new object[]{ true, 0xbf_fffe, Cartridge.TargetDevice.Rom, 0x1ffffe };
            yield return new object[]{ true, 0xbf_fcde, Cartridge.TargetDevice.Rom, 0x1ffcde };
            yield return new object[]{ true, 0xbf_ffff, Cartridge.TargetDevice.Rom, 0x1fffff };
            // LoRom 40-6f: 0000-7fff
            yield return new object[]{ true, 0x40_0000, Cartridge.TargetDevice.Rom, 0x200000 };
            yield return new object[]{ true, 0x40_000a, Cartridge.TargetDevice.Rom, 0x20000a };
            yield return new object[]{ true, 0x40_00ba, Cartridge.TargetDevice.Rom, 0x2000ba };
            yield return new object[]{ true, 0x40_0cba, Cartridge.TargetDevice.Rom, 0x200cba };
            yield return new object[]{ true, 0x40_7fff, Cartridge.TargetDevice.Rom, 0x207fff };
            yield return new object[]{ true, 0x41_0000, Cartridge.TargetDevice.Rom, 0x208000 };
            yield return new object[]{ true, 0x41_7fff, Cartridge.TargetDevice.Rom, 0x20ffff };
            yield return new object[]{ true, 0x42_0000, Cartridge.TargetDevice.Rom, 0x210000 };
            yield return new object[]{ true, 0x42_7fff, Cartridge.TargetDevice.Rom, 0x217fff };
            yield return new object[]{ true, 0x43_0000, Cartridge.TargetDevice.Rom, 0x218000 };
            yield return new object[]{ true, 0x43_7fff, Cartridge.TargetDevice.Rom, 0x21ffff };
            yield return new object[]{ true, 0x6d_0000, Cartridge.TargetDevice.Rom, 0x368000 };
            yield return new object[]{ true, 0x6d_7fff, Cartridge.TargetDevice.Rom, 0x36ffff };
            yield return new object[]{ true, 0x6e_0000, Cartridge.TargetDevice.Rom, 0x370000 };
            yield return new object[]{ true, 0x6e_7fff, Cartridge.TargetDevice.Rom, 0x377fff };
            yield return new object[]{ true, 0x6f_0000, Cartridge.TargetDevice.Rom, 0x378000 };
            yield return new object[]{ true, 0x6f_7ffe, Cartridge.TargetDevice.Rom, 0x37fffe };
            yield return new object[]{ true, 0x6f_7cde, Cartridge.TargetDevice.Rom, 0x37fcde };
            yield return new object[]{ true, 0x6f_7fff, Cartridge.TargetDevice.Rom, 0x37ffff };
            // LoRom c0-ef: 0000-7fff
            yield return new object[]{ true, 0xc0_0000, Cartridge.TargetDevice.Rom, 0x200000 };
            yield return new object[]{ true, 0xc0_000a, Cartridge.TargetDevice.Rom, 0x20000a };
            yield return new object[]{ true, 0xc0_00ba, Cartridge.TargetDevice.Rom, 0x2000ba };
            yield return new object[]{ true, 0xc0_0cba, Cartridge.TargetDevice.Rom, 0x200cba };
            yield return new object[]{ true, 0xc0_7fff, Cartridge.TargetDevice.Rom, 0x207fff };
            yield return new object[]{ true, 0xc1_0000, Cartridge.TargetDevice.Rom, 0x208000 };
            yield return new object[]{ true, 0xc1_7fff, Cartridge.TargetDevice.Rom, 0x20ffff };
            yield return new object[]{ true, 0xc2_0000, Cartridge.TargetDevice.Rom, 0x210000 };
            yield return new object[]{ true, 0xc2_7fff, Cartridge.TargetDevice.Rom, 0x217fff };
            yield return new object[]{ true, 0xc3_0000, Cartridge.TargetDevice.Rom, 0x218000 };
            yield return new object[]{ true, 0xc3_7fff, Cartridge.TargetDevice.Rom, 0x21ffff };
            yield return new object[]{ true, 0xed_0000, Cartridge.TargetDevice.Rom, 0x368000 };
            yield return new object[]{ true, 0xed_7fff, Cartridge.TargetDevice.Rom, 0x36ffff };
            yield return new object[]{ true, 0xee_0000, Cartridge.TargetDevice.Rom, 0x370000 };
            yield return new object[]{ true, 0xee_7fff, Cartridge.TargetDevice.Rom, 0x377fff };
            yield return new object[]{ true, 0xef_0000, Cartridge.TargetDevice.Rom, 0x378000 };
            yield return new object[]{ true, 0xef_7ffe, Cartridge.TargetDevice.Rom, 0x37fffe };
            yield return new object[]{ true, 0xef_7cde, Cartridge.TargetDevice.Rom, 0x37fcde };
            yield return new object[]{ true, 0xef_7fff, Cartridge.TargetDevice.Rom, 0x37ffff };
            // LoRom 70-7d: 0000-7fff
            yield return new object[]{ true, 0x70_0000, Cartridge.TargetDevice.Mode20Sram1, 0x000000 };
            yield return new object[]{ true, 0x70_000a, Cartridge.TargetDevice.Mode20Sram1, 0x00000a };
            yield return new object[]{ true, 0x70_00ba, Cartridge.TargetDevice.Mode20Sram1, 0x0000ba };
            yield return new object[]{ true, 0x70_0cba, Cartridge.TargetDevice.Mode20Sram1, 0x000cba };
            yield return new object[]{ true, 0x70_7fff, Cartridge.TargetDevice.Mode20Sram1, 0x007fff };
            yield return new object[]{ true, 0x71_0000, Cartridge.TargetDevice.Mode20Sram1, 0x008000 };
            yield return new object[]{ true, 0x71_7fff, Cartridge.TargetDevice.Mode20Sram1, 0x00ffff };
            yield return new object[]{ true, 0x72_0000, Cartridge.TargetDevice.Mode20Sram1, 0x010000 };
            yield return new object[]{ true, 0x72_7fff, Cartridge.TargetDevice.Mode20Sram1, 0x017fff };
            yield return new object[]{ true, 0x7c_0000, Cartridge.TargetDevice.Mode20Sram1, 0x060000 };
            yield return new object[]{ true, 0x7c_7fff, Cartridge.TargetDevice.Mode20Sram1, 0x067fff };
            yield return new object[]{ true, 0x7d_0000, Cartridge.TargetDevice.Mode20Sram1, 0x068000 };
            yield return new object[]{ true, 0x7d_7ffe, Cartridge.TargetDevice.Mode20Sram1, 0x06fffe };
            yield return new object[]{ true, 0x7d_7fff, Cartridge.TargetDevice.Mode20Sram1, 0x06ffff };
            // LoRom f0-fd: 0000-7fff
            yield return new object[]{ true, 0xf0_0000, Cartridge.TargetDevice.Mode20Sram1, 0x000000 };
            yield return new object[]{ true, 0xf0_000a, Cartridge.TargetDevice.Mode20Sram1, 0x00000a };
            yield return new object[]{ true, 0xf0_00ba, Cartridge.TargetDevice.Mode20Sram1, 0x0000ba };
            yield return new object[]{ true, 0xf0_0cba, Cartridge.TargetDevice.Mode20Sram1, 0x000cba };
            yield return new object[]{ true, 0xf0_7fff, Cartridge.TargetDevice.Mode20Sram1, 0x007fff };
            yield return new object[]{ true, 0xf1_0000, Cartridge.TargetDevice.Mode20Sram1, 0x008000 };
            yield return new object[]{ true, 0xf1_7fff, Cartridge.TargetDevice.Mode20Sram1, 0x00ffff };
            yield return new object[]{ true, 0xf2_0000, Cartridge.TargetDevice.Mode20Sram1, 0x010000 };
            yield return new object[]{ true, 0xf2_7fff, Cartridge.TargetDevice.Mode20Sram1, 0x017fff };
            yield return new object[]{ true, 0xfc_0000, Cartridge.TargetDevice.Mode20Sram1, 0x060000 };
            yield return new object[]{ true, 0xfc_7fff, Cartridge.TargetDevice.Mode20Sram1, 0x067fff };
            yield return new object[]{ true, 0xfd_0000, Cartridge.TargetDevice.Mode20Sram1, 0x068000 };
            yield return new object[]{ true, 0xfd_7ffe, Cartridge.TargetDevice.Mode20Sram1, 0x06fffe };
            yield return new object[]{ true, 0xfd_7fff, Cartridge.TargetDevice.Mode20Sram1, 0x06ffff };
            // LoRom 70-7d: 8000-ffff
            yield return new object[]{ true, 0x70_8000, Cartridge.TargetDevice.Rom, 0x380000 };
            yield return new object[]{ true, 0x70_800a, Cartridge.TargetDevice.Rom, 0x38000a };
            yield return new object[]{ true, 0x70_80ba, Cartridge.TargetDevice.Rom, 0x3800ba };
            yield return new object[]{ true, 0x70_8cba, Cartridge.TargetDevice.Rom, 0x380cba };
            yield return new object[]{ true, 0x70_ffff, Cartridge.TargetDevice.Rom, 0x387fff };
            yield return new object[]{ true, 0x71_8000, Cartridge.TargetDevice.Rom, 0x388000 };
            yield return new object[]{ true, 0x71_ffff, Cartridge.TargetDevice.Rom, 0x38ffff };
            yield return new object[]{ true, 0x72_8000, Cartridge.TargetDevice.Rom, 0x390000 };
            yield return new object[]{ true, 0x72_ffff, Cartridge.TargetDevice.Rom, 0x397fff };
            yield return new object[]{ true, 0x7b_8000, Cartridge.TargetDevice.Rom, 0x3d8000 };
            yield return new object[]{ true, 0x7b_ffff, Cartridge.TargetDevice.Rom, 0x3dffff };
            yield return new object[]{ true, 0x7c_8000, Cartridge.TargetDevice.Rom, 0x3e0000 };
            yield return new object[]{ true, 0x7c_ffff, Cartridge.TargetDevice.Rom, 0x3e7fff };
            yield return new object[]{ true, 0x7d_8000, Cartridge.TargetDevice.Rom, 0x3e8000 };
            yield return new object[]{ true, 0x7d_fffe, Cartridge.TargetDevice.Rom, 0x3efffe };
            yield return new object[]{ true, 0x7d_ffff, Cartridge.TargetDevice.Rom, 0x3effff };
            // LoRom f0-fd: 8000-ffff
            yield return new object[]{ true, 0xf0_8000, Cartridge.TargetDevice.Rom, 0x380000 };
            yield return new object[]{ true, 0xf0_800a, Cartridge.TargetDevice.Rom, 0x38000a };
            yield return new object[]{ true, 0xf0_80ba, Cartridge.TargetDevice.Rom, 0x3800ba };
            yield return new object[]{ true, 0xf0_8cba, Cartridge.TargetDevice.Rom, 0x380cba };
            yield return new object[]{ true, 0xf0_ffff, Cartridge.TargetDevice.Rom, 0x387fff };
            yield return new object[]{ true, 0xf1_8000, Cartridge.TargetDevice.Rom, 0x388000 };
            yield return new object[]{ true, 0xf1_ffff, Cartridge.TargetDevice.Rom, 0x38ffff };
            yield return new object[]{ true, 0xf2_8000, Cartridge.TargetDevice.Rom, 0x390000 };
            yield return new object[]{ true, 0xf2_ffff, Cartridge.TargetDevice.Rom, 0x397fff };
            yield return new object[]{ true, 0xfb_8000, Cartridge.TargetDevice.Rom, 0x3d8000 };
            yield return new object[]{ true, 0xfb_ffff, Cartridge.TargetDevice.Rom, 0x3dffff };
            yield return new object[]{ true, 0xfc_8000, Cartridge.TargetDevice.Rom, 0x3e0000 };
            yield return new object[]{ true, 0xfc_ffff, Cartridge.TargetDevice.Rom, 0x3e7fff };
            yield return new object[]{ true, 0xfd_8000, Cartridge.TargetDevice.Rom, 0x3e8000 };
            yield return new object[]{ true, 0xfd_fffe, Cartridge.TargetDevice.Rom, 0x3efffe };
            yield return new object[]{ true, 0xfd_ffff, Cartridge.TargetDevice.Rom, 0x3effff };
            // LoRom fe-ff: 0000-7fff
            yield return new object[]{ true, 0xfe_0000, Cartridge.TargetDevice.Mode20Sram2, 0x000000 };
            yield return new object[]{ true, 0xfe_000a, Cartridge.TargetDevice.Mode20Sram2, 0x00000a };
            yield return new object[]{ true, 0xfe_00ba, Cartridge.TargetDevice.Mode20Sram2, 0x0000ba };
            yield return new object[]{ true, 0xfe_0cba, Cartridge.TargetDevice.Mode20Sram2, 0x000cba };
            yield return new object[]{ true, 0xfe_7fff, Cartridge.TargetDevice.Mode20Sram2, 0x007fff };
            yield return new object[]{ true, 0xff_0000, Cartridge.TargetDevice.Mode20Sram2, 0x008000 };
            yield return new object[]{ true, 0xff_7ffe, Cartridge.TargetDevice.Mode20Sram2, 0x00fffe };
            yield return new object[]{ true, 0xff_7fff, Cartridge.TargetDevice.Mode20Sram2, 0x00ffff };
            // LoRom fe-ff: 8000-7fff
            yield return new object[]{ true, 0xfe_8000, Cartridge.TargetDevice.Rom, 0x3f0000 };
            yield return new object[]{ true, 0xfe_8001, Cartridge.TargetDevice.Rom, 0x3f0001 };
            yield return new object[]{ true, 0xfe_8021, Cartridge.TargetDevice.Rom, 0x3f0021 };
            yield return new object[]{ true, 0xfe_8321, Cartridge.TargetDevice.Rom, 0x3f0321 };
            yield return new object[]{ true, 0xfe_ffff, Cartridge.TargetDevice.Rom, 0x3f7fff };
            yield return new object[]{ true, 0xff_8000, Cartridge.TargetDevice.Rom, 0x3f8000 };
            yield return new object[]{ true, 0xff_fabc, Cartridge.TargetDevice.Rom, 0x3ffabc };
            yield return new object[]{ true, 0xff_fffe, Cartridge.TargetDevice.Rom, 0x3ffffe };
            yield return new object[]{ true, 0xff_ffff, Cartridge.TargetDevice.Rom, 0x3fffff };
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