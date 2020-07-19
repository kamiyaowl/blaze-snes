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

        public static IEnumerable<object[]> VerifyDisasmAfterResetParams() {
            // LoROM ResetVector:00_a20eは 00-3f: 8000-ffffにマップされるので
            // a20e-8000=220eにマップされるはず
            // 2200~220dには"HELLO, WORLD!\0" が格納されている
            yield return new object[]{
                SAMPLE_PATH,
                "SAMPLE1              ",
                0xa20e,
                0x220e,
                new (Instruction, byte[], int)[] {
                    // .proc	Reset
                    // 	sei
                    (Instruction.SEI, new byte[]{}, 0),
                    // 	clc
                    (Instruction.CLC, new byte[]{}, 1),
                    // 	xce	; Native Mode
                    (Instruction.XCE, new byte[]{}, 2),
                    // 	phk
                    (Instruction.PHK, new byte[]{}, 3),
                    // 	plb	; DB = 0
                    (Instruction.PLB, new byte[]{}, 4),
                    // 	rep	#$30	; A,I 16bit
                    (Instruction.REP, new byte[]{ 0x30 }, 5),

                    // .a16
                    // .i16
                    // 	ldx	#$1fff
                    (Instruction.LDX, new byte[]{ 0xff, 0x1f }, 7),
                    // 	txs
                    (Instruction.TXS, new byte[]{}, 10),
                    // 	jsr	InitRegs
                    (Instruction.JSR, new byte[]{ 0x82, 0xa2}, 11), // from binary 0x221a
                    // 	sep	#$20
                    (Instruction.SEP, new byte[]{ 0x20 }, 14),

                    // .a8
                    // 	lda	#$40
                    (Instruction.LDA, new byte[]{ 0x40 }, 16),
                    // 	sta	$2107
                    (Instruction.STA, new byte[]{ 0x07, 0x21 }, 18),
                    // 	stz	$210b
                    (Instruction.STZ, new byte[]{ 0x0b, 0x21 }, 21),
                        
                    // ; Copy Palettes
                    // 	stz	$2121
                    (Instruction.STZ, new byte[]{ 0x21, 0x21 }, 24),
                    // 	ldy	#$0200
                    (Instruction.LDY, new byte[]{ 0x00, 0x02 }, 27),
                    // 	ldx	#$0000
                    (Instruction.LDX, new byte[]{ 0x00, 0x00 }, 30),

                    // copypal:
                    // 	lda	Palette, x
                    (Instruction.LDA, new byte[]{ 0x00, 0x80 }, 33), // from binary 0x2230
                    // 	sta	$2122
                    (Instruction.STA, new byte[]{ 0x22, 0x21 }, 36),
                    // 	inx
                    (Instruction.INX, new byte[]{}, 39),
                    // 	dey
                    (Instruction.DEY, new byte[]{}, 40),
                    // 	bne	copypal
                    (Instruction.BNE, new byte[]{ 0xf6 }, 41), // from binary 0x2237

                    // ; Copy Patterns
                    // 	rep	#$20
                    (Instruction.REP, new byte[]{ 0x20 }, 43),

                    // .a16
                    // 	lda	#$0000
                    (Instruction.LDA, new byte[]{ 0x00, 0x00 }, 45),
                    // 	sta	$2116
                    (Instruction.STA, new byte[]{ 0x16, 0x21 }, 48),
                    // 	ldy	#$2000
                    (Instruction.LDY, new byte[]{ 0x00, 0x20 }, 51),
                    // 	ldx	#$0000
                    (Instruction.LDX, new byte[]{ 0x00, 0x00 }, 54),

                    // copyptn:
                    // 	lda	Pattern, x
                    (Instruction.LDA, new byte[]{ 0x00, 0x82 }, 57), // from binary 0x2248
                    // 	sta	$2118
                    (Instruction.STA, new byte[]{ 0x18, 0x21 }, 60),
                    // 	inx
                    (Instruction.INX, new byte[]{}, 63),
                    // 	inx
                    (Instruction.INX, new byte[]{}, 64),
                    // 	dey
                    (Instruction.DEY, new byte[]{}, 65),
                    // 	bne	copyptn
                    (Instruction.BNE, new byte[]{ 0xf5 }, 66), // from binary 0x2251

                    // ; Copy NameTable
                    // 	lda	#$41a9
                    (Instruction.LDA, new byte[]{ 0xa9, 0x41 }, 68),
                    // 	sta	$2116
                    (Instruction.STA, new byte[]{ 0x16, 0x21 }, 71),
                    // 	ldy	#$000d
                    (Instruction.LDY, new byte[]{ 0x0d, 0x00 }, 74),
                    // 	ldx	#$0000
                    (Instruction.LDX, new byte[]{ 0x00, 0x00 }, 77),
                    // 	lda	#$0000
                    (Instruction.LDA, new byte[]{ 0x00, 0x00 }, 80),

                    // copyname:
                    // 	sep	#$20
                    (Instruction.SEP, new byte[]{ 0x20 }, 83),

                    // .a8
                    // 	lda	String, x
                    (Instruction.LDA, new byte[]{ 0x00, 0xa2 }, 85), // from binary 0x2263
                    // 	rep	#$20
                    (Instruction.REP, new byte[]{ 0x20 }, 88),

                    // .a16
                    // 	sta	$2118
                    (Instruction.STA, new byte[]{ 0x18, 0x21 }, 90),
                    // 	inx
                    (Instruction.INX, new byte[]{}, 93),
                    // 	dey
                    (Instruction.DEY, new byte[]{}, 94),
                    // 	bne	copyname
                    (Instruction.BNE, new byte[]{ 0xf2 }, 95), // from binary 0x226e

                    // 	lda	#$01
                    (Instruction.LDA, new byte[]{ 0x01, 0x00 }, 97),
                    // 	sta	$212c
                    (Instruction.STA, new byte[]{ 0x2c, 0x21 }, 100),
                    // 	stz	$212d
                    (Instruction.STZ, new byte[]{ 0x2d, 0x21 }, 103),
                    // 	lda	#$0f
                    (Instruction.LDA, new byte[]{ 0x0f, 0x00 }, 106),
                    // 	sta	$2100
                    (Instruction.STA, new byte[]{ 0x00, 0x21 }, 109),

                    // mainloop:
                    // 	jmp	mainloop
                    (Instruction.JMP, new byte[]{ 0x7e, 0xa2 }, 112), // from binary 0x227f
                    // 	rti
                    (Instruction.RTI, new byte[]{}, 115),
                    // .endproc
                },
            };
        }
        /// <summary>
        /// Reset Vector以後のAssemblyを検証します
        /// Branch先の解析などは含まれません
        /// </summary>
        [Theory, MemberData(nameof(VerifyDisasmAfterResetParams))]
        public void VerifyDisasmAfterReset(
            string path,
            string title,
            ushort resetAddrInEmulation,
            int binAddr,
            IEnumerable<(Instruction, byte[], int)> expectOpcodes) {
            // binaryの内容を展開
            Cartridge cartridge;
            using (var fs = new FileStream(path, FileMode.Open)) {
                cartridge = new Cartridge(fs);
                Assert.Equal(title, cartridge.GameTitle);
                Assert.Equal(resetAddrInEmulation, cartridge.ResetAddrInEmulation); // ResetVectorが期待通りか確認
            }

            // binaryのマップ先が期待通りになっているか確認
            var (targetDevice, localAddr) = cartridge.ConvertToLocalAddr(cartridge.ResetAddrInEmulation);
            Assert.Equal(binAddr, (int)localAddr);
            Assert.Equal(Cartridge.TargetDevice.Rom, targetDevice);

            // resetVector相当の位置からを展開対象にする
            var src = cartridge.RomData.Skip((int)localAddr);

            // 期待値が入っているところまで展開してZipする
            var dst = expectOpcodes.Zip(
                    Disassembler.Parse(src),
                    (expect, actual) => (expect.Item1, expect.Item2, expect.Item3, actual.Item1.Inst, actual.Item2, actual.Item3)
                );

            // 期待値確認
            foreach (var (expectInst, expectArgs, expectOffset, actualInst, actualArgs, actualOffset) in dst) {
                Assert.Equal(expectInst, actualInst);
                Assert.Equal(expectArgs, actualArgs);
                Assert.Equal(expectOffset, actualOffset);
            }
        }
    }
}