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
            // BCS nearlabel	BGE	Branch if Carry Set	B0	Program Counter Relative		2	2[^5][^6]
            // BEQ nearlabel		Branch if Equal	F0	Program Counter Relative		2	2[^5][^6]
            // BIT dp		Test Bits	24	Direct Page	NV----Z-	2	3[^1][^2]
            // BIT addr		Test Bits	2C	Absolute	NV----Z-	3	4[^1]
            // BIT dp,X		Test Bits	34	DP Indexed,X	NV----Z-	2	4[^1][^2]
            // BIT addr,X		Test Bits	3C	Absolute Indexed,X	NV----Z-	3	4[^1][^3]
            // BIT #const		Test Bits	89	Immediate	------Z-	2[^12]	2[^1]
            // BMI nearlabel		Branch if Minus	30	Program Counter Relative		2	2[^5][^6]
            // BNE nearlabel		Branch if Not Equal	D0	Program Counter Relative		2	2[^5][^6]
            // BPL nearlabel		Branch if Plus	10	Program Counter Relative		2	2[^5][^6]
            // BRA nearlabel		Branch Always	80	Program Counter Relative		2	3[^6]
            // BRK		Break	00	Stack/Interrupt	----DI--	2[^13]	7[^7]
            // BRL label		Branch Long Always	82	Program Counter Relative Long		3	4
            // BVC nearlabel		Branch if Overflow Clear	50	Program Counter Relative		2	2[^5][^6]
            // BVS nearlabel		Branch if Overflow Set	70	Program Counter Relative		2	2[^5][^6]
            // CLC		Clear Carry	18	Implied	-------C	1	2
            // CLD		Clear Decimal Mode Flag	D8	Implied	----D---	1	2
            // CLI		Clear Interrupt Disable Flag	58	Implied	-----I--	1	2
            // CLV		Clear Overflow Flag	B8	Implied	-V------	1	2
            // CMP (_dp,_X)		Compare Accumulator with Memory	C1	DP Indexed Indirect,X	N-----ZC	2	6[^1][^2]
            // CMP sr,S		Compare Accumulator with Memory	C3	Stack Relative	N-----ZC	2	4[^1]
            // CMP dp		Compare Accumulator with Memory	C5	Direct Page	N-----ZC	2	3[^1][^2]
            // CMP [dp]		Compare Accumulator with Memory	C7	DP Indirect Long	N-----ZC	2	6[^1][^2]
            // CMP #const		Compare Accumulator with Memory	C9	Immediate	N-----ZC	2[^12]	2[^1]
            // CMP addr		Compare Accumulator with Memory	CD	Absolute	N-----ZC	3	4[^1]
            // CMP long		Compare Accumulator with Memory	CF	Absolute Long	N-----ZC	4	5[^1]
            // CMP (dp),Y		Compare Accumulator with Memory	D1	DP Indirect Indexed, Y	N-----ZC	2	5[^1][^2][^3]
            // CMP (dp)		Compare Accumulator with Memory	D2	DP Indirect	N-----ZC	2	5[^1][^2]
            // CMP (sr,S),Y		Compare Accumulator with Memory	D3	SR Indirect Indexed,Y	N-----ZC	2	7[^1]
            // CMP dp,X		Compare Accumulator with Memory	D5	DP Indexed,X	N-----ZC	2	4[^1][^2]
            // CMP [dp],Y		Compare Accumulator with Memory	D7	DP Indirect Long Indexed, Y	N-----ZC	2	6[^1][^2]
            // CMP addr,Y		Compare Accumulator with Memory	D9	Absolute Indexed,Y	N-----ZC	3	4[^1][^3]
            // CMP addr,X		Compare Accumulator with Memory	DD	Absolute Indexed,X	N-----ZC	3	4[^1][^3]
            // CMP long,X		Compare Accumulator with Memory	DF	Absolute Long Indexed,X	N-----ZC	4	5[^1]
            // COP const		Co-Processor Enable	02	Stack/Interrupt	----DI--	2[^13]	7[^7]
            // CPX #const		Compare Index Register X with Memory	E0	Immediate	N-----ZC	2[^14]	2[^8]
            // CPX dp		Compare Index Register X with Memory	E4	Direct Page	N-----ZC	2	3[^2][^8]
            // CPX addr		Compare Index Register X with Memory	EC	Absolute	N-----ZC	3	4[^8]
            // CPY #const		Compare Index Register Y with Memory	C0	Immediate	N-----ZC	2[^14]	2[^8]
            // CPY dp		Compare Index Register Y with Memory	C4	Direct Page	N-----ZC	2	3[^2][^8]
            // CPY addr		Compare Index Register Y with Memory	CC	Absolute	N-----ZC	3	4[^8]
            // DEC A	DEA	Decrement	3A	Accumulator	N-----Z-	1	2
            // DEC dp		Decrement	C6	Direct Page	N-----Z-	2	5[^2][^4]
            // DEC addr		Decrement	CE	Absolute	N-----Z-	3	6[^4]
            // DEC dp,X		Decrement	D6	DP Indexed,X	N-----Z-	2	6[^2][^4]
            // DEC addr,X		Decrement	DE	Absolute Indexed,X	N-----Z-	3	7[^4]
            // DEX		Decrement Index Register X	CA	Implied	N-----Z-	1	2
            // DEY		Decrement Index Register Y	88	Implied	N-----Z-	1	2
            // EOR (_dp,_X)		Exclusive-OR Accumulator with Memory	41	DP Indexed Indirect,X	N-----Z-	2	6[^1][^2]
            // EOR sr,S		Exclusive-OR Accumulator with Memory	43	Stack Relative	N-----Z-	2	4[^1]
            // EOR dp		Exclusive-OR Accumulator with Memory	45	Direct Page	N-----Z-	2	3[^1][^2]
            // EOR [dp]		Exclusive-OR Accumulator with Memory	47	DP Indirect Long	N-----Z-	2	6[^1][^2]
            // EOR #const		Exclusive-OR Accumulator with Memory	49	Immediate	N-----Z-	2[^12]	2[^1]
            // EOR addr		Exclusive-OR Accumulator with Memory	4D	Absolute	N-----Z-	3	4[^1]
            // EOR long		Exclusive-OR Accumulator with Memory	4F	Absolute Long	N-----Z-	4	5[^1]
            // EOR (dp),Y		Exclusive-OR Accumulator with Memory	51	DP Indirect Indexed, Y	N-----Z-	2	5[^1][^2][^3]
            // EOR (dp)		Exclusive-OR Accumulator with Memory	52	DP Indirect	N-----Z-	2	5[^1][^2]
            // EOR (sr,S),Y		Exclusive-OR Accumulator with Memory	53	SR Indirect Indexed,Y	N-----Z-	2	7[^1]
            // EOR dp,X		Exclusive-OR Accumulator with Memory	55	DP Indexed,X	N-----Z-	2	4[^1][^2]
            // EOR [dp],Y		Exclusive-OR Accumulator with Memory	57	DP Indirect Long Indexed, Y	N-----Z-	2	6[^1][^2]
            // EOR addr,Y		Exclusive-OR Accumulator with Memory	59	Absolute Indexed,Y	N-----Z-	3	4[^1][^3]
            // EOR addr,X		Exclusive-OR Accumulator with Memory	5D	Absolute Indexed,X	N-----Z-	3	4[^1][^3]
            // EOR long,X		Exclusive-OR Accumulator with Memory	5F	Absolute Long Indexed,X	N-----Z-	4	5[^1]
            // INC A	INA	Increment	1A	Accumulator	N-----Z-	1	2
            // INC dp		Increment	E6	Direct Page	N-----Z-	2	5[^2][^4]
            // INC addr		Increment	EE	Absolute	N-----Z-	3	6[^4]
            // INC dp,X		Increment	F6	DP Indexed,X	N-----Z-	2	6[^2][^4]
            // INC addr,X		Increment	FE	Absolute Indexed,X	N-----Z-	3	7[^4]
            // INX		Increment Index Register X	E8	Implied	N-----Z-	1	2
            // INY		Increment Index Register Y	C8	Implied	N-----Z-	1	2
            // JMP addr		Jump	4C	Absolute		3	3
            // JMP long	JML	Jump	5C	Absolute Long		4	4
            // JMP (addr)		Jump	6C	Absolute Indirect		3	5
            // JMP (addr,X)		Jump	7C	Absolute Indexed Indirect		3	6
            // JMP [addr]	JML	Jump	DC	Absolute Indirect Long		3	6
            // JSR addr		Jump to Subroutine	20	Absolute		3	6
            // JSR long	JSL	Jump to Subroutine	22	Absolute Long		4	8
            // JSR (addr,X))		Jump to Subroutine	FC	Absolute Indexed Indirect		3	8
            // LDA (_dp,_X)		Load Accumulator from Memory	A1	DP Indexed Indirect,X	N-----Z-	2	6[^1][^2]
            // LDA sr,S		Load Accumulator from Memory	A3	Stack Relative	N-----Z-	2	4[^1]
            // LDA dp		Load Accumulator from Memory	A5	Direct Page	N-----Z-	2	3[^1][^2]
            // LDA [dp]		Load Accumulator from Memory	A7	DP Indirect Long	N-----Z-	2	6[^1][^2]
            // LDA #const		Load Accumulator from Memory	A9	Immediate	N-----Z-	2[^12]	2[^1]
            // LDA addr		Load Accumulator from Memory	AD	Absolute	N-----Z-	3	4[^1]
            // LDA long		Load Accumulator from Memory	AF	Absolute Long	N-----Z-	4	5[^1]
            // LDA (dp),Y		Load Accumulator from Memory	B1	DP Indirect Indexed, Y	N-----Z-	2	5[^1][^2][^3]
            // LDA (dp)		Load Accumulator from Memory	B2	DP Indirect	N-----Z-	2	5[^1][^2]
            // LDA (sr,S),Y		Load Accumulator from Memory	B3	SR Indirect Indexed,Y	N-----Z-	2	7[^1]
            // LDA dp,X		Load Accumulator from Memory	B5	DP Indexed,X	N-----Z-	2	4[^1][^2]
            // LDA [dp],Y		Load Accumulator from Memory	B7	DP Indirect Long Indexed, Y	N-----Z-	2	6[^1][^2]
            // LDA addr,Y		Load Accumulator from Memory	B9	Absolute Indexed,Y	N-----Z-	3	4[^1][^3]
            // LDA addr,X		Load Accumulator from Memory	BD	Absolute Indexed,X	N-----Z-	3	4[^1][^3]
            // LDA long,X		Load Accumulator from Memory	BF	Absolute Long Indexed,X	N-----Z-	4	5[^1]
            // LDX #const		Load Index Register X from Memory	A2	Immediate	N-----Z-	2[^14]	2[^8]
            // LDX dp		Load Index Register X from Memory	A6	Direct Page	N-----Z-	2	3[^2][^8]
            // LDX addr		Load Index Register X from Memory	AE	Absolute	N-----Z-	3	4[^8]
            // LDX dp,Y		Load Index Register X from Memory	B6	DP Indexed,Y	N-----Z-	2	4[^2][^8]
            // LDX addr,Y		Load Index Register X from Memory	BE	Absolute Indexed,Y	N-----Z-	3	4[^3][^8]
            // LDY #const		Load Index Register Y from Memory	A0	Immediate	N-----Z-	2[^14]	2[^8]
            // LDY dp		Load Index Register Y from Memory	A4	Direct Page	N-----Z-	2	3[^2][^8]
            // LDY addr		Load Index Register Y from Memory	AC	Absolute	N-----Z-	3	4[^8]
            // LDY dp,X		Load Index Register Y from Memory	B4	DP Indexed,X	N-----Z-	2	4[^2][^8]
            // LDY addr,X		Load Index Register Y from Memory	BC	Absolute Indexed,X	N-----Z-	3	4[^3][^8]
            // LSR dp		Logical Shift Memory or Accumulator Right	46	Direct Page	N-----ZC	2	5[^2][^4]
            // LSR A		Logical Shift Memory or Accumulator Right	4A	Accumulator	N-----ZC	1	2
            // LSR addr		Logical Shift Memory or Accumulator Right	4E	Absolute	N-----ZC	3	6[^4]
            // LSR dp,X		Logical Shift Memory or Accumulator Right	56	DP Indexed,X	N-----ZC	2	6[^2][^4]
            // LSR addr,X		Logical Shift Memory or Accumulator Right	5E	Absolute Indexed,X	N-----ZC	3	7[^4]
            // MVN srcbk,destbk		Block Move Negative	54	Block Move		3	1[^3]
            // MVP srcbk,destbk		Block Move Positive	44	Block Move		3	1[^3]
            // NOP		No Operation	EA	Implied		1	2
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