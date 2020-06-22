using System;
using System.IO;
using System.Collections.Generic;
using static BlazeSnes.Core.Cpu.OpCode;
using static BlazeSnes.Core.Cpu.FetchByte;

namespace BlazeSnes.Core.Cpu {
    /// <summary>
    /// Assembler HEXと命令の対応を示します
    /// </summary>
    public static class OpCodeDefs {
        public static readonly SortedDictionary<byte, OpCode> OpCodes = new SortedDictionary<byte, OpCode>(){
            { 0x61, new OpCode(0x61, Instruction.ADC, Addressing.DirectIndexedIndirectX, new FetchByte(2), 6, CycleOption.Add1CycleIf16bitAcccess | CycleOption.Add1CycleIfDPRegNonZero) },
            { 0x63, new OpCode(0x63, Instruction.ADC, Addressing.StackRelative, new FetchByte(2), 4, CycleOption.Add1CycleIf16bitAcccess) },
            { 0x65, new OpCode(0x65, Instruction.ADC, Addressing.Direct, new FetchByte(2), 3, CycleOption.Add1CycleIf16bitAcccess | CycleOption.Add1CycleIfDPRegNonZero) },
            { 0x67, new OpCode(0x67, Instruction.ADC, Addressing.DirectIndirectLong, new FetchByte(2), 6, CycleOption.Add1CycleIf16bitAcccess | CycleOption.Add1CycleIfDPRegNonZero) },
            { 0x69, new OpCode(0x69, Instruction.ADC, Addressing.Immediate, new FetchByte(2, AddMode.Add1ByteIfMRegZero), 2, CycleOption.Add1CycleIf16bitAcccess) },
            // TODO: 全部やる
        };
    }
}