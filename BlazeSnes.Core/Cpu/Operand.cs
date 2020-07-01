using System;
using System.Diagnostics;
using System.IO;

using BlazeSnes.Core.Common;

namespace BlazeSnes.Core.Cpu {
    /// <summary>
    /// アドレッシング解決した結果を示します
    /// </summary>
    public class Operand {
        /// <summary>
        /// 使用したアドレッシングモード、デバッグ用
        /// </summary>
        /// <value></value>
        public Addressing AddressingMode { get; internal set; }
        /// <summary>
        /// 読み出し先アドレス, Implied, Accumulatorのときに限りnull
        /// </summary>
        /// <value></value>
        public uint Addr { get; internal set; } = 0x0;
        /// <summary>
        /// かかったClock Cylce
        /// </summary>
        /// <value></value>
        public int Cycles { get; internal set; }
        /// <summary>
        /// FetchしてすすめるPCの数。命令の分は含まない
        /// </summary>
        /// <value></value>
        public int ArrangeBytes { get; internal set; }

        /// <summary>
        /// Operandを生成する際に呼び出します
        /// </summary>
        /// <param name="a"></param>
        /// <param name="addr"></param>
        /// <param name="c"></param>
        /// <param name="bytes"></param>
        public Operand(Addressing a, uint addr, int c, int bytes) {
            this.AddressingMode = a;
            this.Addr = addr;
            this.Cycles = c;
            this.ArrangeBytes = bytes;
        }

        public override string ToString() => $"{AddressingMode} addr:{Addr:x} ({Cycles}cyc, {ArrangeBytes}byte)";
    }
}