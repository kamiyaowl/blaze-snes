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
                new (Instruction, byte[])[] {
                    // .proc	Reset
                    // 	sei
                    (Instruction.SEI, new byte[]{}),
                    // 	clc
                    (Instruction.CLC, new byte[]{}),
                    // 	xce	; Native Mode
                    (Instruction.XCE, new byte[]{}),
                    // 	phk
                    (Instruction.PHK, new byte[]{}),
                    // 	plb	; DB = 0
                    (Instruction.PLB, new byte[]{}),
                    // 	rep	#$30	; A,I 16bit
                    (Instruction.REP, new byte[]{ 0x30 }),

                    // .a16
                    // .i16
                    // 	ldx	#$1fff
                    (Instruction.LDX, new byte[]{ 0xff, 0x1f }),
                    // 	txs
                    (Instruction.TXS, new byte[]{}),
                    // 	jsr	InitRegs
                    (Instruction.JSR, new byte[]{ 0x82, 0xa2}), // from binary 0x221a
                    // 	sep	#$20
                    (Instruction.SEP, new byte[]{ 0x20 }),

                    // .a8
                    // 	lda	#$40
                    (Instruction.LDA, new byte[]{ 0x40 }),
                    // 	sta	$2107
                    (Instruction.STA, new byte[]{ 0x07, 0x21 }),
                    // 	stz	$210b
                    (Instruction.STZ, new byte[]{ 0x0b, 0x21 }),
                        
                    // ; Copy Palettes
                    // 	stz	$2121
                    (Instruction.STZ, new byte[]{ 0x21, 0x21 }),
                    // 	ldy	#$0200
                    (Instruction.LDY, new byte[]{ 0x00, 0x02 }),
                    // 	ldx	#$0000
                    (Instruction.LDX, new byte[]{ 0x00, 0x00 }),

                    // copypal:
                    // 	lda	Palette, x
                    (Instruction.LDA, new byte[]{ 0x00, 0x80 }), // from binary 0x2230
                    // 	sta	$2122
                    (Instruction.STA, new byte[]{ 0x22, 0x21 }),
                    // 	inx
                    (Instruction.INX, new byte[]{  }),
                    // 	dey
                    (Instruction.DEY, new byte[]{  }),
                    // 	bne	copypal
                    (Instruction.BNE, new byte[]{ 0xf6 }), // from binary 0x2237

                    // ; Copy Patterns
                    // 	rep	#$20
                    (Instruction.REP, new byte[]{ 0x20 }),

                    // .a16
                    // 	lda	#$0000
                    (Instruction.LDA, new byte[]{ 0x00, 0x00 }),
                    // 	sta	$2116
                    (Instruction.STA, new byte[]{ 0x16, 0x21 }),
                    // 	ldy	#$2000
                    (Instruction.LDY, new byte[]{ 0x00, 0x20 }),
                    // 	ldx	#$0000
                    (Instruction.LDX, new byte[]{ 0x00, 0x00 }),

                    // copyptn:
                    // 	lda	Pattern, x
                    (Instruction.LDA, new byte[]{ 0x00, 0x82 }), // from binary 0x2248
                    // 	sta	$2118
                    (Instruction.STA, new byte[]{ 0x18, 0x21 }),
                    // 	inx
                    (Instruction.INX, new byte[]{ }),
                    // 	inx
                    (Instruction.INX, new byte[]{ }),
                    // 	dey
                    (Instruction.DEY, new byte[]{ }),
                    // 	bne	copyptn
                    (Instruction.BNE, new byte[]{ 0xf5 }), // from binary 0x2251

                    // ; Copy NameTable
                    // 	lda	#$41a9
                    (Instruction.LDA, new byte[]{ 0xa9, 0x41 }),
                    // 	sta	$2116
                    (Instruction.STA, new byte[]{ 0x16, 0x21 }),
                    // 	ldy	#$000d
                    (Instruction.LDY, new byte[]{ 0x0d, 0x00 }),
                    // 	ldx	#$0000
                    (Instruction.LDX, new byte[]{ 0x00, 0x00 }),
                    // 	lda	#$0000
                    (Instruction.LDA, new byte[]{ 0x00, 0x00 }),

                    // copyname:
                    // 	sep	#$20
                    (Instruction.SEP, new byte[]{ 0x20 }),

                    // .a8
                    // 	lda	String, x
                    (Instruction.LDA, new byte[]{ 0x00, 0xa2 }), // from binary 0x2263
                    // 	rep	#$20
                    (Instruction.REP, new byte[]{ 0x20 }),

                    // .a16
                    // 	sta	$2118
                    (Instruction.STA, new byte[]{ 0x18, 0x21 }),
                    // 	inx
                    (Instruction.INX, new byte[]{ }),
                    // 	dey
                    (Instruction.DEY, new byte[]{ }),
                    // 	bne	copyname
                    (Instruction.BNE, new byte[]{ 0xf2 }), // from binary 0x226e

                    // 	lda	#$01
                    (Instruction.LDA, new byte[]{ 0x01, 0x00 }),
                    // 	sta	$212c
                    (Instruction.STA, new byte[]{ 0x2c, 0x21 }),
                    // 	stz	$212d
                    (Instruction.STZ, new byte[]{ 0x2d, 0x21 }),
                    // 	lda	#$0f
                    (Instruction.LDA, new byte[]{ 0x0f, 0x00 }),
                    // 	sta	$2100
                    (Instruction.STA, new byte[]{ 0x00, 0x21 }),

                    // mainloop:
                    // 	jmp	mainloop
                    (Instruction.JMP, new byte[]{ 0x7e, 0xa2 }), // from binary 0x227f
                    // 	rti
                    (Instruction.RTI, new byte[]{ }),
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
            IEnumerable<(Instruction, byte[])> expectOpcodes) {
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
                    (expect, actual) => (expect.Item1, expect.Item2, actual.Item1.Inst, actual.Item2)
                );

            // 期待値確認
            foreach (var (expectInst, expectArgs, actualInst, actualArgs) in dst) {
                Assert.Equal(expectInst, actualInst);
                Assert.Equal(expectArgs, actualArgs);
            }
        }
    }
}