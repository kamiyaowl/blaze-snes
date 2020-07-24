using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using BlazeSnes.Core.Cpu;
using BlazeSnes.Core.External;

namespace BlazeSnes.Core.Tool {
    /// <summary>
    /// 65812のDisassembler
    /// </summary>
    public static class Disassembler {
        /// <summary>
        /// 引数に指定されたバイナリをすべて展開します
        /// </summary>
        /// <param name="src">データソース</param>
        /// <param name="cpuReg">CPUの事前状態、未指定の場合はReset状態の値が使われる。引数に指定した場合、継続して呼び出せるように状態は変更される。変更されたくない場合はDeepCloneした値を渡す</param>
        /// <param name="cpuReg">CPU Regの値を解析中も固定したい場合はtrue</param>
        /// <returns>(OpCode, Operand, offset)の組み合わせ</returns>
        public static IEnumerable<(OpCode, byte[], int)> Parse(IEnumerable<byte> src, CpuRegister cpuReg = null, bool isFixedReg = false) {
            // 指定されてなければ初期値を使う
            if (cpuReg == null) {
                cpuReg = new CpuRegister();
                cpuReg.Reset();
            }
            // 順番に読み出す
            IEnumerator<byte> e = src.GetEnumerator();
            for (int offset = 0; e.MoveNext(); offset++) {
                // get opcode
                var rawOpCode = e.Current;
                if (!OpCodeDefs.OpCodes.TryGetValue(rawOpCode, out OpCode opcode)) {
                    throw new FormatException($"Opcode:{rawOpCode:02X}が見つかりませんでした. {nameof(offset)}={offset}");
                }
                // get operand
                var operandLength = opcode.GetTotalArrangeBytes(cpuReg) - 1;
                var operandList = new List<byte>();
                for (int i = 0; i < operandLength; i++) {
                    if (!e.MoveNext()) {
                        throw new FormatException($"Opcode:{opcode}のOperandを取得中にデータソースが枯渇しました,  {nameof(offset)}={offset}");
                    }

                    operandList.Add(e.Current);
                }
                // 命令内容を見てX,M,E flagを更新し、次回以降の動的にフェッチサイズを変える
                // SEP/REPはOperandが1だったフラグをSET/RESETするので直接Valueに上書きしてない...
                if (!isFixedReg) {
                    switch (opcode.Inst) {
                        case Instruction.SEP: { // SEP #u8
                                var flags = (ProcessorStatusFlag)operandList[0];
                                if (flags.HasFlag(ProcessorStatusFlag.M)) {
                                    cpuReg.P.UpdateFlag(ProcessorStatusFlag.M, true);
                                }
                                if (flags.HasFlag(ProcessorStatusFlag.X)) {
                                    cpuReg.P.UpdateFlag(ProcessorStatusFlag.X, true);
                                }
                                break;
                            }
                        case Instruction.REP: { // REP #u8
                                var flags = (ProcessorStatusFlag)operandList[0];
                                if (flags.HasFlag(ProcessorStatusFlag.M)) {
                                    cpuReg.P.UpdateFlag(ProcessorStatusFlag.M, false);
                                }
                                if (flags.HasFlag(ProcessorStatusFlag.X)) {
                                    cpuReg.P.UpdateFlag(ProcessorStatusFlag.X, false);
                                }
                                break;
                            }
                        case Instruction.XCE: // Exchange Carry and Emulation Flags, --MX---CE
                            cpuReg.P.UpdateFlag(ProcessorStatusFlag.M | ProcessorStatusFlag.X | ProcessorStatusFlag.E, false);
                            break;
                        default:
                            break;
                    }
                }
                // 今回の結果
                yield return (opcode, operandList.ToArray(), offset);

                // Operand分もfetchしているのでアドレス進める
                offset += operandLength;
            }
        }
    }

    /// <summary>
    /// Disassemblerの機能を直接クラスにインプリするのは微妙なので、拡張メソッドで用意する
    /// </summary>
    public static class DisassemblerExtension {
        /// <summary>
        /// ROMの内容をDisassembleします
        /// systemAddrにResetAddrを使用します。引数のCpuRegisterは内容が変更されるのでCopyされたものを指定します
        /// </summary>
        /// <param name="c"></param>
        public static IEnumerable<(OpCode, byte[], uint, uint)> Disassemble(this Cartridge cartridge, CpuRegister cpu) => cartridge.Disassemble(cpu, cartridge.ResetAddrInEmulation);
        public static IEnumerable<(OpCode, byte[], uint, uint)> Disassemble(this Cartridge cartridge, CpuRegister cpu, uint startSysAddr) {
            // ResetVectorのLocalAddrに展開済サイズを足す
            var startBinAddr = cartridge.ConvertToLocalAddr(startSysAddr).Item2;
            var dst = Disassembler.Parse(cartridge.RomData.Skip((int)startBinAddr), cpu);
            // ひたすら頭から展開する
            foreach (var (opcode, args, offset) in dst) {
                var sysAddr = startSysAddr + (uint)offset;
                var binAddr = startBinAddr + (uint)offset;
                yield return (opcode, args, sysAddr, binAddr);
            }
        }
    }
}