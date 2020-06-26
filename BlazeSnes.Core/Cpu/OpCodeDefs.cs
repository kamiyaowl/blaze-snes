using System;
using System.Collections.Generic;
using System.IO;
using static BlazeSnes.Core.Cpu.FetchByte;
using static BlazeSnes.Core.Cpu.OpCode;

namespace BlazeSnes.Core.Cpu {
    /// <summary>
    /// Assembler HEXと命令の対応を示します
    /// </summary>
    public static class OpCodeDefs {

        // TODO: 全部やる
        /// <summary>
        ///  命令定義一式です
        ///  ref https://wiki.superfamicom.org/65816-reference
        /// </summary>
        /// <typeparam name="byte"></typeparam>
        /// <typeparam name="OpCode"></typeparam>
        /// <returns></returns>
        public static readonly SortedDictionary<byte, OpCode> OpCodes = new SortedDictionary<byte, OpCode>() {
            // Assembler Example	Alias	Proper Name	HEX	Addressing Mode	Flags Set	Bytes	Cycles
            // ADC (dp,X)		Add With Carry	61	DP Indexed Indirect,X	NV----ZC	2	6[^1][^2]
            { 0x61, new OpCode (0x61, Instruction.ADC, Addressing.DirectIndexedIndirectX, new FetchByte (2), 6, CycleOption.Add1CycleIf16bitAcccess | CycleOption.Add1CycleIfDPRegNonZero) },
            // ADC sr,S		Add With Carry	63	Stack Relative	NV----ZC	2	4[^1]
            { 0x63, new OpCode (0x63, Instruction.ADC, Addressing.StackRelative, new FetchByte (2), 4, CycleOption.Add1CycleIf16bitAcccess) },
            // ADC dp		Add With Carry	65	Direct Page	NV----ZC	2	3[^1][^2]
            { 0x65, new OpCode (0x65, Instruction.ADC, Addressing.Direct, new FetchByte (2), 3, CycleOption.Add1CycleIf16bitAcccess | CycleOption.Add1CycleIfDPRegNonZero) },
            // ADC [dp]		Add With Carry	67	DP Indirect Long	NV----ZC	2	6[^1][^2]
            { 0x67, new OpCode (0x67, Instruction.ADC, Addressing.DirectIndirectLong, new FetchByte (2), 6, CycleOption.Add1CycleIf16bitAcccess | CycleOption.Add1CycleIfDPRegNonZero) },
            // ADC #const		Add With Carry	69	Immediate	NV----ZC	2[^12]	2[^1]
            { 0x69, new OpCode (0x69, Instruction.ADC, Addressing.Immediate, new FetchByte (2, AddMode.Add1ByteIfMRegZero), 2, CycleOption.Add1CycleIf16bitAcccess) },
            // ADC addr		Add With Carry	6D	Absolute	NV----ZC	3	4[^1]
            { 0x6d, new OpCode (0x6d, Instruction.ADC, Addressing.Absolute, new FetchByte (3), 4, CycleOption.Add1CycleIf16bitAcccess) },
            // ADC long		Add With Carry	6F	Absolute Long	NV----ZC	4	5[^1]
            { 0x6f, new OpCode (0x6f, Instruction.ADC, Addressing.AbsoluteLong, new FetchByte (4), 5, CycleOption.Add1CycleIf16bitAcccess) },
            // ADC ( dp),Y		Add With Carry	71	DP Indirect Indexed, Y	NV----ZC	2	5[^1][^2][^3]
            { 0x71, new OpCode (0x71, Instruction.ADC, Addressing.DirectIndirectIndexedY, new FetchByte (2), 5, CycleOption.Add1CycleIf16bitAcccess | CycleOption.Add1CycleIfDPRegNonZero | CycleOption.Add1CycleIfPageBoundaryOrXRegZero) },
            // ADC (dp)		Add With Carry	72	DP Indirect	NV----ZC	2	5[^1][^2]
            { 0x72, new OpCode (0x72, Instruction.ADC, Addressing.DirectIndirect, new FetchByte (2), 5, CycleOption.Add1CycleIf16bitAcccess | CycleOption.Add1CycleIfDPRegNonZero) },
            // ADC (sr,S),Y		Add With Carry	73	SR Indirect Indexed,Y	NV----ZC	2	7[^1]
            { 0x73, new OpCode (0x73, Instruction.ADC, Addressing.StackRelativeIndirectIndexedY, new FetchByte (2), 7, CycleOption.Add1CycleIf16bitAcccess) },
            // ADC dp,X		Add With Carry	75	DP Indexed,X	NV----ZC	2	4[^1][^2]
            { 0x75, new OpCode (0x75, Instruction.ADC, Addressing.DirectIndexedX, new FetchByte (2), 4, CycleOption.Add1CycleIf16bitAcccess | CycleOption.Add1CycleIfDPRegNonZero) },
            // ADC [dp],Y		Add With Carry	77	DP Indirect Long Indexed, Y	NV----ZC	2	6[^1][^2]
            { 0x77, new OpCode (0x77, Instruction.ADC, Addressing.DirectIndirectLongIndexedY, new FetchByte (2), 6, CycleOption.Add1CycleIf16bitAcccess | CycleOption.Add1CycleIfDPRegNonZero) },
            // ADC addr,Y		Add With Carry	79	Absolute Indexed,Y	NV----ZC	3	4[^1][^3]
            { 0x79, new OpCode (0x79, Instruction.ADC, Addressing.AbsoluteIndexedY, new FetchByte (3), 4, CycleOption.Add1CycleIf16bitAcccess | CycleOption.Add1CycleIfPageBoundaryOrXRegZero) },
            // ADC addr,X		Add With Carry	7D	Absolute Indexed,X	NV----ZC	3	4[^1][^3]
            { 0x7d, new OpCode (0x7d, Instruction.ADC, Addressing.AbsoluteIndexedX, new FetchByte (3), 4, CycleOption.Add1CycleIf16bitAcccess | CycleOption.Add1CycleIfPageBoundaryOrXRegZero) },
            // ADC long,X		Add With Carry	7F	Absolute Long Indexed,X	NV----ZC	4	5[^1]
            { 0x7f, new OpCode (0x7f, Instruction.ADC, Addressing.AbsoluteLongIndexedX, new FetchByte (4), 5, CycleOption.Add1CycleIf16bitAcccess) },
            // AND (_dp,_X)		AND Accumulator with Memory	21	DP Indexed Indirect,X	N-----Z-	2	6[^1][^2]
            { 0x21, new OpCode (0x21, Instruction.AND, Addressing.DirectIndexedIndirectX, new FetchByte (2), 6, CycleOption.Add1CycleIf16bitAcccess | CycleOption.Add1CycleIfDPRegNonZero) },
            // AND sr,S		AND Accumulator with Memory	23	Stack Relative	N-----Z-	2	4[^1]
            { 0x23, new OpCode (0x23, Instruction.AND, Addressing.StackRelative, new FetchByte (2), 4, CycleOption.Add1CycleIf16bitAcccess) },
            // AND dp		AND Accumulator with Memory	25	Direct Page	N-----Z-	2	3[^1][^2]
            { 0x25, new OpCode (0x25, Instruction.AND, Addressing.Direct, new FetchByte (2), 3, CycleOption.Add1CycleIf16bitAcccess | CycleOption.Add1CycleIfDPRegNonZero) },
            // AND [dp]		AND Accumulator with Memory	27	DP Indirect Long	N-----Z-	2	6[^1][^2]
            { 0x27, new OpCode (0x27, Instruction.AND, Addressing.DirectIndirectLong, new FetchByte (2), 6, CycleOption.Add1CycleIf16bitAcccess | CycleOption.Add1CycleIfDPRegNonZero) },
            // AND #const		AND Accumulator with Memory	29	Immediate	N-----Z-	2[^12]	2[^1]
            { 0x29, new OpCode (0x29, Instruction.AND, Addressing.Immediate, new FetchByte (2, AddMode.Add1ByteIfMRegZero), 2, CycleOption.Add1CycleIf16bitAcccess) },
            // AND addr		AND Accumulator with Memory	2D	Absolute	N-----Z-	3	4[^1]
            { 0x2d, new OpCode (0x2d, Instruction.AND, Addressing.Absolute, new FetchByte (3), 4, CycleOption.Add1CycleIf16bitAcccess) },
            // AND long		AND Accumulator with Memory	2F	Absolute Long	N-----Z-	4	5[^1]
            { 0x2f, new OpCode (0x2f, Instruction.AND, Addressing.AbsoluteLong, new FetchByte (4), 5, CycleOption.Add1CycleIf16bitAcccess) },
            // AND (dp),Y		AND Accumulator with Memory	31	DP Indirect Indexed, Y	N-----Z-	2	5[^1][^2][^3]
            { 0x31, new OpCode (0x31, Instruction.AND, Addressing.DirectIndirectIndexedY, new FetchByte (2), 5, CycleOption.Add1CycleIf16bitAcccess | CycleOption.Add1CycleIfDPRegNonZero | CycleOption.Add1CycleIfPageBoundaryOrXRegZero) },
            // AND (dp)		AND Accumulator with Memory	32	DP Indirect	N-----Z-	2	5[^1][^2]
            { 0x32, new OpCode (0x32, Instruction.AND, Addressing.DirectIndirect, new FetchByte (2), 5, CycleOption.Add1CycleIf16bitAcccess | CycleOption.Add1CycleIfDPRegNonZero) },
            // AND (sr,S),Y		AND Accumulator with Memory	33	SR Indirect Indexed,Y	N-----Z-	2	7[^1]
            { 0x33, new OpCode (0x33, Instruction.AND, Addressing.StackRelativeIndirectIndexedY, new FetchByte (2), 7, CycleOption.Add1CycleIf16bitAcccess) },
            // AND dp,X		AND Accumulator with Memory	35	DP Indexed,X	N-----Z-	2	4[^1][^2]
            { 0x35, new OpCode (0x35, Instruction.AND, Addressing.DirectIndexedX, new FetchByte (2), 4, CycleOption.Add1CycleIf16bitAcccess | CycleOption.Add1CycleIfDPRegNonZero) },
            // AND [dp],Y		AND Accumulator with Memory	37	DP Indirect Long Indexed, Y	N-----Z-	2	6[^1][^2]
            { 0x37, new OpCode (0x37, Instruction.AND, Addressing.DirectIndirectLongIndexedY, new FetchByte (2), 6, CycleOption.Add1CycleIf16bitAcccess | CycleOption.Add1CycleIfDPRegNonZero) },
            // AND addr,Y		AND Accumulator with Memory	39	Absolute Indexed,Y	N-----Z-	3	4[^1][^3]
            { 0x39, new OpCode (0x39, Instruction.AND, Addressing.AbsoluteIndexedY, new FetchByte (3), 4, CycleOption.Add1CycleIf16bitAcccess | CycleOption.Add1CycleIfPageBoundaryOrXRegZero) },
            // AND addr,X		AND Accumulator with Memory	3D	Absolute Indexed,X	N-----Z-	3	4[^1][^3]
            { 0x3d, new OpCode (0x3d, Instruction.AND, Addressing.AbsoluteIndexedX, new FetchByte (3), 4, CycleOption.Add1CycleIf16bitAcccess | CycleOption.Add1CycleIfPageBoundaryOrXRegZero) },
            // AND long,X		AND Accumulator with Memory	3F	Absolute Long Indexed,X	N-----Z-	4	5[^1]
            { 0x3f, new OpCode (0x3f, Instruction.AND, Addressing.AbsoluteLongIndexedX, new FetchByte (4), 5, CycleOption.Add1CycleIf16bitAcccess) },
            // ASL dp		Arithmetic Shift Left	06	Direct Page	N-----ZC	2	5[^2][^4]
            { 0x06, new OpCode (0x06, Instruction.ASL, Addressing.Direct, new FetchByte (2), 5, CycleOption.Add1CycleIfDPRegNonZero | CycleOption.Add2CycleIf16bitaccess) },
            // ASL A		Arithmetic Shift Left	0A	Accumulator	N-----ZC	1	2
            { 0x0a, new OpCode (0x0a, Instruction.ASL, Addressing.Accumulator, new FetchByte (1), 2, CycleOption.None) },
            // ASL addr		Arithmetic Shift Left	0E	Absolute	N-----ZC	3	6[^4]
            { 0x0e, new OpCode (0x0e, Instruction.ASL, Addressing.Absolute, new FetchByte (3), 6, CycleOption.Add2CycleIf16bitaccess) },
            // ASL dp,X		Arithmetic Shift Left	16	DP Indexed,X	N-----ZC	2	6[^2][^4]
            { 0x16, new OpCode (0x16, Instruction.ASL, Addressing.DirectIndexedX, new FetchByte (2), 6, CycleOption.Add1CycleIfDPRegNonZero | CycleOption.Add2CycleIf16bitaccess) },
            // ASL addr,X		Arithmetic Shift Left	1E	Absolute Indexed,X	N-----ZC	3	7[^4]
            { 0x1e, new OpCode (0x1e, Instruction.ASL, Addressing.AbsoluteIndexedX, new FetchByte (3), 7, CycleOption.Add2CycleIf16bitaccess) },
            // BCC nearlabel	BLT	Branch if Carry Clear	90	Program Counter Relative		2	2[^5][^6]
            { 0x90, new OpCode (0x90, Instruction.BCC, Addressing.ProgramCounterRelative, new FetchByte (2), 2, CycleOption.Add1CycleIfBranchIsTaken | CycleOption.Add1CycleIfBranchIsTakenAndPageCrossesInEmuMode) },
            // BCS nearlabel	BGE	Branch if Carry Set	B0	Program Counter Relative		2	2[^5][^6]
            { 0x80, new OpCode (0x80, Instruction.BCS, Addressing.ProgramCounterRelative, new FetchByte (2), 2, CycleOption.Add1CycleIfBranchIsTaken | CycleOption.Add1CycleIfBranchIsTakenAndPageCrossesInEmuMode) },
            // BEQ nearlabel		Branch if Equal	F0	Program Counter Relative		2	2[^5][^6]
            { 0xf0, new OpCode (0xf0, Instruction.BEQ, Addressing.ProgramCounterRelative, new FetchByte (2), 2, CycleOption.Add1CycleIfBranchIsTaken | CycleOption.Add1CycleIfBranchIsTakenAndPageCrossesInEmuMode) },
            // BIT dp		Test Bits	24	Direct Page	NV----Z-	2	3[^1][^2]
            { 0x24, new OpCode (0x24, Instruction.BIT, Addressing.Direct, new FetchByte (2), 3, CycleOption.Add1CycleIf16bitAcccess | CycleOption.Add1CycleIfDPRegNonZero) },
            // BIT addr		Test Bits	2C	Absolute	NV----Z-	3	4[^1]
            { 0x2c, new OpCode (0x2c, Instruction.BIT, Addressing.Absolute, new FetchByte (3), 4, CycleOption.Add1CycleIf16bitAcccess) },
            // BIT dp,X		Test Bits	34	DP Indexed,X	NV----Z-	2	4[^1][^2]
            { 0x34, new OpCode (0x34, Instruction.BIT, Addressing.DirectIndexedX, new FetchByte (2), 4, CycleOption.Add1CycleIf16bitAcccess | CycleOption.Add1CycleIfDPRegNonZero) },
            // BIT addr,X		Test Bits	3C	Absolute Indexed,X	NV----Z-	3	4[^1][^3]
            { 0x3c, new OpCode (0x3c, Instruction.BIT, Addressing.AbsoluteIndexedX, new FetchByte (3), 4, CycleOption.Add1CycleIf16bitAcccess | CycleOption.Add1CycleIfPageBoundaryOrXRegZero) },
            // BIT #const		Test Bits	89	Immediate	------Z-	2[^12]	2[^1]
            { 0x89, new OpCode (0x89, Instruction.BIT, Addressing.Implied, new FetchByte (2, AddMode.Add1ByteIfMRegZero), 2, CycleOption.Add1CycleIf16bitAcccess | CycleOption.Add1CycleIfPageBoundaryOrXRegZero) },
            // BMI nearlabel		Branch if Minus	30	Program Counter Relative		2	2[^5][^6]
            { 0x30, new OpCode (0x30, Instruction.BMI, Addressing.ProgramCounterRelative, new FetchByte (2), 2, CycleOption.Add1CycleIfBranchIsTaken | CycleOption.Add1CycleIfBranchIsTakenAndPageCrossesInEmuMode) },
            // BNE nearlabel		Branch if Not Equal	D0	Program Counter Relative		2	2[^5][^6]
            { 0xd0, new OpCode (0xd0, Instruction.BNE, Addressing.ProgramCounterRelative, new FetchByte (2), 2, CycleOption.Add1CycleIfBranchIsTaken | CycleOption.Add1CycleIfBranchIsTakenAndPageCrossesInEmuMode) },
            // BPL nearlabel		Branch if Plus	10	Program Counter Relative		2	2[^5][^6]
            { 0x10, new OpCode (0x10, Instruction.BPL, Addressing.ProgramCounterRelative, new FetchByte (2), 2, CycleOption.Add1CycleIfBranchIsTaken | CycleOption.Add1CycleIfBranchIsTakenAndPageCrossesInEmuMode) },
            // BRA nearlabel		Branch Always	80	Program Counter Relative		2	3[^6]
            { 0x80, new OpCode (0x80, Instruction.BRA, Addressing.ProgramCounterRelative, new FetchByte (2), 3, CycleOption.Add1CycleIfBranchIsTakenAndPageCrossesInEmuMode) },
            // BRK		Break	00	Stack/Interrupt	----DI--	2[^13]	7[^7]
            { 0x00, new OpCode (0x00, Instruction.BRK, Addressing.Implied, new FetchByte (2, AddMode.Add1ByteForSignatureByte), 7, CycleOption.Add1CycleIfNativeMode) },
            // BRL label		Branch Long Always	82	Program Counter Relative Long		3	4
            { 0x82, new OpCode (0x82, Instruction.BRL, Addressing.ProgramCounterRelativeLong, new FetchByte (3), 4, CycleOption.None) },
            // BVC nearlabel		Branch if Overflow Clear	50	Program Counter Relative		2	2[^5][^6]
            { 0x50, new OpCode (0x50, Instruction.BVC, Addressing.ProgramCounterRelative, new FetchByte (2), 2, CycleOption.Add1CycleIfBranchIsTaken | CycleOption.Add1CycleIfBranchIsTakenAndPageCrossesInEmuMode) },
            // BVS nearlabel		Branch if Overflow Set	70	Program Counter Relative		2	2[^5][^6]
            { 0x70, new OpCode (0x70, Instruction.BVS, Addressing.ProgramCounterRelative, new FetchByte (2), 2, CycleOption.Add1CycleIfBranchIsTaken | CycleOption.Add1CycleIfBranchIsTakenAndPageCrossesInEmuMode) },
            // CLC		Clear Carry	18	Implied	-------C	1	2
            { 0x18, new OpCode (0x18, Instruction.CLC, Addressing.Implied, new FetchByte (1), 2, CycleOption.None) },
            // CLD		Clear Decimal Mode Flag	D8	Implied	----D---	1	2
            { 0xd8, new OpCode (0xd8, Instruction.CLD, Addressing.Implied, new FetchByte (1), 2, CycleOption.None) },
            // CLI		Clear Interrupt Disable Flag	58	Implied	-----I--	1	2
            { 0x58, new OpCode (0x58, Instruction.CLI, Addressing.Implied, new FetchByte (1), 2, CycleOption.None) },
            // CLV		Clear Overflow Flag	B8	Implied	-V------	1	2
            { 0xb8, new OpCode (0xb8, Instruction.CLV, Addressing.Implied, new FetchByte (1), 2, CycleOption.None) },
            // CMP (_dp,_X)		Compare Accumulator with Memory	C1	DP Indexed Indirect,X	N-----ZC	2	6[^1][^2]
            { 0xc1, new OpCode (0xc1, Instruction.CMP, Addressing.DirectIndexedIndirectX, new FetchByte (2), 6, CycleOption.Add1CycleIf16bitAcccess | CycleOption.Add1CycleIfDPRegNonZero) },
            // CMP sr,S		Compare Accumulator with Memory	C3	Stack Relative	N-----ZC	2	4[^1]
            { 0xc3, new OpCode (0xc3, Instruction.CMP, Addressing.StackRelative, new FetchByte (2), 4, CycleOption.Add1CycleIf16bitAcccess) },
            // CMP dp		Compare Accumulator with Memory	C5	Direct Page	N-----ZC	2	3[^1][^2]
            { 0xc5, new OpCode (0x65, Instruction.CMP, Addressing.Direct, new FetchByte (2), 3, CycleOption.Add1CycleIf16bitAcccess | CycleOption.Add1CycleIfDPRegNonZero) },
            // CMP [dp]		Compare Accumulator with Memory	C7	DP Indirect Long	N-----ZC	2	6[^1][^2]
            { 0xc7, new OpCode (0xc7, Instruction.CMP, Addressing.DirectIndirectLong, new FetchByte (2), 6, CycleOption.Add1CycleIf16bitAcccess | CycleOption.Add1CycleIfDPRegNonZero) },
            // CMP #const		Compare Accumulator with Memory	C9	Immediate	N-----ZC	2[^12]	2[^1]
            { 0xc9, new OpCode (0xc9, Instruction.CMP, Addressing.Immediate, new FetchByte (2, AddMode.Add1ByteIfMRegZero), 2, CycleOption.Add1CycleIf16bitAcccess) },
            // CMP addr		Compare Accumulator with Memory	CD	Absolute	N-----ZC	3	4[^1]
            { 0xcd, new OpCode (0xcd, Instruction.CMP, Addressing.Absolute, new FetchByte (3), 4, CycleOption.Add1CycleIf16bitAcccess) },
            // CMP long		Compare Accumulator with Memory	CF	Absolute Long	N-----ZC	4	5[^1]
            { 0xcf, new OpCode (0xcf, Instruction.CMP, Addressing.AbsoluteLong, new FetchByte (4), 5, CycleOption.Add1CycleIf16bitAcccess) },
            // CMP (dp),Y		Compare Accumulator with Memory	D1	DP Indirect Indexed, Y	N-----ZC	2	5[^1][^2][^3]
            { 0xd1, new OpCode (0xd1, Instruction.CMP, Addressing.DirectIndirectIndexedY, new FetchByte (2), 5, CycleOption.Add1CycleIf16bitAcccess | CycleOption.Add1CycleIfDPRegNonZero | CycleOption.Add1CycleIfPageBoundaryOrXRegZero) },
            // CMP (dp)		Compare Accumulator with Memory	D2	DP Indirect	N-----ZC	2	5[^1][^2]
            { 0xd2, new OpCode (0xd2, Instruction.CMP, Addressing.DirectIndirect, new FetchByte (2), 5, CycleOption.Add1CycleIf16bitAcccess | CycleOption.Add1CycleIfDPRegNonZero) },
            // CMP (sr,S),Y		Compare Accumulator with Memory	D3	SR Indirect Indexed,Y	N-----ZC	2	7[^1]
            { 0xd3, new OpCode (0xd3, Instruction.CMP, Addressing.StackRelativeIndirectIndexedY, new FetchByte (2), 7, CycleOption.Add1CycleIf16bitAcccess) },
            // CMP dp,X		Compare Accumulator with Memory	D5	DP Indexed,X	N-----ZC	2	4[^1][^2]
            { 0xd5, new OpCode (0xd5, Instruction.CMP, Addressing.DirectIndexedX, new FetchByte (2), 4, CycleOption.Add1CycleIf16bitAcccess | CycleOption.Add1CycleIfDPRegNonZero) },
            // CMP [dp],Y		Compare Accumulator with Memory	D7	DP Indirect Long Indexed, Y	N-----ZC	2	6[^1][^2]
            { 0xd7, new OpCode (0xd7, Instruction.CMP, Addressing.DirectIndirectLongIndexedY, new FetchByte (2), 6, CycleOption.Add1CycleIf16bitAcccess | CycleOption.Add1CycleIfDPRegNonZero) },
            // CMP addr,Y		Compare Accumulator with Memory	D9	Absolute Indexed,Y	N-----ZC	3	4[^1][^3]
            { 0xd9, new OpCode (0xd9, Instruction.CMP, Addressing.AbsoluteIndexedY, new FetchByte (3), 4, CycleOption.Add1CycleIf16bitAcccess | CycleOption.Add1CycleIfPageBoundaryOrXRegZero) },
            // CMP addr,X		Compare Accumulator with Memory	DD	Absolute Indexed,X	N-----ZC	3	4[^1][^3]
            { 0xdd, new OpCode (0xdd, Instruction.CMP, Addressing.AbsoluteIndexedX, new FetchByte (3), 4, CycleOption.Add1CycleIf16bitAcccess | CycleOption.Add1CycleIfPageBoundaryOrXRegZero) },
            // CMP long,X		Compare Accumulator with Memory	DF	Absolute Long Indexed,X	N-----ZC	4	5[^1]
            { 0xdf, new OpCode (0xdf, Instruction.CMP, Addressing.AbsoluteLongIndexedX, new FetchByte (4), 5, CycleOption.Add1CycleIf16bitAcccess) },
            // COP const		Co-Processor Enable	02	Stack/Interrupt	----DI--	2[^13]	7[^7]
            { 0x02, new OpCode (0x02, Instruction.COP, Addressing.Implied, new FetchByte (2, AddMode.Add1ByteForSignatureByte), 7, CycleOption.Add1CycleIfNativeMode) },
            // CPX #const		Compare Index Register X with Memory	E0	Immediate	N-----ZC	2[^14]	2[^8]
            { 0xe0, new OpCode (0xe0, Instruction.CPX, Addressing.Immediate, new FetchByte (2, AddMode.Add1ByteIfXRegZero), 2, CycleOption.Add1CycleIfXZero) },
            // CPX dp		Compare Index Register X with Memory	E4	Direct Page	N-----ZC	2	3[^2][^8]
            { 0xe4, new OpCode (0xe4, Instruction.CPX, Addressing.Direct, new FetchByte (2), 3, CycleOption.Add1CycleIfDPRegNonZero | CycleOption.Add1CycleIfXZero) },
            // CPX addr		Compare Index Register X with Memory	EC	Absolute	N-----ZC	3	4[^8]
            { 0xec, new OpCode (0xec, Instruction.CPX, Addressing.Absolute, new FetchByte (3), 4, CycleOption.Add1CycleIfXZero) },
            // CPY #const		Compare Index Register Y with Memory	C0	Immediate	N-----ZC	2[^14]	2[^8]
            { 0xc0, new OpCode (0xc0, Instruction.CPY, Addressing.Immediate, new FetchByte (2, AddMode.Add1ByteIfXRegZero), 2, CycleOption.Add1CycleIfXZero) },
            // CPY dp		Compare Index Register Y with Memory	C4	Direct Page	N-----ZC	2	3[^2][^8]
            { 0xc4, new OpCode (0xc4, Instruction.CPY, Addressing.Direct, new FetchByte (2), 3, CycleOption.Add1CycleIfDPRegNonZero | CycleOption.Add1CycleIfXZero) },
            // CPY addr		Compare Index Register Y with Memory	CC	Absolute	N-----ZC	3	4[^8]
            { 0xcc, new OpCode (0xcc, Instruction.CPY, Addressing.Absolute, new FetchByte (3), 4, CycleOption.Add1CycleIfXZero) },
            // DEC A	DEA	Decrement	3A	Accumulator	N-----Z-	1	2
            { 0x3a, new OpCode (0x3a, Instruction.DEC, Addressing.Accumulator, new FetchByte (1), 2, CycleOption.None) },
            // DEC dp		Decrement	C6	Direct Page	N-----Z-	2	5[^2][^4]
            { 0xc6, new OpCode (0xc6, Instruction.DEC, Addressing.Direct, new FetchByte (2), 5, CycleOption.Add1CycleIfDPRegNonZero | CycleOption.Add2CycleIf16bitaccess) },
            // DEC addr		Decrement	CE	Absolute	N-----Z-	3	6[^4]
            { 0xce, new OpCode (0xce, Instruction.DEC, Addressing.Absolute, new FetchByte (3), 6, CycleOption.Add2CycleIf16bitaccess) },
            // DEC dp,X		Decrement	D6	DP Indexed,X	N-----Z-	2	6[^2][^4]
            { 0xd6, new OpCode (0xd6, Instruction.DEC, Addressing.DirectIndexedX, new FetchByte (2), 6, CycleOption.Add1CycleIfDPRegNonZero | CycleOption.Add2CycleIf16bitaccess) },
            // DEC addr,X		Decrement	DE	Absolute Indexed,X	N-----Z-	3	7[^4]
            { 0xde, new OpCode (0xde, Instruction.DEC, Addressing.AbsoluteIndexedX, new FetchByte (3), 7, CycleOption.Add2CycleIf16bitaccess) },
            // DEX		Decrement Index Register X	CA	Implied	N-----Z-	1	2
            { 0xca, new OpCode (0xca, Instruction.DEX, Addressing.Implied, new FetchByte (1), 2, CycleOption.None) },
            // DEY		Decrement Index Register Y	88	Implied	N-----Z-	1	2
            { 0x88, new OpCode (0x88, Instruction.DEY, Addressing.Implied, new FetchByte (1), 2, CycleOption.None) },
            // EOR (_dp,_X)		Exclusive-OR Accumulator with Memory	41	DP Indexed Indirect,X	N-----Z-	2	6[^1][^2]
            { 0x41, new OpCode (0x41, Instruction.EOR, Addressing.DirectIndexedIndirectX, new FetchByte (2), 6, CycleOption.Add1CycleIf16bitAcccess | CycleOption.Add1CycleIfDPRegNonZero) },
            // EOR sr,S		Exclusive-OR Accumulator with Memory	43	Stack Relative	N-----Z-	2	4[^1]
            { 0x43, new OpCode (0x43, Instruction.EOR, Addressing.StackRelative, new FetchByte (2), 4, CycleOption.Add1CycleIf16bitAcccess) },
            // EOR dp		Exclusive-OR Accumulator with Memory	45	Direct Page	N-----Z-	2	3[^1][^2]
            { 0x45, new OpCode (0x45, Instruction.EOR, Addressing.Direct, new FetchByte (2), 3, CycleOption.Add1CycleIf16bitAcccess | CycleOption.Add1CycleIfDPRegNonZero) },
            // EOR [dp]		Exclusive-OR Accumulator with Memory	47	DP Indirect Long	N-----Z-	2	6[^1][^2]
            { 0x47, new OpCode (0x47, Instruction.EOR, Addressing.DirectIndirectLong, new FetchByte (2), 6, CycleOption.Add1CycleIf16bitAcccess | CycleOption.Add1CycleIfDPRegNonZero) },
            // EOR #const		Exclusive-OR Accumulator with Memory	49	Immediate	N-----Z-	2[^12]	2[^1]
            { 0x49, new OpCode (0x49, Instruction.EOR, Addressing.Immediate, new FetchByte (2, AddMode.Add1ByteIfMRegZero), 2, CycleOption.Add1CycleIf16bitAcccess) },
            // EOR addr		Exclusive-OR Accumulator with Memory	4D	Absolute	N-----Z-	3	4[^1]
            { 0x4d, new OpCode (0x4d, Instruction.EOR, Addressing.Absolute, new FetchByte (3), 4, CycleOption.Add1CycleIf16bitAcccess) },
            // EOR long		Exclusive-OR Accumulator with Memory	4F	Absolute Long	N-----Z-	4	5[^1]
            { 0x4f, new OpCode (0x4f, Instruction.EOR, Addressing.AbsoluteLong, new FetchByte (4), 5, CycleOption.Add1CycleIf16bitAcccess) },
            // EOR (dp),Y		Exclusive-OR Accumulator with Memory	51	DP Indirect Indexed, Y	N-----Z-	2	5[^1][^2][^3]
            { 0x51, new OpCode (0x51, Instruction.EOR, Addressing.DirectIndirectIndexedY, new FetchByte (2), 5, CycleOption.Add1CycleIf16bitAcccess | CycleOption.Add1CycleIfDPRegNonZero | CycleOption.Add1CycleIfPageBoundaryOrXRegZero) },
            // EOR (dp)		Exclusive-OR Accumulator with Memory	52	DP Indirect	N-----Z-	2	5[^1][^2]
            { 0x52, new OpCode (0x52, Instruction.EOR, Addressing.DirectIndirect, new FetchByte (2), 5, CycleOption.Add1CycleIf16bitAcccess | CycleOption.Add1CycleIfDPRegNonZero) },
            // EOR (sr,S),Y		Exclusive-OR Accumulator with Memory	53	SR Indirect Indexed,Y	N-----Z-	2	7[^1]
            { 0x53, new OpCode (0x53, Instruction.EOR, Addressing.StackRelativeIndirectIndexedY, new FetchByte (2), 7, CycleOption.Add1CycleIf16bitAcccess) },
            // EOR dp,X		Exclusive-OR Accumulator with Memory	55	DP Indexed,X	N-----Z-	2	4[^1][^2]
            { 0x55, new OpCode (0x55, Instruction.EOR, Addressing.DirectIndexedX, new FetchByte (2), 4, CycleOption.Add1CycleIf16bitAcccess | CycleOption.Add1CycleIfDPRegNonZero) },
            // EOR [dp],Y		Exclusive-OR Accumulator with Memory	57	DP Indirect Long Indexed, Y	N-----Z-	2	6[^1][^2]
            { 0x57, new OpCode (0x57, Instruction.EOR, Addressing.DirectIndirectLongIndexedY, new FetchByte (2), 6, CycleOption.Add1CycleIf16bitAcccess | CycleOption.Add1CycleIfDPRegNonZero) },
            // EOR addr,Y		Exclusive-OR Accumulator with Memory	59	Absolute Indexed,Y	N-----Z-	3	4[^1][^3]
            { 0x59, new OpCode (0x59, Instruction.EOR, Addressing.AbsoluteIndexedY, new FetchByte (3), 4, CycleOption.Add1CycleIf16bitAcccess | CycleOption.Add1CycleIfPageBoundaryOrXRegZero) },
            // EOR addr,X		Exclusive-OR Accumulator with Memory	5D	Absolute Indexed,X	N-----Z-	3	4[^1][^3]
            { 0x5d, new OpCode (0x5d, Instruction.EOR, Addressing.AbsoluteIndexedX, new FetchByte (3), 4, CycleOption.Add1CycleIf16bitAcccess | CycleOption.Add1CycleIfPageBoundaryOrXRegZero) },
            // EOR long,X		Exclusive-OR Accumulator with Memory	5F	Absolute Long Indexed,X	N-----Z-	4	5[^1]
            { 0x5f, new OpCode (0x5f, Instruction.EOR, Addressing.AbsoluteLongIndexedX, new FetchByte (4), 5, CycleOption.Add1CycleIf16bitAcccess) },
            // INC A	INA	Increment	1A	Accumulator	N-----Z-	1	2
            { 0x1a, new OpCode (0x1a, Instruction.INC, Addressing.Accumulator, new FetchByte (1), 2, CycleOption.None) },
            // INC dp		Increment	E6	Direct Page	N-----Z-	2	5[^2][^4]
            { 0xe6, new OpCode (0xe6, Instruction.INC, Addressing.Direct, new FetchByte (2), 5, CycleOption.Add1CycleIfDPRegNonZero | CycleOption.Add2CycleIf16bitaccess) },
            // INC addr		Increment	EE	Absolute	N-----Z-	3	6[^4]
            { 0xee, new OpCode (0xee, Instruction.INC, Addressing.Absolute, new FetchByte (3), 6, CycleOption.Add2CycleIf16bitaccess) },
            // INC dp,X		Increment	F6	DP Indexed,X	N-----Z-	2	6[^2][^4]
            { 0xf6, new OpCode (0xf6, Instruction.INC, Addressing.DirectIndexedX, new FetchByte (2), 6, CycleOption.Add1CycleIfDPRegNonZero | CycleOption.Add2CycleIf16bitaccess) },
            // INC addr,X		Increment	FE	Absolute Indexed,X	N-----Z-	3	7[^4]
            { 0xfe, new OpCode (0xfe, Instruction.INC, Addressing.AbsoluteIndexedX, new FetchByte (3), 7, CycleOption.Add2CycleIf16bitaccess) },
            // INX		Increment Index Register X	E8	Implied	N-----Z-	1	2
            { 0xe8, new OpCode (0xe8, Instruction.INX, Addressing.Implied, new FetchByte (1), 2, CycleOption.None) },
            // INY		Increment Index Register Y	C8	Implied	N-----Z-	1	2
            { 0xc8, new OpCode (0xc8, Instruction.INY, Addressing.Implied, new FetchByte (1), 2, CycleOption.None) },
            // JMP addr		Jump	4C	Absolute		3	3
            { 0x4c, new OpCode (0x4c, Instruction.JMP, Addressing.Absolute, new FetchByte (3), 3, CycleOption.None) },
            // JMP long	JML	Jump	5C	Absolute Long		4	4
            { 0x5c, new OpCode (0x5c, Instruction.JMP, Addressing.AbsoluteLong, new FetchByte (4), 4, CycleOption.None) },
            // JMP (addr)		Jump	6C	Absolute Indirect		3	5
            { 0x6c, new OpCode (0x6c, Instruction.JMP, Addressing.AbsoluteIndirect, new FetchByte (3), 5, CycleOption.None) },
            // JMP (addr,X)		Jump	7C	Absolute Indexed Indirect		3	6
            { 0x7c, new OpCode (0x7c, Instruction.JMP, Addressing.AbsoluteIndexedIndirectX, new FetchByte (3), 6, CycleOption.None) },
            // JMP [addr]	JML	Jump	DC	Absolute Indirect Long		3	6
            { 0xdc, new OpCode (0xdc, Instruction.JMP, Addressing.AbsoluteIndirectLong, new FetchByte (3), 6, CycleOption.None) },
            // JSR addr		Jump to Subroutine	20	Absolute		3	6
            { 0x20, new OpCode (0x20, Instruction.JSR, Addressing.Absolute, new FetchByte (3), 6, CycleOption.None) },
            // JSR long	JSL	Jump to Subroutine	22	Absolute Long		4	8
            { 0x22, new OpCode (0x22, Instruction.JSR, Addressing.AbsoluteLong, new FetchByte (4), 8, CycleOption.None) },
            // JSR (addr,X))		Jump to Subroutine	FC	Absolute Indexed Indirect		3	8
            { 0xfc, new OpCode (0xfc, Instruction.JSR, Addressing.AbsoluteIndexedIndirectX, new FetchByte (3), 8, CycleOption.None) },
            // LDA (_dp,_X)		Load Accumulator from Memory	A1	DP Indexed Indirect,X	N-----Z-	2	6[^1][^2]
            { 0xa1, new OpCode (0xa1, Instruction.LDA, Addressing.DirectIndexedIndirectX, new FetchByte (2), 6, CycleOption.Add1CycleIf16bitAcccess | CycleOption.Add1CycleIfDPRegNonZero) },
            // LDA sr,S		Load Accumulator from Memory	A3	Stack Relative	N-----Z-	2	4[^1]
            { 0xa3, new OpCode (0xa3, Instruction.LDA, Addressing.StackRelative, new FetchByte (2), 4, CycleOption.Add1CycleIf16bitAcccess) },
            // LDA dp		Load Accumulator from Memory	A5	Direct Page	N-----Z-	2	3[^1][^2]
            { 0xa5, new OpCode (0xa5, Instruction.LDA, Addressing.Direct, new FetchByte (2), 3, CycleOption.Add1CycleIf16bitAcccess | CycleOption.Add1CycleIfDPRegNonZero) },
            // LDA [dp]		Load Accumulator from Memory	A7	DP Indirect Long	N-----Z-	2	6[^1][^2]
            { 0xa7, new OpCode (0xa7, Instruction.LDA, Addressing.DirectIndirectLong, new FetchByte (2), 6, CycleOption.Add1CycleIf16bitAcccess | CycleOption.Add1CycleIfDPRegNonZero) },
            // LDA #const		Load Accumulator from Memory	A9	Immediate	N-----Z-	2[^12]	2[^1]
            { 0xa9, new OpCode (0xa9, Instruction.LDA, Addressing.Immediate, new FetchByte (2, AddMode.Add1ByteIfMRegZero), 2, CycleOption.Add1CycleIf16bitAcccess) },
            // LDA addr		Load Accumulator from Memory	AD	Absolute	N-----Z-	3	4[^1]
            { 0xad, new OpCode (0xad, Instruction.LDA, Addressing.Absolute, new FetchByte (3), 4, CycleOption.Add1CycleIf16bitAcccess) },
            // LDA long		Load Accumulator from Memory	AF	Absolute Long	N-----Z-	4	5[^1]
            { 0xaf, new OpCode (0xaf, Instruction.LDA, Addressing.AbsoluteLong, new FetchByte (4), 5, CycleOption.Add1CycleIf16bitAcccess) },
            // LDA (dp),Y		Load Accumulator from Memory	B1	DP Indirect Indexed, Y	N-----Z-	2	5[^1][^2][^3]
            { 0xb1, new OpCode (0xb1, Instruction.LDA, Addressing.DirectIndirectIndexedY, new FetchByte (2), 5, CycleOption.Add1CycleIf16bitAcccess | CycleOption.Add1CycleIfDPRegNonZero | CycleOption.Add1CycleIfPageBoundaryOrXRegZero) },
            // LDA (dp)		Load Accumulator from Memory	B2	DP Indirect	N-----Z-	2	5[^1][^2]
            { 0xb2, new OpCode (0xb2, Instruction.LDA, Addressing.DirectIndirect, new FetchByte (2), 5, CycleOption.Add1CycleIf16bitAcccess | CycleOption.Add1CycleIfDPRegNonZero) },
            // LDA (sr,S),Y		Load Accumulator from Memory	B3	SR Indirect Indexed,Y	N-----Z-	2	7[^1]
            { 0xb3, new OpCode (0xb3, Instruction.LDA, Addressing.StackRelativeIndirectIndexedY, new FetchByte (2), 7, CycleOption.Add1CycleIf16bitAcccess) },
            // LDA dp,X		Load Accumulator from Memory	B5	DP Indexed,X	N-----Z-	2	4[^1][^2]
            { 0xb5, new OpCode (0xb5, Instruction.LDA, Addressing.DirectIndexedX, new FetchByte (2), 4, CycleOption.Add1CycleIf16bitAcccess | CycleOption.Add1CycleIfDPRegNonZero) },
            // LDA [dp],Y		Load Accumulator from Memory	B7	DP Indirect Long Indexed, Y	N-----Z-	2	6[^1][^2]
            { 0xb7, new OpCode (0xb7, Instruction.LDA, Addressing.DirectIndirectLongIndexedY, new FetchByte (2), 6, CycleOption.Add1CycleIf16bitAcccess | CycleOption.Add1CycleIfDPRegNonZero) },
            // LDA addr,Y		Load Accumulator from Memory	B9	Absolute Indexed,Y	N-----Z-	3	4[^1][^3]
            { 0xb9, new OpCode (0xb9, Instruction.LDA, Addressing.AbsoluteIndexedY, new FetchByte (3), 4, CycleOption.Add1CycleIf16bitAcccess | CycleOption.Add1CycleIfPageBoundaryOrXRegZero) },
            // LDA addr,X		Load Accumulator from Memory	BD	Absolute Indexed,X	N-----Z-	3	4[^1][^3]
            { 0xbd, new OpCode (0xbd, Instruction.LDA, Addressing.AbsoluteIndexedX, new FetchByte (3), 4, CycleOption.Add1CycleIf16bitAcccess | CycleOption.Add1CycleIfPageBoundaryOrXRegZero) },
            // LDA long,X		Load Accumulator from Memory	BF	Absolute Long Indexed,X	N-----Z-	4	5[^1]
            { 0xbf, new OpCode (0xbf, Instruction.LDA, Addressing.AbsoluteLongIndexedX, new FetchByte (4), 5, CycleOption.Add1CycleIf16bitAcccess) },
            // LDX #const		Load Index Register X from Memory	A2	Immediate	N-----Z-	2[^14]	2[^8]
            { 0xa2, new OpCode (0xa2, Instruction.LDX, Addressing.Immediate, new FetchByte (2, AddMode.Add1ByteIfXRegZero), 2, CycleOption.Add1CycleIfXZero) },
            // LDX dp		Load Index Register X from Memory	A6	Direct Page	N-----Z-	2	3[^2][^8]
            { 0xa6, new OpCode (0xa6, Instruction.LDX, Addressing.Direct, new FetchByte (2), 3, CycleOption.Add1CycleIfDPRegNonZero | CycleOption.Add1CycleIfXZero) },
            // LDX addr		Load Index Register X from Memory	AE	Absolute	N-----Z-	3	4[^8]
            { 0xae, new OpCode (0xae, Instruction.LDX, Addressing.Absolute, new FetchByte (3), 4, CycleOption.Add1CycleIfXZero) },
            // LDX dp,Y		Load Index Register X from Memory	B6	DP Indexed,Y	N-----Z-	2	4[^2][^8]
            { 0xb6, new OpCode (0xb6, Instruction.LDX, Addressing.DirectIndexedY, new FetchByte (2), 4, CycleOption.Add1CycleIfDPRegNonZero | CycleOption.Add1CycleIfXZero) },
            // LDX addr,Y		Load Index Register X from Memory	BE	Absolute Indexed,Y	N-----Z-	3	4[^3][^8]
            { 0xbe, new OpCode (0xbe, Instruction.LDX, Addressing.AbsoluteIndexedY, new FetchByte (3), 4, CycleOption.Add1CycleIfPageBoundaryOrXRegZero | CycleOption.Add1CycleIfXZero) },
            // LDY #const		Load Index Register Y from Memory	A0	Immediate	N-----Z-	2[^14]	2[^8]
            { 0xa0, new OpCode (0xa0, Instruction.LDY, Addressing.Immediate, new FetchByte (2, AddMode.Add1ByteIfXRegZero), 2, CycleOption.Add1CycleIfXZero) },
            // LDY dp		Load Index Register Y from Memory	A4	Direct Page	N-----Z-	2	3[^2][^8]
            { 0xa4, new OpCode (0xa4, Instruction.LDY, Addressing.Direct, new FetchByte (2), 3, CycleOption.Add1CycleIfDPRegNonZero | CycleOption.Add1CycleIfXZero) },
            // LDY addr		Load Index Register Y from Memory	AC	Absolute	N-----Z-	3	4[^8]
            { 0xac, new OpCode (0xac, Instruction.LDY, Addressing.Absolute, new FetchByte (3), 4, CycleOption.Add1CycleIfXZero) },
            // LDY dp,X		Load Index Register Y from Memory	B4	DP Indexed,X	N-----Z-	2	4[^2][^8]
            { 0xb4, new OpCode (0xb4, Instruction.LDY, Addressing.DirectIndexedX, new FetchByte (2), 4, CycleOption.Add1CycleIfDPRegNonZero | CycleOption.Add1CycleIfXZero) },
            // LDY addr,X		Load Index Register Y from Memory	BC	Absolute Indexed,X	N-----Z-	3	4[^3][^8]
            { 0xbc, new OpCode (0xbc, Instruction.LDY, Addressing.AbsoluteIndexedX, new FetchByte (3), 4, CycleOption.Add1CycleIfPageBoundaryOrXRegZero | CycleOption.Add1CycleIfXZero) },
            // LSR dp		Logical Shift Memory or Accumulator Right	46	Direct Page	N-----ZC	2	5[^2][^4]
            { 0x46, new OpCode (0x46, Instruction.LSR, Addressing.Direct, new FetchByte (2), 5, CycleOption.Add1CycleIfDPRegNonZero | CycleOption.Add2CycleIf16bitaccess) },
            // LSR A		Logical Shift Memory or Accumulator Right	4A	Accumulator	N-----ZC	1	2
            { 0x4a, new OpCode (0x4a, Instruction.LSR, Addressing.Accumulator, new FetchByte (1), 2, CycleOption.None) },
            // LSR addr		Logical Shift Memory or Accumulator Right	4E	Absolute	N-----ZC	3	6[^4]
            { 0x4e, new OpCode (0x4e, Instruction.LSR, Addressing.Absolute, new FetchByte (3), 6, CycleOption.Add2CycleIf16bitaccess) },
            // LSR dp,X		Logical Shift Memory or Accumulator Right	56	DP Indexed,X	N-----ZC	2	6[^2][^4]
            { 0x56, new OpCode (0x56, Instruction.LSR, Addressing.DirectIndexedX, new FetchByte (2), 6, CycleOption.Add1CycleIfDPRegNonZero | CycleOption.Add2CycleIf16bitaccess) },
            // LSR addr,X		Logical Shift Memory or Accumulator Right	5E	Absolute Indexed,X	N-----ZC	3	7[^4]
            { 0x5e, new OpCode (0x5e, Instruction.LSR, Addressing.AbsoluteIndexedX, new FetchByte (3), 7, CycleOption.Add2CycleIf16bitaccess) },
            // MVN srcbk,destbk		Block Move Negative	54	Block Move		3	1[^3]
            { 0x54, new OpCode (0x54, Instruction.MVN, Addressing.BlockMove, new FetchByte (3), 1, CycleOption.Add2CycleIf16bitaccess) },
            // MVP srcbk,destbk		Block Move Positive	44	Block Move		3	1[^3]
            { 0x44, new OpCode (0x44, Instruction.MVN, Addressing.BlockMove, new FetchByte (3), 1, CycleOption.Add2CycleIf16bitaccess) },
            // NOP		No Operation	EA	Implied		1	2
            { 0xea, new OpCode (0xea, Instruction.NOP, Addressing.Implied, new FetchByte (1), 2, CycleOption.None) },
            // ORA (_dp,_X)		OR Accumulator with Memory	01	DP Indexed Indirect,X	N-----Z-	2	6[^1][^2]
            // ORA sr,S		OR Accumulator with Memory	03	Stack Relative	N-----Z-	2	4[^1]
            // ORA dp		OR Accumulator with Memory	05	Direct Page	N-----Z-	2	3[^1][^2]
            // ORA [dp]		OR Accumulator with Memory	07	DP Indirect Long	N-----Z-	2	6[^1][^2]
            // ORA #const		OR Accumulator with Memory	09	Immediate	N-----Z-	2[^12]	2[^1]
            // ORA addr		OR Accumulator with Memory	0D	Absolute	N-----Z-	3	4[^1]
            // ORA long		OR Accumulator with Memory	0F	Absolute Long	N-----Z-	4	5[^1]
            // ORA (dp),Y		OR Accumulator with Memory	11	DP Indirect Indexed, Y	N-----Z-	2	5[^1][^2][^3]
            // ORA (dp)		OR Accumulator with Memory	12	DP Indirect	N-----Z-	2	5[^1][^2]
            // ORA (sr,S),Y		OR Accumulator with Memory	13	SR Indirect Indexed,Y	N-----Z-	2	7[^1]
            // ORA dp,X		OR Accumulator with Memory	15	DP Indexed,X	N-----Z-	2	4[^1][^2]
            // ORA [dp],Y		OR Accumulator with Memory	17	DP Indirect Long Indexed, Y	N-----Z-	2	6[^1][^2]
            // ORA addr,Y		OR Accumulator with Memory	19	Absolute Indexed,Y	N-----Z-	3	4[^1][^3]
            // ORA addr,X		OR Accumulator with Memory	1D	Absolute Indexed,X	N-----Z-	3	4[^1][^3]
            // ORA long,X		OR Accumulator with Memory	1F	Absolute Long Indexed,X	N-----Z-	4	5[^1]
            // PEA addr		Push Effective Absolute Address	F4	Stack (Absolute)		3	5
            // PEI (dp)		Push Effective Indirect Address	D4	Stack (DP Indirect)		2	6[^2]
            // PER label		Push Effective PC Relative Indirect Address	62	Stack (PC Relative Long)		3	6
            // PHA		Push Accumulator	48	Stack (Push)		1	3[^1]
            // PHB		Push Data Bank Register	8B	Stack (Push)		1	3
            // PHD		Push Direct Page Register	0B	Stack (Push)		1	4
            // PHK		Push Program Bank Register	4B	Stack (Push)		1	3
            // PHP		Push Processor Status Register	08	Stack (Push)		1	3
            // PHX		Push Index Register X	DA	Stack (Push)		1	3[^8]
            // PHY		Push Index Register Y	5A	Stack (Push)		1	3[^8]
            // PLA		Pull Accumulator	68	Stack (Pull)	N-----Z-	1	4[^1]
            // PLB		Pull Data Bank Register	AB	Stack (Pull)	N-----Z-	1	4
            // PLD		Pull Direct Page Register	2B	Stack (Pull)	N-----Z-	1	5
            // PLP		Pull Processor Status Register	28	Stack (Pull)	NVMXDIZC	1	4
            // PLX		Pull Index Register X	FA	Stack (Pull)	N-----Z-	1	4[^8]
            // PLY		Pull Index Register Y	7A	Stack (Pull)	N-----Z-	1	4[^8]
            // REP #const		Reset Processor Status Bits	C2	Immediate	NVMXDIZC	2	3
            // ROL dp		Rotate Memory or Accumulator Left	26	Direct Page	N-----ZC	2	5[^2][^4]
            // ROL A		Rotate Memory or Accumulator Left	2A	Accumulator	N-----ZC	1	2
            // ROL addr		Rotate Memory or Accumulator Left	2E	Absolute	N-----ZC	3	6[^4]
            // ROL dp,X		Rotate Memory or Accumulator Left	36	DP Indexed,X	N-----ZC	2	6[^2][^4]
            // ROL addr,X		Rotate Memory or Accumulator Left	3E	Absolute Indexed,X	N-----ZC	3	7[^4]
            // ROR dp		Rotate Memory or Accumulator Right	66	Direct Page	N-----ZC	2	5[^2][^4]
            // ROR A		Rotate Memory or Accumulator Right	6A	Accumulator	N-----ZC	1	2
            // ROR addr		Rotate Memory or Accumulator Right	6E	Absolute	N-----ZC	3	6[^4]
            // ROR dp,X		Rotate Memory or Accumulator Right	76	DP Indexed,X	N-----ZC	2	6[^2][^4]
            // ROR addr,X		Rotate Memory or Accumulator Right	7E	Absolute Indexed,X	N-----ZC	3	7[^4]
            // RTI		Return from Interrupt	40	Stack (RTI)	NVMXDIZC	1	6[^7]
            // RTL		Return from Subroutine Long	6B	Stack (RTL)		1	6
            // RTS		Return from Subroutine	60	Stack (RTS)		1	6
            // SBC (_dp,_X)		Subtract with Borrow from Accumulator	E1	DP Indexed Indirect,X	NV----ZC	2	6[^1][^2]
            // SBC sr,S		Subtract with Borrow from Accumulator	E3	Stack Relative	NV----ZC	2	4[^1]
            // SBC dp		Subtract with Borrow from Accumulator	E5	Direct Page	NV----ZC	2	3[^1][^2]
            // SBC [dp]		Subtract with Borrow from Accumulator	E7	DP Indirect Long	NV----ZC	2	6[^1][^2]
            // SBC #const		Subtract with Borrow from Accumulator	E9	Immediate	NV----ZC	2[^12]	2[^1]
            // SBC addr		Subtract with Borrow from Accumulator	ED	Absolute	NV----ZC	3	4[^1]
            // SBC long		Subtract with Borrow from Accumulator	EF	Absolute Long	NV----ZC	4	5[^1]
            // SBC (dp),Y		Subtract with Borrow from Accumulator	F1	DP Indirect Indexed, Y	NV----ZC	2	5[^1][^2][^3]
            // SBC (dp)		Subtract with Borrow from Accumulator	F2	DP Indirect	NV----ZC	2	5[^1][^2]
            // SBC (sr,S),Y		Subtract with Borrow from Accumulator	F3	SR Indirect Indexed,Y	NV----ZC	2	7[^1]
            // SBC dp,X		Subtract with Borrow from Accumulator	F5	DP Indexed,X	NV----ZC	2	4[^1][^2]
            // SBC [dp],Y		Subtract with Borrow from Accumulator	F7	DP Indirect Long Indexed, Y	NV----ZC	2	6[^1][^2]
            // SBC addr,Y		Subtract with Borrow from Accumulator	F9	Absolute Indexed,Y	NV----ZC	3	4[^1][^3]
            // SBC addr,X		Subtract with Borrow from Accumulator	FD	Absolute Indexed,X	NV----ZC	3	4[^1][^3]
            // SBC long,X		Subtract with Borrow from Accumulator	FF	Absolute Long Indexed,X	NV----ZC	4	5[^1]
            // SEC		Set Carry Flag	38	Implied	-------C	1	2
            // SED		Set Decimal Flag	F8	Implied	----D---	1	2
            // SEI		Set Interrupt Disable Flag	78	Implied	-----I--	1	2
            // SEP #const		Reset Processor Status Bits	E2	Immediate	NVMXDIZC	2	3
            // STA (_dp,_X)		Store Accumulator to Memory	81	DP Indexed Indirect,X		2	6[^1][^2]
            // STA sr,S		Store Accumulator to Memory	83	Stack Relative		2	4[^1]
            // STA dp		Store Accumulator to Memory	85	Direct Page		2	3[^1][^2]
            // STA [dp]		Store Accumulator to Memory	87	DP Indirect Long		2	6[^1][^2]
            // STA addr		Store Accumulator to Memory	8D	Absolute		3	4[^1]
            // STA long		Store Accumulator to Memory	8F	Absolute Long		4	5[^1]
            // STA (dp),Y		Store Accumulator to Memory	91	DP Indirect Indexed, Y		2	6[^1][^2]
            // STA (dp)		Store Accumulator to Memory	92	DP Indirect		2	5[^1][^2]
            // STA (sr,S),Y		Store Accumulator to Memory	93	SR Indirect Indexed,Y		2	7[^1]
            // STA _dp_X		Store Accumulator to Memory	95	DP Indexed,X		2	4[^1][^2]
            // STA [dp],Y		Store Accumulator to Memory	97	DP Indirect Long Indexed, Y		2	6[^1][^2]
            // STA addr,Y		Store Accumulator to Memory	99	Absolute Indexed,Y		3	5[^1]
            // STA addr,X		Store Accumulator to Memory	9D	Absolute Indexed,X		3	5[^1]
            // STA long,X		Store Accumulator to Memory	9F	Absolute Long Indexed,X		4	5[^1]
            // STP		Stop Processor	DB	Implied		1	3[^9]
            // STX dp		Store Index Register X to Memory	86	Direct Page		2	3[^2][^8]
            // STX addr		Store Index Register X to Memory	8E	Absolute		3	4[^8]
            // STX dp,Y		Store Index Register X to Memory	96	DP Indexed,Y		2	4[^2][^8]
            // STY dp		Store Index Register Y to Memory	84	Direct Page		2	3[^2][^8]
            // STY addr		Store Index Register Y to Memory	8C	Absolute		3	4[^8]
            // STY dp,X		Store Index Register Y to Memory	94	DP Indexed,X		2	4[^2][^8]
            // STZ dp		Store Zero to Memory	64	Direct Page		2	3[^1][^2]
            // STZ dp,X		Store Zero to Memory	74	DP Indexed,X		2	4[^1][^2]
            // STZ addr		Store Zero to Memory	9C	Absolute		3	4[^1]
            // STZ addr,X		Store Zero to Memory	9E	Absolute Indexed,X		3	5[^1]
            // TAX		Transfer Accumulator to Index Register X	AA	Implied	N-----Z-	1	2
            // TAY		Transfer Accumulator to Index Register Y	A8	Implied	N-----Z-	1	2
            // TCD		Transfer 16-bit Accumulator to Direct Page Register	5B	Implied	N-----Z-	1	2
            // TCS		Transfer 16-bit Accumulator to Stack Pointer	1B	Implied		1	2
            // TDC		Transfer Direct Page Register to 16-bit Accumulator	7B	Implied	N-----Z-	1	2
            // TRB dp		Test and Reset Memory Bits Against Accumulator	14	Direct Page	------Z-	2	5[^2][^4]
            // TRB addr		Test and Reset Memory Bits Against Accumulator	1C	Absolute	------Z-	3	6[^3]
            // TSB dp		Test and Set Memory Bits Against Accumulator	04	Direct Page	------Z-	2	5[^2][^4]
            // TSB addr		Test and Set Memory Bits Against Accumulator	0C	Absolute	------Z-	3	6[^4]
            // TSC		Transfer Stack Pointer to 16-bit Accumulator	3B	Implied	N-----Z-	1	2
            // TSX		Transfer Stack Pointer to Index Register X	BA	Implied	N-----Z-	1	2
            // TXA		Transfer Index Register X to Accumulator	8A	Implied	N-----Z-	1	2
            // TXS		Transfer Index Register X to Stack Pointer	9A	Implied		1	2
            // TXY		Transfer Index Register X to Index Register Y	9B	Implied	N-----Z-	1	2
            // TYA		Transfer Index Register Y to Accumulator	98	Implied	N-----Z-	1	2
            // TYX		Transfer Index Register Y to Index Register X	BB	Implied	N-----Z-	1	2
            // WAI		Wait for Interrupt	CB	Implied		1	3[^10]
            // WDM		Reserved for Future Expansion	42			2	0[^11]
            // XBA		Exchange B and A 8-bit Accumulators	EB	Implied	N-----Z-	1	3
            // XCE		Exchange Carry and Emulation Flags	FB	Implied	--MX---CE	1	2
        };
    }
}