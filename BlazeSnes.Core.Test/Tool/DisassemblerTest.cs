using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using BlazeSnes.Core.Common;
using BlazeSnes.Core.Cpu;
using BlazeSnes.Core.External;
using BlazeSnes.Core.Tool;

using Xunit;
using Xunit.Sdk;

namespace BlazeSnes.Core.Test.Tool {
    public class DisassemblerTest {
        static readonly string SAMPLE_PATH = @"../../../../assets/roms/helloworld/sample1.smc";

        /// <summary>
        /// Hello, Worldを出力するサンプルROMのDisassemblerが正しいことを検証します
        /// </summary>
        [Fact]
        public void SampleRom() {
            Cartridge cartridge;
            using (var fs = new FileStream(SAMPLE_PATH, FileMode.Open)) {
                cartridge = new Cartridge(fs);
                Assert.Equal("SAMPLE1              ", cartridge.GameTitle);
                Assert.Equal(0x737f, cartridge.CheckSumComplement);
                Assert.Equal(0x8c80, cartridge.CheckSum);
                Assert.Equal(0xa20e, cartridge.ResetAddrInEmulation); // HelloのSampleはEmulationのResetしか定義していない
                Assert.Equal(0x0000, cartridge.BreakAddrInEmulation); // HelloのSampleはEmulationのResetしか定義していない
            }
            // LoROM ResetVector:00_a20eは 00-3f: 8000-ffffにマップされるので
            // a20e-8000=220eにマップされるはず
            // 2200~220dには"HELLO, WORLD!\0" が格納されている
            var expectRomAddr = (cartridge.ResetAddrInEmulation - 0x8000);
            var (targetDevice, localAddr) = cartridge.ConvertToLocalAddr(cartridge.ResetAddrInEmulation);
            Assert.Equal(0x220e, expectRomAddr);
            Assert.Equal(0x220e, (int)localAddr);
            Assert.Equal(Cartridge.TargetDevice.Rom, targetDevice);
            // とりあえずデータが入っていそうな領域だけ展開してみる
            var src = cartridge.RomData.Skip((int)localAddr).Take(18);
            var dst = Disassembler.Parse(src).ToArray();

            // sample1.asm参照
            // .proc Reset
            Assert.Equal(Instruction.SEI, dst[0].Item1.Inst); // Implied
            Assert.Equal(Instruction.CLC, dst[1].Item1.Inst); // Implied
            Assert.Equal(Instruction.XCE, dst[2].Item1.Inst); // Implied
            Assert.Equal(Instruction.PHK, dst[3].Item1.Inst); // Implied
            Assert.Equal(Instruction.PLB, dst[4].Item1.Inst); // Implied
            Assert.Equal(Instruction.REP, dst[5].Item1.Inst); // Immediate #30
            Assert.Equal(0x30, dst[5].Item2[0]); // args
            // .a16
            // .i16
            Assert.Equal(Instruction.LDX, dst[6].Item1.Inst); // #1fff
            Assert.Equal(0xff, dst[6].Item2[0]); // args
            Assert.Equal(0x1f, dst[6].Item2[1]); // args
            Assert.Equal(Instruction.TXS, dst[7].Item1.Inst); // Implied
            Assert.Equal(Instruction.JSR, dst[8].Item1.Inst); // $InitRegs
            Assert.Equal(0x82, dst[8].Item2[0]); // args
            Assert.Equal(0xa2, dst[8].Item2[1]); // args
            Assert.Equal(Instruction.SEP, dst[9].Item1.Inst); // #20
            Assert.Equal(0x20, dst[9].Item2[0]); // args
            // .a8

            // TODO: 期待ケースをもう少し伸ばす
        }
    }
}