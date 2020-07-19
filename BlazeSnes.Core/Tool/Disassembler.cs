using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using BlazeSnes.Core.Cpu;

namespace BlazeSnes.Core.Tool {
    /// <summary>
    /// 65812のDisassembler
    /// TODO: 不十分であれば、パス解析する機能を実装する(low priority)
    /// </summary>
    public static class Disassembler {
        /// <summary>
        /// 引数に指定されたバイナリをすべて展開します
        /// </summary>
        /// <param name="src">データソース</param>
        /// <param name="cpuReg">CPUの事前状態、未指定の場合はReset状態の値が使われる</param>
        /// <param name="cpuReg">CPU Regの値を解析中も固定したい場合はtrue</param>
        /// <returns>(OpCode, Operand)の組み合わせ</returns>
        public static IEnumerable<(OpCode, byte[])> Parse(IEnumerable<byte> src, CpuRegister cpuReg = null, bool isFixedReg = false) {
            // 指定されてなければ初期値を使う
            if (cpuReg == null) {
                cpuReg = new CpuRegister();
                cpuReg.Reset();
            }
            // 順番に読み出す
            int decodeCount = 0;
            IEnumerator<byte> e = src.GetEnumerator();
            while (e.MoveNext()) {
                // get opcode
                var rawOpCode = e.Current;
                if (!OpCodeDefs.OpCodes.TryGetValue(rawOpCode, out OpCode opcode)) {
                    throw new FormatException($"Opcode:{rawOpCode:02X}が見つかりませんでした. {nameof(decodeCount)}={decodeCount}");
                }
                // get operand
                var operandLength = opcode.GetTotalArrangeBytes(cpuReg) - 1;
                var operandList = new List<byte>();
                for (int i = 0; i < operandLength; i++) {
                    if (!e.MoveNext()) {
                        throw new FormatException($"Opcode:{opcode}のOperandを取得中にデータソースが枯渇しました,  {nameof(decodeCount)}={decodeCount}");
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

                yield return (opcode, operandList.ToArray());
                decodeCount++;
            }
        }
    }
}