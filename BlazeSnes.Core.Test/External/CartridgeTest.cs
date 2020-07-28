using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using BlazeSnes.Core.Common;
using BlazeSnes.Core.Cpu;
using BlazeSnes.Core.External;

using Xunit;
using Xunit.Sdk;

namespace BlazeSnes.Core.Test.External {
    public class CartridgeTest {

        static readonly string SAMPLE_PATH = @"../../../../assets/roms/helloworld/sample1.smc";

        /// <summary>
        /// GameTitle, CheckSumで期待値が取得できるか確認する
        /// </summary>
        [Fact]
        public void ReadSampleRom() {
            using (var fs = new FileStream(SAMPLE_PATH, FileMode.Open)) {
                var c = new Cartridge(fs, isRestricted: true);
                Assert.Equal("SAMPLE1              ", c.GameTitle);
                Assert.Equal(0x737f, c.CheckSumComplement);
                Assert.Equal(0x8c80, c.CheckSum);
                Assert.Equal(0xa20e, c.ResetAddrInEmulation); // HelloのSampleはEmulationのResetしか定義していない
                Assert.Equal(0x0000, c.BreakAddrInEmulation); // HelloのSampleはEmulationのResetしか定義していない
            }
        }
        /// <summary>
        /// テストするROMのパス一覧
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<object[]> GetRomPathes() {
            yield return new object[] { @"../../../../assets/roms/helloworld/sample1.smc", "SAMPLE1              ", true, 0x8c80 };
            yield return new object[] { @"../../../../assets/roms/SNES/CPUTest/CPU/ADC/CPUADC.sfc", "65816 CPU TEST ADC   ", true, 0x5343 };
            yield return new object[] { @"../../../../assets/roms/SNES/CPUTest/CPU/ASL/CPUASL.sfc", "65816 CPU TEST ASL   ", true, 0x5343 };
            yield return new object[] { @"../../../../assets/roms/SNES/CPUTest/CPU/BIT/CPUBIT.sfc", "65816 CPU TEST BIT   ", true, 0x5343 };
            yield return new object[] { @"../../../../assets/roms/SNES/CPUTest/CPU/BRA/CPUBRA.sfc", "65816 CPU TEST BRA   ", true, 0x5343 };
            yield return new object[] { @"../../../../assets/roms/SNES/CPUTest/CPU/CMP/CPUCMP.sfc", "65816 CPU TEST CMP   ", true, 0x5343 };
            yield return new object[] { @"../../../../assets/roms/SNES/CPUTest/CPU/DEC/CPUDEC.sfc", "65816 CPU TEST DEC   ", true, 0x5343 };
            yield return new object[] { @"../../../../assets/roms/SNES/CPUTest/CPU/EOR/CPUEOR.sfc", "65816 CPU TEST EOR   ", true, 0x5343 };
            yield return new object[] { @"../../../../assets/roms/SNES/CPUTest/CPU/INC/CPUINC.sfc", "65816 CPU TEST INC   ", true, 0x5343 };
            yield return new object[] { @"../../../../assets/roms/SNES/CPUTest/CPU/JMP/CPUJMP.sfc", "65816 CPU TEST JMP   ", true, 0x5343 };
            yield return new object[] { @"../../../../assets/roms/SNES/CPUTest/CPU/LDR/CPULDR.sfc", "65816 CPU TEST LDR   ", true, 0x5343 };
            yield return new object[] { @"../../../../assets/roms/SNES/CPUTest/CPU/LSR/CPULSR.sfc", "65816 CPU TEST LSR   ", true, 0x5343 };
            yield return new object[] { @"../../../../assets/roms/SNES/CPUTest/CPU/MOV/CPUMOV.sfc", "65816 CPU TEST MOV   ", true, 0x5343 };
            yield return new object[] { @"../../../../assets/roms/SNES/CPUTest/CPU/MSC/CPUMSC.sfc", "65816 CPU TEST MSC   ", true, 0x5343 };
            yield return new object[] { @"../../../../assets/roms/SNES/CPUTest/CPU/ORA/CPUORA.sfc", "65816 CPU TEST ORA   ", true, 0x5343 };
            yield return new object[] { @"../../../../assets/roms/SNES/CPUTest/CPU/PHL/CPUPHL.sfc", "65816 CPU TEST PHL   ", true, 0x5343 };
            yield return new object[] { @"../../../../assets/roms/SNES/CPUTest/CPU/PSR/CPUPSR.sfc", "65816 CPU TEST PSR   ", true, 0x5343 };
            yield return new object[] { @"../../../../assets/roms/SNES/CPUTest/CPU/RET/CPURET.sfc", "65816 CPU TEST RET   ", true, 0x5343 };
            yield return new object[] { @"../../../../assets/roms/SNES/CPUTest/CPU/ROL/CPUROL.sfc", "65816 CPU TEST ROL   ", true, 0x5343 };
            yield return new object[] { @"../../../../assets/roms/SNES/CPUTest/CPU/ROR/CPUROR.sfc", "65816 CPU TEST ROR   ", true, 0x5343 };
            yield return new object[] { @"../../../../assets/roms/SNES/CPUTest/CPU/SBC/CPUSBC.sfc", "65816 CPU TEST SBC   ", true, 0x5343 };
            yield return new object[] { @"../../../../assets/roms/SNES/CPUTest/CPU/STR/CPUSTR.sfc", "65816 CPU TEST STR   ", true, 0x5343 };
            yield return new object[] { @"../../../../assets/roms/SNES/CPUTest/CPU/TRN/CPUTRN.sfc", "65816 CPU TEST TRN   ", true, 0x5343 };
            yield return new object[] { @"../../../../assets/roms/SNES/CPUTest/CPU/AND/CPUAND.sfc", "65816 CPU TEST AND   ", true, 0x5343 };
        }

        /// <summary>
        /// ROMの検査に成功して、binaryを展開できていることを確認します
        /// </summary>
        /// <param name="path"></param>
        [Theory, MemberData(nameof(GetRomPathes))]
        public void ReadRom(string path, string gameTitle, bool isLoRom, ushort checkSum) {
            Cartridge cartridge;
            // read
            using (var fs = new FileStream(path, FileMode.Open)) {
                cartridge = new Cartridge(fs, isRestricted: false);
            }
            // verify
            Assert.Equal(gameTitle, cartridge.GameTitle);
            Assert.Equal(isLoRom, cartridge.IsLoRom);
            Assert.Equal(checkSum, cartridge.CheckSum);

            // ROM size比較もしておく
            var romBinary = File.ReadAllBytes(path);
            Assert.Equal(romBinary, cartridge.RomData.Take(romBinary.Length)); // Lo/Hi関わらず最大サイズでMapしてあるので比較範囲は狭めておく
        }

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
        /// ConvertToLocalAddr のテスト期待値
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<object[]> GetConvertToLocalAddrParams() {
            // LoRom 00-3f: 8000-ffff
            yield return new object[] { true, 0x00_8000, Cartridge.TargetDevice.Rom, 0x000000 };
            yield return new object[] { true, 0x00_800a, Cartridge.TargetDevice.Rom, 0x00000a };
            yield return new object[] { true, 0x00_80ba, Cartridge.TargetDevice.Rom, 0x0000ba };
            yield return new object[] { true, 0x00_8cba, Cartridge.TargetDevice.Rom, 0x000cba };
            yield return new object[] { true, 0x00_ffff, Cartridge.TargetDevice.Rom, 0x007fff };
            yield return new object[] { true, 0x01_8000, Cartridge.TargetDevice.Rom, 0x008000 };
            yield return new object[] { true, 0x01_ffff, Cartridge.TargetDevice.Rom, 0x00ffff };
            yield return new object[] { true, 0x02_8000, Cartridge.TargetDevice.Rom, 0x010000 };
            yield return new object[] { true, 0x02_ffff, Cartridge.TargetDevice.Rom, 0x017fff };
            yield return new object[] { true, 0x03_8000, Cartridge.TargetDevice.Rom, 0x018000 };
            yield return new object[] { true, 0x03_ffff, Cartridge.TargetDevice.Rom, 0x01ffff };
            yield return new object[] { true, 0x3d_8000, Cartridge.TargetDevice.Rom, 0x1e8000 };
            yield return new object[] { true, 0x3d_ffff, Cartridge.TargetDevice.Rom, 0x1effff };
            yield return new object[] { true, 0x3e_8000, Cartridge.TargetDevice.Rom, 0x1f0000 };
            yield return new object[] { true, 0x3e_ffff, Cartridge.TargetDevice.Rom, 0x1f7fff };
            yield return new object[] { true, 0x3f_8000, Cartridge.TargetDevice.Rom, 0x1f8000 };
            yield return new object[] { true, 0x3f_fffe, Cartridge.TargetDevice.Rom, 0x1ffffe };
            yield return new object[] { true, 0x3f_fcde, Cartridge.TargetDevice.Rom, 0x1ffcde };
            yield return new object[] { true, 0x3f_ffff, Cartridge.TargetDevice.Rom, 0x1fffff };
            // LoRom 80-bf: 8000-ffff
            yield return new object[] { true, 0x80_8000, Cartridge.TargetDevice.Rom, 0x000000 };
            yield return new object[] { true, 0x80_800a, Cartridge.TargetDevice.Rom, 0x00000a };
            yield return new object[] { true, 0x80_80ba, Cartridge.TargetDevice.Rom, 0x0000ba };
            yield return new object[] { true, 0x80_8cba, Cartridge.TargetDevice.Rom, 0x000cba };
            yield return new object[] { true, 0x80_ffff, Cartridge.TargetDevice.Rom, 0x007fff };
            yield return new object[] { true, 0x81_8000, Cartridge.TargetDevice.Rom, 0x008000 };
            yield return new object[] { true, 0x81_ffff, Cartridge.TargetDevice.Rom, 0x00ffff };
            yield return new object[] { true, 0x82_8000, Cartridge.TargetDevice.Rom, 0x010000 };
            yield return new object[] { true, 0x82_ffff, Cartridge.TargetDevice.Rom, 0x017fff };
            yield return new object[] { true, 0x83_8000, Cartridge.TargetDevice.Rom, 0x018000 };
            yield return new object[] { true, 0x83_ffff, Cartridge.TargetDevice.Rom, 0x01ffff };
            yield return new object[] { true, 0xbd_8000, Cartridge.TargetDevice.Rom, 0x1e8000 };
            yield return new object[] { true, 0xbd_ffff, Cartridge.TargetDevice.Rom, 0x1effff };
            yield return new object[] { true, 0xbe_8000, Cartridge.TargetDevice.Rom, 0x1f0000 };
            yield return new object[] { true, 0xbe_ffff, Cartridge.TargetDevice.Rom, 0x1f7fff };
            yield return new object[] { true, 0xbf_8000, Cartridge.TargetDevice.Rom, 0x1f8000 };
            yield return new object[] { true, 0xbf_fffe, Cartridge.TargetDevice.Rom, 0x1ffffe };
            yield return new object[] { true, 0xbf_fcde, Cartridge.TargetDevice.Rom, 0x1ffcde };
            yield return new object[] { true, 0xbf_ffff, Cartridge.TargetDevice.Rom, 0x1fffff };
            // LoRom 40-6f: 0000-7fff
            yield return new object[] { true, 0x40_0000, Cartridge.TargetDevice.Rom, 0x200000 };
            yield return new object[] { true, 0x40_000a, Cartridge.TargetDevice.Rom, 0x20000a };
            yield return new object[] { true, 0x40_00ba, Cartridge.TargetDevice.Rom, 0x2000ba };
            yield return new object[] { true, 0x40_0cba, Cartridge.TargetDevice.Rom, 0x200cba };
            yield return new object[] { true, 0x40_7fff, Cartridge.TargetDevice.Rom, 0x207fff };
            yield return new object[] { true, 0x41_0000, Cartridge.TargetDevice.Rom, 0x208000 };
            yield return new object[] { true, 0x41_7fff, Cartridge.TargetDevice.Rom, 0x20ffff };
            yield return new object[] { true, 0x42_0000, Cartridge.TargetDevice.Rom, 0x210000 };
            yield return new object[] { true, 0x42_7fff, Cartridge.TargetDevice.Rom, 0x217fff };
            yield return new object[] { true, 0x43_0000, Cartridge.TargetDevice.Rom, 0x218000 };
            yield return new object[] { true, 0x43_7fff, Cartridge.TargetDevice.Rom, 0x21ffff };
            yield return new object[] { true, 0x6d_0000, Cartridge.TargetDevice.Rom, 0x368000 };
            yield return new object[] { true, 0x6d_7fff, Cartridge.TargetDevice.Rom, 0x36ffff };
            yield return new object[] { true, 0x6e_0000, Cartridge.TargetDevice.Rom, 0x370000 };
            yield return new object[] { true, 0x6e_7fff, Cartridge.TargetDevice.Rom, 0x377fff };
            yield return new object[] { true, 0x6f_0000, Cartridge.TargetDevice.Rom, 0x378000 };
            yield return new object[] { true, 0x6f_7ffe, Cartridge.TargetDevice.Rom, 0x37fffe };
            yield return new object[] { true, 0x6f_7cde, Cartridge.TargetDevice.Rom, 0x37fcde };
            yield return new object[] { true, 0x6f_7fff, Cartridge.TargetDevice.Rom, 0x37ffff };
            // LoRom c0-ef: 0000-7fff
            yield return new object[] { true, 0xc0_0000, Cartridge.TargetDevice.Rom, 0x200000 };
            yield return new object[] { true, 0xc0_000a, Cartridge.TargetDevice.Rom, 0x20000a };
            yield return new object[] { true, 0xc0_00ba, Cartridge.TargetDevice.Rom, 0x2000ba };
            yield return new object[] { true, 0xc0_0cba, Cartridge.TargetDevice.Rom, 0x200cba };
            yield return new object[] { true, 0xc0_7fff, Cartridge.TargetDevice.Rom, 0x207fff };
            yield return new object[] { true, 0xc1_0000, Cartridge.TargetDevice.Rom, 0x208000 };
            yield return new object[] { true, 0xc1_7fff, Cartridge.TargetDevice.Rom, 0x20ffff };
            yield return new object[] { true, 0xc2_0000, Cartridge.TargetDevice.Rom, 0x210000 };
            yield return new object[] { true, 0xc2_7fff, Cartridge.TargetDevice.Rom, 0x217fff };
            yield return new object[] { true, 0xc3_0000, Cartridge.TargetDevice.Rom, 0x218000 };
            yield return new object[] { true, 0xc3_7fff, Cartridge.TargetDevice.Rom, 0x21ffff };
            yield return new object[] { true, 0xed_0000, Cartridge.TargetDevice.Rom, 0x368000 };
            yield return new object[] { true, 0xed_7fff, Cartridge.TargetDevice.Rom, 0x36ffff };
            yield return new object[] { true, 0xee_0000, Cartridge.TargetDevice.Rom, 0x370000 };
            yield return new object[] { true, 0xee_7fff, Cartridge.TargetDevice.Rom, 0x377fff };
            yield return new object[] { true, 0xef_0000, Cartridge.TargetDevice.Rom, 0x378000 };
            yield return new object[] { true, 0xef_7ffe, Cartridge.TargetDevice.Rom, 0x37fffe };
            yield return new object[] { true, 0xef_7cde, Cartridge.TargetDevice.Rom, 0x37fcde };
            yield return new object[] { true, 0xef_7fff, Cartridge.TargetDevice.Rom, 0x37ffff };
            // LoRom 70-7d: 0000-7fff
            yield return new object[] { true, 0x70_0000, Cartridge.TargetDevice.Mode20Sram1, 0x000000 };
            yield return new object[] { true, 0x70_000a, Cartridge.TargetDevice.Mode20Sram1, 0x00000a };
            yield return new object[] { true, 0x70_00ba, Cartridge.TargetDevice.Mode20Sram1, 0x0000ba };
            yield return new object[] { true, 0x70_0cba, Cartridge.TargetDevice.Mode20Sram1, 0x000cba };
            yield return new object[] { true, 0x70_7fff, Cartridge.TargetDevice.Mode20Sram1, 0x007fff };
            yield return new object[] { true, 0x71_0000, Cartridge.TargetDevice.Mode20Sram1, 0x008000 };
            yield return new object[] { true, 0x71_7fff, Cartridge.TargetDevice.Mode20Sram1, 0x00ffff };
            yield return new object[] { true, 0x72_0000, Cartridge.TargetDevice.Mode20Sram1, 0x010000 };
            yield return new object[] { true, 0x72_7fff, Cartridge.TargetDevice.Mode20Sram1, 0x017fff };
            yield return new object[] { true, 0x7c_0000, Cartridge.TargetDevice.Mode20Sram1, 0x060000 };
            yield return new object[] { true, 0x7c_7fff, Cartridge.TargetDevice.Mode20Sram1, 0x067fff };
            yield return new object[] { true, 0x7d_0000, Cartridge.TargetDevice.Mode20Sram1, 0x068000 };
            yield return new object[] { true, 0x7d_7ffe, Cartridge.TargetDevice.Mode20Sram1, 0x06fffe };
            yield return new object[] { true, 0x7d_7fff, Cartridge.TargetDevice.Mode20Sram1, 0x06ffff };
            // LoRom f0-fd: 0000-7fff
            yield return new object[] { true, 0xf0_0000, Cartridge.TargetDevice.Mode20Sram1, 0x000000 };
            yield return new object[] { true, 0xf0_000a, Cartridge.TargetDevice.Mode20Sram1, 0x00000a };
            yield return new object[] { true, 0xf0_00ba, Cartridge.TargetDevice.Mode20Sram1, 0x0000ba };
            yield return new object[] { true, 0xf0_0cba, Cartridge.TargetDevice.Mode20Sram1, 0x000cba };
            yield return new object[] { true, 0xf0_7fff, Cartridge.TargetDevice.Mode20Sram1, 0x007fff };
            yield return new object[] { true, 0xf1_0000, Cartridge.TargetDevice.Mode20Sram1, 0x008000 };
            yield return new object[] { true, 0xf1_7fff, Cartridge.TargetDevice.Mode20Sram1, 0x00ffff };
            yield return new object[] { true, 0xf2_0000, Cartridge.TargetDevice.Mode20Sram1, 0x010000 };
            yield return new object[] { true, 0xf2_7fff, Cartridge.TargetDevice.Mode20Sram1, 0x017fff };
            yield return new object[] { true, 0xfc_0000, Cartridge.TargetDevice.Mode20Sram1, 0x060000 };
            yield return new object[] { true, 0xfc_7fff, Cartridge.TargetDevice.Mode20Sram1, 0x067fff };
            yield return new object[] { true, 0xfd_0000, Cartridge.TargetDevice.Mode20Sram1, 0x068000 };
            yield return new object[] { true, 0xfd_7ffe, Cartridge.TargetDevice.Mode20Sram1, 0x06fffe };
            yield return new object[] { true, 0xfd_7fff, Cartridge.TargetDevice.Mode20Sram1, 0x06ffff };
            // LoRom 70-7d: 8000-ffff
            yield return new object[] { true, 0x70_8000, Cartridge.TargetDevice.Rom, 0x380000 };
            yield return new object[] { true, 0x70_800a, Cartridge.TargetDevice.Rom, 0x38000a };
            yield return new object[] { true, 0x70_80ba, Cartridge.TargetDevice.Rom, 0x3800ba };
            yield return new object[] { true, 0x70_8cba, Cartridge.TargetDevice.Rom, 0x380cba };
            yield return new object[] { true, 0x70_ffff, Cartridge.TargetDevice.Rom, 0x387fff };
            yield return new object[] { true, 0x71_8000, Cartridge.TargetDevice.Rom, 0x388000 };
            yield return new object[] { true, 0x71_ffff, Cartridge.TargetDevice.Rom, 0x38ffff };
            yield return new object[] { true, 0x72_8000, Cartridge.TargetDevice.Rom, 0x390000 };
            yield return new object[] { true, 0x72_ffff, Cartridge.TargetDevice.Rom, 0x397fff };
            yield return new object[] { true, 0x7b_8000, Cartridge.TargetDevice.Rom, 0x3d8000 };
            yield return new object[] { true, 0x7b_ffff, Cartridge.TargetDevice.Rom, 0x3dffff };
            yield return new object[] { true, 0x7c_8000, Cartridge.TargetDevice.Rom, 0x3e0000 };
            yield return new object[] { true, 0x7c_ffff, Cartridge.TargetDevice.Rom, 0x3e7fff };
            yield return new object[] { true, 0x7d_8000, Cartridge.TargetDevice.Rom, 0x3e8000 };
            yield return new object[] { true, 0x7d_fffe, Cartridge.TargetDevice.Rom, 0x3efffe };
            yield return new object[] { true, 0x7d_ffff, Cartridge.TargetDevice.Rom, 0x3effff };
            // LoRom f0-fd: 8000-ffff
            yield return new object[] { true, 0xf0_8000, Cartridge.TargetDevice.Rom, 0x380000 };
            yield return new object[] { true, 0xf0_800a, Cartridge.TargetDevice.Rom, 0x38000a };
            yield return new object[] { true, 0xf0_80ba, Cartridge.TargetDevice.Rom, 0x3800ba };
            yield return new object[] { true, 0xf0_8cba, Cartridge.TargetDevice.Rom, 0x380cba };
            yield return new object[] { true, 0xf0_ffff, Cartridge.TargetDevice.Rom, 0x387fff };
            yield return new object[] { true, 0xf1_8000, Cartridge.TargetDevice.Rom, 0x388000 };
            yield return new object[] { true, 0xf1_ffff, Cartridge.TargetDevice.Rom, 0x38ffff };
            yield return new object[] { true, 0xf2_8000, Cartridge.TargetDevice.Rom, 0x390000 };
            yield return new object[] { true, 0xf2_ffff, Cartridge.TargetDevice.Rom, 0x397fff };
            yield return new object[] { true, 0xfb_8000, Cartridge.TargetDevice.Rom, 0x3d8000 };
            yield return new object[] { true, 0xfb_ffff, Cartridge.TargetDevice.Rom, 0x3dffff };
            yield return new object[] { true, 0xfc_8000, Cartridge.TargetDevice.Rom, 0x3e0000 };
            yield return new object[] { true, 0xfc_ffff, Cartridge.TargetDevice.Rom, 0x3e7fff };
            yield return new object[] { true, 0xfd_8000, Cartridge.TargetDevice.Rom, 0x3e8000 };
            yield return new object[] { true, 0xfd_fffe, Cartridge.TargetDevice.Rom, 0x3efffe };
            yield return new object[] { true, 0xfd_ffff, Cartridge.TargetDevice.Rom, 0x3effff };
            // LoRom fe-ff: 0000-7fff
            yield return new object[] { true, 0xfe_0000, Cartridge.TargetDevice.Mode20Sram2, 0x000000 };
            yield return new object[] { true, 0xfe_000a, Cartridge.TargetDevice.Mode20Sram2, 0x00000a };
            yield return new object[] { true, 0xfe_00ba, Cartridge.TargetDevice.Mode20Sram2, 0x0000ba };
            yield return new object[] { true, 0xfe_0cba, Cartridge.TargetDevice.Mode20Sram2, 0x000cba };
            yield return new object[] { true, 0xfe_7fff, Cartridge.TargetDevice.Mode20Sram2, 0x007fff };
            yield return new object[] { true, 0xff_0000, Cartridge.TargetDevice.Mode20Sram2, 0x008000 };
            yield return new object[] { true, 0xff_7ffe, Cartridge.TargetDevice.Mode20Sram2, 0x00fffe };
            yield return new object[] { true, 0xff_7fff, Cartridge.TargetDevice.Mode20Sram2, 0x00ffff };
            // LoRom fe-ff: 8000-7fff
            yield return new object[] { true, 0xfe_8000, Cartridge.TargetDevice.Rom, 0x3f0000 };
            yield return new object[] { true, 0xfe_8001, Cartridge.TargetDevice.Rom, 0x3f0001 };
            yield return new object[] { true, 0xfe_8021, Cartridge.TargetDevice.Rom, 0x3f0021 };
            yield return new object[] { true, 0xfe_8321, Cartridge.TargetDevice.Rom, 0x3f0321 };
            yield return new object[] { true, 0xfe_ffff, Cartridge.TargetDevice.Rom, 0x3f7fff };
            yield return new object[] { true, 0xff_8000, Cartridge.TargetDevice.Rom, 0x3f8000 };
            yield return new object[] { true, 0xff_fabc, Cartridge.TargetDevice.Rom, 0x3ffabc };
            yield return new object[] { true, 0xff_fffe, Cartridge.TargetDevice.Rom, 0x3ffffe };
            yield return new object[] { true, 0xff_ffff, Cartridge.TargetDevice.Rom, 0x3fffff };

            // HiRom 00-1f: 8000-ffff
            yield return new object[] { false, 0x00_8000, Cartridge.TargetDevice.Rom, 0x008000 };
            yield return new object[] { false, 0x00_800a, Cartridge.TargetDevice.Rom, 0x00800a };
            yield return new object[] { false, 0x00_80ab, Cartridge.TargetDevice.Rom, 0x0080ab };
            yield return new object[] { false, 0x00_8abc, Cartridge.TargetDevice.Rom, 0x008abc };
            yield return new object[] { false, 0x00_ffff, Cartridge.TargetDevice.Rom, 0x00ffff };
            yield return new object[] { false, 0x01_8000, Cartridge.TargetDevice.Rom, 0x018000 };
            yield return new object[] { false, 0x01_ffff, Cartridge.TargetDevice.Rom, 0x01ffff };
            yield return new object[] { false, 0x02_8000, Cartridge.TargetDevice.Rom, 0x028000 };
            yield return new object[] { false, 0x02_ffff, Cartridge.TargetDevice.Rom, 0x02ffff };
            yield return new object[] { false, 0x1d_8000, Cartridge.TargetDevice.Rom, 0x1d8000 };
            yield return new object[] { false, 0x1d_ffff, Cartridge.TargetDevice.Rom, 0x1dffff };
            yield return new object[] { false, 0x1e_8000, Cartridge.TargetDevice.Rom, 0x1e8000 };
            yield return new object[] { false, 0x1e_ffff, Cartridge.TargetDevice.Rom, 0x1effff };
            yield return new object[] { false, 0x1f_8000, Cartridge.TargetDevice.Rom, 0x1f8000 };
            yield return new object[] { false, 0x1f_fcde, Cartridge.TargetDevice.Rom, 0x1ffcde };
            yield return new object[] { false, 0x1f_ffde, Cartridge.TargetDevice.Rom, 0x1fffde };
            yield return new object[] { false, 0x1f_fffe, Cartridge.TargetDevice.Rom, 0x1ffffe };
            yield return new object[] { false, 0x1f_ffff, Cartridge.TargetDevice.Rom, 0x1fffff };
            // HiRom 80-9f: 8000-ffff
            yield return new object[] { false, 0x80_8000, Cartridge.TargetDevice.Rom, 0x008000 };
            yield return new object[] { false, 0x80_800a, Cartridge.TargetDevice.Rom, 0x00800a };
            yield return new object[] { false, 0x80_80ab, Cartridge.TargetDevice.Rom, 0x0080ab };
            yield return new object[] { false, 0x80_8abc, Cartridge.TargetDevice.Rom, 0x008abc };
            yield return new object[] { false, 0x80_ffff, Cartridge.TargetDevice.Rom, 0x00ffff };
            yield return new object[] { false, 0x81_8000, Cartridge.TargetDevice.Rom, 0x018000 };
            yield return new object[] { false, 0x81_ffff, Cartridge.TargetDevice.Rom, 0x01ffff };
            yield return new object[] { false, 0x82_8000, Cartridge.TargetDevice.Rom, 0x028000 };
            yield return new object[] { false, 0x82_ffff, Cartridge.TargetDevice.Rom, 0x02ffff };
            yield return new object[] { false, 0x9d_8000, Cartridge.TargetDevice.Rom, 0x1d8000 };
            yield return new object[] { false, 0x9d_ffff, Cartridge.TargetDevice.Rom, 0x1dffff };
            yield return new object[] { false, 0x9e_8000, Cartridge.TargetDevice.Rom, 0x1e8000 };
            yield return new object[] { false, 0x9e_ffff, Cartridge.TargetDevice.Rom, 0x1effff };
            yield return new object[] { false, 0x9f_8000, Cartridge.TargetDevice.Rom, 0x1f8000 };
            yield return new object[] { false, 0x9f_fcde, Cartridge.TargetDevice.Rom, 0x1ffcde };
            yield return new object[] { false, 0x9f_ffde, Cartridge.TargetDevice.Rom, 0x1fffde };
            yield return new object[] { false, 0x9f_fffe, Cartridge.TargetDevice.Rom, 0x1ffffe };
            yield return new object[] { false, 0x9f_ffff, Cartridge.TargetDevice.Rom, 0x1fffff };
            // HiRom 20-3f: 6000-7fff
            yield return new object[] { false, 0x20_6000, Cartridge.TargetDevice.Mode21Sram, 0x000000 };
            yield return new object[] { false, 0x20_6001, Cartridge.TargetDevice.Mode21Sram, 0x000001 };
            yield return new object[] { false, 0x20_6021, Cartridge.TargetDevice.Mode21Sram, 0x000021 };
            yield return new object[] { false, 0x20_6321, Cartridge.TargetDevice.Mode21Sram, 0x000321 };
            yield return new object[] { false, 0x20_7fff, Cartridge.TargetDevice.Mode21Sram, 0x001fff };
            yield return new object[] { false, 0x21_6000, Cartridge.TargetDevice.Mode21Sram, 0x002000 };
            yield return new object[] { false, 0x21_7fff, Cartridge.TargetDevice.Mode21Sram, 0x003fff };
            yield return new object[] { false, 0x3e_6000, Cartridge.TargetDevice.Mode21Sram, 0x03c000 };
            yield return new object[] { false, 0x3e_7fff, Cartridge.TargetDevice.Mode21Sram, 0x03dfff };
            yield return new object[] { false, 0x3f_6000, Cartridge.TargetDevice.Mode21Sram, 0x03e000 };
            yield return new object[] { false, 0x3f_7fff, Cartridge.TargetDevice.Mode21Sram, 0x03ffff };
            // HiRom a0-bf: 6000-7fff
            yield return new object[] { false, 0xa0_6000, Cartridge.TargetDevice.Mode21Sram, 0x000000 };
            yield return new object[] { false, 0xa0_6001, Cartridge.TargetDevice.Mode21Sram, 0x000001 };
            yield return new object[] { false, 0xa0_6021, Cartridge.TargetDevice.Mode21Sram, 0x000021 };
            yield return new object[] { false, 0xa0_6321, Cartridge.TargetDevice.Mode21Sram, 0x000321 };
            yield return new object[] { false, 0xa0_7fff, Cartridge.TargetDevice.Mode21Sram, 0x001fff };
            yield return new object[] { false, 0xa1_6000, Cartridge.TargetDevice.Mode21Sram, 0x002000 };
            yield return new object[] { false, 0xa1_7fff, Cartridge.TargetDevice.Mode21Sram, 0x003fff };
            yield return new object[] { false, 0xbe_6000, Cartridge.TargetDevice.Mode21Sram, 0x03c000 };
            yield return new object[] { false, 0xbe_7fff, Cartridge.TargetDevice.Mode21Sram, 0x03dfff };
            yield return new object[] { false, 0xbf_6000, Cartridge.TargetDevice.Mode21Sram, 0x03e000 };
            yield return new object[] { false, 0xbf_7fff, Cartridge.TargetDevice.Mode21Sram, 0x03ffff };
            // HiRom 20-3f: 8000-ffff
            yield return new object[] { false, 0x20_8000, Cartridge.TargetDevice.Rom, 0x208000 };
            yield return new object[] { false, 0x20_800a, Cartridge.TargetDevice.Rom, 0x20800a };
            yield return new object[] { false, 0x20_80ab, Cartridge.TargetDevice.Rom, 0x2080ab };
            yield return new object[] { false, 0x20_8abc, Cartridge.TargetDevice.Rom, 0x208abc };
            yield return new object[] { false, 0x20_ffff, Cartridge.TargetDevice.Rom, 0x20ffff };
            yield return new object[] { false, 0x21_8000, Cartridge.TargetDevice.Rom, 0x218000 };
            yield return new object[] { false, 0x21_ffff, Cartridge.TargetDevice.Rom, 0x21ffff };
            yield return new object[] { false, 0x22_8000, Cartridge.TargetDevice.Rom, 0x228000 };
            yield return new object[] { false, 0x22_ffff, Cartridge.TargetDevice.Rom, 0x22ffff };
            yield return new object[] { false, 0x3d_8000, Cartridge.TargetDevice.Rom, 0x3d8000 };
            yield return new object[] { false, 0x3d_ffff, Cartridge.TargetDevice.Rom, 0x3dffff };
            yield return new object[] { false, 0x3e_8000, Cartridge.TargetDevice.Rom, 0x3e8000 };
            yield return new object[] { false, 0x3e_ffff, Cartridge.TargetDevice.Rom, 0x3effff };
            yield return new object[] { false, 0x3f_8000, Cartridge.TargetDevice.Rom, 0x3f8000 };
            yield return new object[] { false, 0x3f_fcde, Cartridge.TargetDevice.Rom, 0x3ffcde };
            yield return new object[] { false, 0x3f_ffde, Cartridge.TargetDevice.Rom, 0x3fffde };
            yield return new object[] { false, 0x3f_fffe, Cartridge.TargetDevice.Rom, 0x3ffffe };
            yield return new object[] { false, 0x3f_ffff, Cartridge.TargetDevice.Rom, 0x3fffff };
            // HiRom a0-bf: 8000-ffff
            yield return new object[] { false, 0xa0_8000, Cartridge.TargetDevice.Rom, 0x208000 };
            yield return new object[] { false, 0xa0_800a, Cartridge.TargetDevice.Rom, 0x20800a };
            yield return new object[] { false, 0xa0_80ab, Cartridge.TargetDevice.Rom, 0x2080ab };
            yield return new object[] { false, 0xa0_8abc, Cartridge.TargetDevice.Rom, 0x208abc };
            yield return new object[] { false, 0xa0_ffff, Cartridge.TargetDevice.Rom, 0x20ffff };
            yield return new object[] { false, 0xa1_8000, Cartridge.TargetDevice.Rom, 0x218000 };
            yield return new object[] { false, 0xa1_ffff, Cartridge.TargetDevice.Rom, 0x21ffff };
            yield return new object[] { false, 0xa2_8000, Cartridge.TargetDevice.Rom, 0x228000 };
            yield return new object[] { false, 0xa2_ffff, Cartridge.TargetDevice.Rom, 0x22ffff };
            yield return new object[] { false, 0xbd_8000, Cartridge.TargetDevice.Rom, 0x3d8000 };
            yield return new object[] { false, 0xbd_ffff, Cartridge.TargetDevice.Rom, 0x3dffff };
            yield return new object[] { false, 0xbe_8000, Cartridge.TargetDevice.Rom, 0x3e8000 };
            yield return new object[] { false, 0xbe_ffff, Cartridge.TargetDevice.Rom, 0x3effff };
            yield return new object[] { false, 0xbf_8000, Cartridge.TargetDevice.Rom, 0x3f8000 };
            yield return new object[] { false, 0xbf_fcde, Cartridge.TargetDevice.Rom, 0x3ffcde };
            yield return new object[] { false, 0xbf_ffde, Cartridge.TargetDevice.Rom, 0x3fffde };
            yield return new object[] { false, 0xbf_fffe, Cartridge.TargetDevice.Rom, 0x3ffffe };
            yield return new object[] { false, 0xbf_ffff, Cartridge.TargetDevice.Rom, 0x3fffff };
            // HiRom 40-7d: 0000-ffff
            yield return new object[] { false, 0x40_0000, Cartridge.TargetDevice.Rom, 0x000000 };
            yield return new object[] { false, 0x40_0001, Cartridge.TargetDevice.Rom, 0x000001 };
            yield return new object[] { false, 0x40_0012, Cartridge.TargetDevice.Rom, 0x000012 };
            yield return new object[] { false, 0x40_0123, Cartridge.TargetDevice.Rom, 0x000123 };
            yield return new object[] { false, 0x40_8000, Cartridge.TargetDevice.Rom, 0x008000 };
            yield return new object[] { false, 0x40_ffff, Cartridge.TargetDevice.Rom, 0x00ffff };
            yield return new object[] { false, 0x41_0000, Cartridge.TargetDevice.Rom, 0x010000 };
            yield return new object[] { false, 0x41_8000, Cartridge.TargetDevice.Rom, 0x018000 };
            yield return new object[] { false, 0x41_ffff, Cartridge.TargetDevice.Rom, 0x01ffff };
            yield return new object[] { false, 0x42_0000, Cartridge.TargetDevice.Rom, 0x020000 };
            yield return new object[] { false, 0x42_8000, Cartridge.TargetDevice.Rom, 0x028000 };
            yield return new object[] { false, 0x42_ffff, Cartridge.TargetDevice.Rom, 0x02ffff };
            yield return new object[] { false, 0x7c_0000, Cartridge.TargetDevice.Rom, 0x3c0000 };
            yield return new object[] { false, 0x7c_8000, Cartridge.TargetDevice.Rom, 0x3c8000 };
            yield return new object[] { false, 0x7c_ffff, Cartridge.TargetDevice.Rom, 0x3cffff };
            yield return new object[] { false, 0x7d_0000, Cartridge.TargetDevice.Rom, 0x3d0000 };
            yield return new object[] { false, 0x7d_8000, Cartridge.TargetDevice.Rom, 0x3d8000 };
            yield return new object[] { false, 0x7d_fcde, Cartridge.TargetDevice.Rom, 0x3dfcde };
            yield return new object[] { false, 0x7d_ffde, Cartridge.TargetDevice.Rom, 0x3dffde };
            yield return new object[] { false, 0x7d_fffe, Cartridge.TargetDevice.Rom, 0x3dfffe };
            yield return new object[] { false, 0x7d_ffff, Cartridge.TargetDevice.Rom, 0x3dffff };
            // HiRom c0-fd: 0000-ffff
            yield return new object[] { false, 0xc0_0000, Cartridge.TargetDevice.Rom, 0x000000 };
            yield return new object[] { false, 0xc0_0001, Cartridge.TargetDevice.Rom, 0x000001 };
            yield return new object[] { false, 0xc0_0012, Cartridge.TargetDevice.Rom, 0x000012 };
            yield return new object[] { false, 0xc0_0123, Cartridge.TargetDevice.Rom, 0x000123 };
            yield return new object[] { false, 0xc0_8000, Cartridge.TargetDevice.Rom, 0x008000 };
            yield return new object[] { false, 0xc0_ffff, Cartridge.TargetDevice.Rom, 0x00ffff };
            yield return new object[] { false, 0xc1_0000, Cartridge.TargetDevice.Rom, 0x010000 };
            yield return new object[] { false, 0xc1_8000, Cartridge.TargetDevice.Rom, 0x018000 };
            yield return new object[] { false, 0xc1_ffff, Cartridge.TargetDevice.Rom, 0x01ffff };
            yield return new object[] { false, 0xc2_0000, Cartridge.TargetDevice.Rom, 0x020000 };
            yield return new object[] { false, 0xc2_8000, Cartridge.TargetDevice.Rom, 0x028000 };
            yield return new object[] { false, 0xc2_ffff, Cartridge.TargetDevice.Rom, 0x02ffff };
            yield return new object[] { false, 0xfc_0000, Cartridge.TargetDevice.Rom, 0x3c0000 };
            yield return new object[] { false, 0xfc_8000, Cartridge.TargetDevice.Rom, 0x3c8000 };
            yield return new object[] { false, 0xfc_ffff, Cartridge.TargetDevice.Rom, 0x3cffff };
            yield return new object[] { false, 0xfd_0000, Cartridge.TargetDevice.Rom, 0x3d0000 };
            yield return new object[] { false, 0xfd_8000, Cartridge.TargetDevice.Rom, 0x3d8000 };
            yield return new object[] { false, 0xfd_fcde, Cartridge.TargetDevice.Rom, 0x3dfcde };
            yield return new object[] { false, 0xfd_ffde, Cartridge.TargetDevice.Rom, 0x3dffde };
            yield return new object[] { false, 0xfd_fffe, Cartridge.TargetDevice.Rom, 0x3dfffe };
            yield return new object[] { false, 0xfd_ffff, Cartridge.TargetDevice.Rom, 0x3dffff };
            // HiRom fe-ff: 0000-ffff
            yield return new object[] { false, 0xfe_0000, Cartridge.TargetDevice.Rom, 0x3e0000 };
            yield return new object[] { false, 0xfe_0001, Cartridge.TargetDevice.Rom, 0x3e0001 };
            yield return new object[] { false, 0xfe_0021, Cartridge.TargetDevice.Rom, 0x3e0021 };
            yield return new object[] { false, 0xfe_0321, Cartridge.TargetDevice.Rom, 0x3e0321 };
            yield return new object[] { false, 0xfe_8000, Cartridge.TargetDevice.Rom, 0x3e8000 };
            yield return new object[] { false, 0xfe_fcde, Cartridge.TargetDevice.Rom, 0x3efcde };
            yield return new object[] { false, 0xfe_ffde, Cartridge.TargetDevice.Rom, 0x3effde };
            yield return new object[] { false, 0xfe_fffe, Cartridge.TargetDevice.Rom, 0x3efffe };
            yield return new object[] { false, 0xfe_ffff, Cartridge.TargetDevice.Rom, 0x3effff };
            yield return new object[] { false, 0xff_0000, Cartridge.TargetDevice.Rom, 0x3f0000 };
            yield return new object[] { false, 0xff_0001, Cartridge.TargetDevice.Rom, 0x3f0001 };
            yield return new object[] { false, 0xff_0021, Cartridge.TargetDevice.Rom, 0x3f0021 };
            yield return new object[] { false, 0xff_0321, Cartridge.TargetDevice.Rom, 0x3f0321 };
            yield return new object[] { false, 0xff_8000, Cartridge.TargetDevice.Rom, 0x3f8000 };
            yield return new object[] { false, 0xff_fcde, Cartridge.TargetDevice.Rom, 0x3ffcde };
            yield return new object[] { false, 0xff_ffde, Cartridge.TargetDevice.Rom, 0x3fffde };
            yield return new object[] { false, 0xff_fffe, Cartridge.TargetDevice.Rom, 0x3ffffe };
            yield return new object[] { false, 0xff_ffff, Cartridge.TargetDevice.Rom, 0x3fffff };
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
            using (var stream = new MemoryStream(romData)) {
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

        /// <summary>
        /// CartridgeへのWrite/Readで期待通りの値が帰ってくること、ROMの期待した場所に書き込みがなされていることを確認する
        /// </summary>
        /// <param name="isLowRom"></param>
        /// <param name="addr"></param>
        /// <param name="expectTarget"></param>
        /// <param name="expectLocalAddr"></param>
        [Theory, MemberData(nameof(GetConvertToLocalAddrParams))]
        public void WriteRead(bool isLowRom, uint addr, Cartridge.TargetDevice expectTarget, uint expectLocalAddr) {
            // CheckSum/CheckSumComplementが一致するようなデータを作る
            Cartridge cartridge;
            var romData = CreateRomData(isLowRom, false);
            using (var stream = new MemoryStream(romData)) {
                cartridge = new Cartridge(stream);
            }
            // 一応ROM Typeが一致することを確認
            Assert.Equal(isLowRom, cartridge.IsLoRom);
            Assert.False(cartridge.HasHeaderOffset);

            // 適当なデータを書いて読み出す
            byte expectData = 0xa5;
            cartridge.Write8(addr, expectData);
            var data = cartridge.Read8(addr);
            Assert.Equal(expectData, data);

            // Cartridge内部変数でも期待位置に書かれているか確認する
            byte[] expectTargetBuffer =
                cartridge.GetType()
                    .GetMethod("GetTargetBuffer", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance)
                    .Invoke(cartridge, new object[] { expectTarget }) as byte[]; // private methodなので強引に呼び出し
            Assert.Equal(expectData, expectTargetBuffer[expectLocalAddr]);
        }

    }
}