using System;
using System.IO;

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
        /// <returns></returns>
        public static IEnumerable<OpCode> Parse(IEnumerable<byte> raws, bool is8bitMemoMode, bool is8bitIndexMode) {
            // cpu regだけ模倣したものにしておく
            var c = new CpuRegister();
            c.P.UpdateFlag(ProcessorStatusFlag.M, is8bitMemoMode);
            c.P.UpdateFlag(ProcessorStatusFlag.X, is8bitIndexMode);
            throw new NotImplementedException();
        }
    }
}