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
                    // 	sta	$2118
                    // 	inx
                    // 	inx
                    // 	dey
                    // 	bne	copyptn

                    // ; Copy NameTable
                    // 	lda	#$41a9
                    // 	sta	$2116
                    // 	ldy	#$000d
                    // 	ldx	#$0000
                    // 	lda	#$0000
                    // copyname:
                    // 	sep	#$20
                    // .a8
                    // 	lda	String, x
                    // 	rep	#$20
                    // .a16
                    // 	sta	$2118
                    // 	inx
                    // 	dey
                    // 	bne	copyname

                    // 	lda	#$01
                    // 	sta	$212c
                    // 	stz	$212d
                    // 	lda	#$0f
                    // 	sta	$2100
                    // mainloop:
                    // 	jmp	mainloop

                    // 	rti
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

            // // sample1.asm参照
            // // .proc Reset
            // Assert.Equal(Instruction.SEI, dst[0].Item1.Inst); // Implied
            // Assert.Equal(Instruction.CLC, dst[1].Item1.Inst); // Implied
            // Assert.Equal(Instruction.XCE, dst[2].Item1.Inst); // Implied
            // Assert.Equal(Instruction.PHK, dst[3].Item1.Inst); // Implied
            // Assert.Equal(Instruction.PLB, dst[4].Item1.Inst); // Implied
            // Assert.Equal(Instruction.REP, dst[5].Item1.Inst); // Immediate #30
            // Assert.Equal(0x30, dst[5].Item2[0]); // args
            // // .a16
            // // .i16
            // Assert.Equal(Instruction.LDX, dst[6].Item1.Inst); // #1fff
            // Assert.Equal(0xff, dst[6].Item2[0]); // args
            // Assert.Equal(0x1f, dst[6].Item2[1]); // args
            // Assert.Equal(Instruction.TXS, dst[7].Item1.Inst); // Implied
            // Assert.Equal(Instruction.JSR, dst[8].Item1.Inst); // $InitRegs
            // Assert.Equal(0x82, dst[8].Item2[0]); // args
            // Assert.Equal(0xa2, dst[8].Item2[1]); // args
            // Assert.Equal(Instruction.SEP, dst[9].Item1.Inst); // #20
            // Assert.Equal(0x20, dst[9].Item2[0]); // args
            // // .a8

            // TODO: 期待ケースをもう少し伸ばす
        }
    }
}