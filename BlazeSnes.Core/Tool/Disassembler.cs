using System;
using System.IO;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

using BlazeSnes.Core.Cpu;

namespace BlazeSnes.Core.Tool {
    /// <summary>
    /// 65812のDisassembler
    /// </summary>
    public static class Disassembler {
        /// <summary>
        /// 引数に指定されたバイナリをすべて展開します
        /// </summary>
        /// <param name="raws"></param>
        /// <returns>(OpCode, Operand)の組み合わせ</returns>
        public static IEnumerable<(OpCode, byte[])> Parse(IEnumerable<byte> src, bool is8bitMemoMode, bool is8bitIndexMode) {
            // cpu regだけ模倣したものにしておく
            var c = new CpuRegister();
            c.Reset();
            c.P.UpdateFlag(ProcessorStatusFlag.M, is8bitMemoMode);
            c.P.UpdateFlag(ProcessorStatusFlag.X, is8bitIndexMode);
            // 順番に読み出す
            int decodeCount = 0;
            IEnumerator<byte> e = src.GetEnumerator();
            while(e.MoveNext()) {
                // get opcode
                var rawOpCode = e.Current;
                if (!OpCodeDefs.OpCodes.TryGetValue(rawOpCode, out OpCode opcode)) {
                    throw new FormatException($"Opcode:{rawOpCode:02X}が見つかりませんでした. {nameof(decodeCount)}={decodeCount}");
                }
                // get operand
                var operandLength = opcode.GetTotalArrangeBytes(c) - 1;
                var operandList = new List<byte>();
                for (int i = 0; i < operandLength; i++) {
                    if (!e.MoveNext()) {
                        throw new FormatException($"Opcode:{opcode}のOperandを取得中にデータソースが枯渇しました,  {nameof(decodeCount)}={decodeCount}");
                    }
                    operandList.Add(e.Current);
                }
                // TODO: 命令内容を見てX,M,E flagを更新し、次回以降の動的にフェッチサイズを変える
                
                yield return (opcode, operandList.ToArray());
                decodeCount++;
            }
        }
    }
}