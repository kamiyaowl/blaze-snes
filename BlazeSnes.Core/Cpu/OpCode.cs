using System;
using System.Collections.Generic;
using System.IO;

namespace BlazeSnes.Core.Cpu {
    public static class OpCode {
        /// <summary>
        /// OpCodeと命令の対応を示します
        /// </summary>
        public static readonly SortedDictionary < byte, (Instruction, Addressing) > OpCodes = new SortedDictionary < byte, (Instruction, Addressing) > () {
            // { 0x61, ( Instruction.ADC, Addressing.indexedin ) }, // TODO: Addressing Modeを一通りちゃんと見る
            { 0x63, (Instruction.ADC, Addressing.StackRelative) },
            // TODO: 全部作る
        };
    }
}