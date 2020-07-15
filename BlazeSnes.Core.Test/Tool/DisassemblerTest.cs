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
            var expectRomAddr = (cartridge.ResetAddrInEmulation - 0x8000);
            var (targetDevice, localAddr) = cartridge.ConvertToLocalAddr(cartridge.ResetAddrInEmulation);
            Assert.Equal(0x220e, expectRomAddr);
            Assert.Equal(0x220e, (int)localAddr);
            Assert.Equal(Cartridge.TargetDevice.Rom, targetDevice);
            // とりあえずデータが入っていそうな領域だけ展開してみる
            var src = cartridge.RomData.Skip((int)localAddr).Take(7);
            var dst = Disassembler.Parse(src, false, false).ToArray();
            Assert.Equal(Instruction.SEI, dst[0].Item1.Inst); // Implied
            Assert.Equal(Instruction.CLC, dst[1].Item1.Inst); // Implied
            Assert.Equal(Instruction.XCE, dst[2].Item1.Inst); // Implied
            Assert.Equal(Instruction.PHK, dst[3].Item1.Inst); // Implied
            Assert.Equal(Instruction.PLB, dst[4].Item1.Inst); // Implied
            Assert.Equal(Instruction.REP, dst[5].Item1.Inst); // Immediate
            Assert.Equal(0x30, dst[5].Item2[0]); // args
            // TODO: 期待ケースをもう少し伸ばす
        }
    }
}