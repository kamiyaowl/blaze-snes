using System;
using System.Diagnostics;
using System.IO;

using BlazeSnes.Core.Common;

namespace BlazeSnes.Core.Cpu {

    /// <summary>
    /// Assembler HEXと命令の対応を示します
    /// </summary>
    public class OpCode {
        /// <summary>
        /// Clock Cycleの決定するための定義を示します
        /// </summary>
        [Flags]
        public enum CycleOption {
            None = 0, // 0.追加はない
            Add1CycleIf16bitAcccess = 0x1, // 1.16bit Access時に1cycle追加
            Add1CycleIfDPRegNonZero = 0x2, // 2.Direct Page Registerが非Zeroなら1cycle追加
            Add1CycleIfPageBoundaryOrXRegZero = 0x4, // 3.page境界を跨ぐ、x=0で1cycle追加
            Add2CycleIf16bitaccess = 0x8, // 4.16bit Access時に2cycle追加
            Add1CycleIfBranchIsTaken = 0x10, // 5.分岐した場合に1cycle追加
            Add1CycleIfBranchIsTakenAndPageCrossesInEmuMode = 0x20, // 6.EmulationModeでページ跨ぎの分岐が発生した場合に1cycle追加
            Add1CycleIfNativeMode = 0x40, // 7.NativeModeで1cycle追加
            Add1CycleIfXZero = 0x80, // 8.X=0で1cycle追加
            Add3CycleToShutdownByReset = 0x100, // 9.Processor停止に3cycle
            Add3CycleToShutdownByInterrupt = 0x200, // 10.Processor停止に3cycle
        }
        /// <summary>
        /// HEX
        /// </summary>
        /// <value></value>
        public byte Code { get; internal set; }
        /// <summary>
        /// 処理内容
        /// </summary>
        /// <value></value>
        public Instruction Inst { get; internal set; }
        /// <summary>
        /// アドレッシングモード
        /// </summary>
        /// <value></value>
        public Addressing AddressingMode { get; internal set; }
        /// <summary>
        /// 命令フェッチするbyte数(AddressingMode依存ではある気がするが)
        /// </summary>
        /// <value></value>
        public FetchByte FetchBytes { get; internal set; }
        /// <summary>
        /// 処理にかかるCPU Clock Cycle Count
        /// </summary>
        /// <value></value>
        public int Cycles { get; internal set; }
        /// <summary>
        /// Cycles計算の補助情報
        /// </summary>
        /// <value></value>
        public CycleOption Option { get; internal set; }

        public OpCode(byte code, Instruction instruction, Addressing addressing, FetchByte fetchBytes, int cycles, CycleOption option) {
            this.Code = code;
            this.Inst = instruction;
            this.AddressingMode = addressing;
            this.FetchBytes = fetchBytes;
            this.Cycles = cycles;
            this.Option = option;
        }

        public override string ToString() => $"{Code:02X}: {Inst} {AddressingMode} ({FetchBytes}bytes, {Cycles}cyc)";

        /// <summary>
        /// 必要なCPU Clock Cycle数を求めます
        /// 分岐予測までは確認できていないので、Add1CycleIfBranchIsTaken, Add1CycleIfBranchIsTakenAndPageCrossesInEmuMode は別途確認すること
        /// </summary>
        /// <param name="bus"></param>
        /// <param name="cpu"></param>
        /// <returns></returns>
        public int GetTotalCycles(in CpuRegister cpu) {
            // 特殊なオプションなし
            if (Option == CycleOption.None) {
                return this.Cycles;
            }

            // 事前計算可能なcycle数
            int c = this.Cycles;
            if (cpu.Is16bitMemoryAccess) {
                if (Option.HasFlag(CycleOption.Add1CycleIf16bitAcccess)) c += 1;
                if (Option.HasFlag(CycleOption.Add2CycleIf16bitaccess)) c += 2;
            }
            if (Option.HasFlag(CycleOption.Add1CycleIfDPRegNonZero) && cpu.DP != 0) c += 1;
            if ((Option.HasFlag(CycleOption.Add1CycleIfPageBoundaryOrXRegZero) || Option.HasFlag(CycleOption.Add1CycleIfXZero)) && cpu.XConsideringIndexReg == 0) c += 1;
            if (Option.HasFlag(CycleOption.Add1CycleIfNativeMode) && !cpu.P.Value.HasFlag(ProcessorStatusFlag.E)) c += 1;

            return c;
        }
        /// <summary>
        /// 今回のオペランド取得で進むPCのbyte数を取得します
        /// FetchByte.AddModeはフラグではないので分岐して取得可
        /// </summary>
        /// <param name="cpu"></param>
        /// <returns></returns>
        public int GetTotalArrangeBytes(in CpuRegister cpu) => this.FetchBytes.Mode switch
        {
            FetchByte.AddMode.Fixed => FetchBytes.Bytes,
            FetchByte.AddMode.Add1ByteIfMRegZero when cpu.Is16bitMemoryAccess => FetchBytes.Bytes + 1,
            FetchByte.AddMode.Add1ByteIfXRegZero when cpu.Is16bitIndexAccess => FetchBytes.Bytes + 1,
            FetchByte.AddMode.Add1ByteForSignatureByte => FetchBytes.Bytes + 1,
            _ => FetchBytes.Bytes,
        };

        /// <summary>
        /// アドレッシング解決して値を取得します
        /// </summary>
        /// <param name="bus">Mmu</param>
        /// <param name="bus">Cpu, 変更は行わない</param>
        /// <param name="isNondestructive">破壊読み出しを避ける場合はtrue</param>
        /// <returns>読み出し先ベースアドレス</returns>
        public uint GetAddr(IBusAccessible bus, in CpuRegister cpu, bool isNondestructive = false) {
            // PCは現在の命令を指した状態で呼ばれるので+1した位置から読む
            var operandBaseAddr = (uint)(cpu.PC + 1);

            // Addressing modeごとに実装
            switch (this.AddressingMode) {
                case Addressing.Immediate: {
                        return operandBaseAddr; // OpCodeの次のアドレスそのまま
                    }
                case Addressing.DirectPage: {
                        return (uint)(cpu.DP + bus.Read8(operandBaseAddr, isNondestructive));
                    }
                case Addressing.DirectPageIndexedX: {
                        return (uint)(cpu.DP + (uint)(cpu.XConsideringIndexReg + bus.Read8(operandBaseAddr, isNondestructive)));
                    }
                case Addressing.DirectPageIndexedY: {
                        return (uint)(cpu.DP + (uint)(cpu.YConsideringIndexReg + bus.Read8(operandBaseAddr, isNondestructive)));
                    }
                case Addressing.DirectPageIndirect: {
                        var interAddr = bus.Read16((uint)(cpu.DP + bus.Read8(operandBaseAddr, isNondestructive)), isNondestructive);
                        return (cpu.DataBankAddr | interAddr);
                    }
                case Addressing.DirectPageIndirectLong: {
                        return (uint)(bus.Read24((uint)(cpu.DP + bus.Read8(operandBaseAddr, isNondestructive)), isNondestructive));
                    }
                case Addressing.DirectPageIndexedIndirectX: {
                        var interAddr = bus.Read16((uint)(cpu.DP + (uint)(bus.Read8(operandBaseAddr, isNondestructive) + cpu.XConsideringIndexReg)), isNondestructive);
                        return (cpu.DataBankAddr | interAddr);
                    }
                case Addressing.DirectPageIndirectIndexedY: {
                        var interAddr = bus.Read16((uint)(cpu.DP + bus.Read8(operandBaseAddr, isNondestructive)), isNondestructive);
                        return (uint)(cpu.DataBankAddr | (uint)(interAddr + cpu.YConsideringIndexReg));
                    }
                case Addressing.DirectPageIndirectLongIndexedY: {
                        var interAddr = bus.Read24((uint)(cpu.DP + bus.Read8(operandBaseAddr, isNondestructive)), isNondestructive);
                        return (uint)(interAddr + cpu.YConsideringIndexReg);
                    }
                case Addressing.Absolute: {
                        return (uint)(cpu.DataBankAddr | bus.Read16(operandBaseAddr, isNondestructive));
                    }
                case Addressing.AbsoluteIndexedX: {
                        return (uint)(cpu.DataBankAddr | (uint)(bus.Read16(operandBaseAddr, isNondestructive) + cpu.XConsideringIndexReg));
                    }
                case Addressing.AbsoluteIndexedY: {
                        return (uint)(cpu.DataBankAddr | (uint)(bus.Read16(operandBaseAddr, isNondestructive) + cpu.YConsideringIndexReg));
                    }
                case Addressing.AbsoluteIndirect: {
                        var interAddr = (uint)(bus.Read16(operandBaseAddr, isNondestructive));
                        return (uint)(cpu.PageBankAddr | interAddr);
                    }
                case Addressing.AbsoluteIndexedIndirectX: {
                        var interAddr = (uint)(bus.Read16(operandBaseAddr + cpu.XConsideringIndexReg));
                        return (uint)(cpu.PageBankAddr | interAddr);
                    }
                case Addressing.AbsoluteLong: {
                        return (uint)(bus.Read24(operandBaseAddr, isNondestructive));
                    }
                case Addressing.AbsoluteLongIndexedX: {
                        return (uint)(bus.Read24(operandBaseAddr, isNondestructive) + cpu.XConsideringIndexReg);
                    }
                case Addressing.StackRelative: {
                        // SPは常に空き領域を示している、ここからのオフセットを1byteで指定(SP自体は変更しない)
                        // SPはPushするたびに奥のアドレスから手前に伸びる
                        var offset = (byte)bus.Read8(operandBaseAddr, isNondestructive);
                        return (uint)checked(cpu.SP + offset);
                    }
                case Addressing.ProgramCounterRelative: {
                        // PCは次の命令位置を指している前提で演算されるため、事前に足しておく
                        var nextOpcodeAddr = cpu.PC + GetTotalArrangeBytes(cpu);
                        var offset = (sbyte)bus.Read8(operandBaseAddr, isNondestructive);
                        return (uint)(nextOpcodeAddr + offset);
                    }
                case Addressing.ProgramCounterRelativeLong: {
                        // PCは次の命令位置を指している前提で演算されるため、事前に足しておく
                        var nextOpcodeAddr = cpu.PC + GetTotalArrangeBytes(cpu);
                        var offset = (short)bus.Read16(operandBaseAddr, isNondestructive);
                        return (uint)(nextOpcodeAddr + offset);
                    }
                case Addressing.Implied:
                case Addressing.Accumulator:
                case Addressing.BlockMove:
                    throw new ArgumentException("Implied, Accumulator, BlockMoveではアドレス解決できません");
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// アドレッシング解決して、読みだしたいデータアドレス、かかったクロックサイクル、進めるPCを取得します
        /// この関数はデバッグ用の解析用途に使用することが目的のため、非破壊読み出しがデフォルトで有効になっています
        /// </summary>
        /// <param name="bus"></param>
        /// <param name="cpu"></param>
        /// <returns></returns>
        public Operand ResolveAddressing(IBusAccessible bus, in CpuRegister cpu, bool isNondestructive = true) =>
            new Operand(this.AddressingMode, this.GetAddr(bus, cpu, isNondestructive), this.GetTotalCycles(cpu), this.GetTotalArrangeBytes(cpu));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bus"></param>
        /// <param name="cpu"></param>
        /// <returns>処理にかかったCPU Clock Cycle数</returns>
        public int Run(IBusAccessible bus, CpuRegister cpu) {
            // Memory mode次第で読み出すデータ量が違うのでWrapする
            Func<uint, ushort> readFromBus = (srcAddr) => cpu.Is16bitMemoryAccess ? bus.Read16(srcAddr) : bus.Read8(srcAddr);

            // 命令を実行, PCは個別に進める必要があるので注意
            switch (this.Inst) {
                // Load/Store
                case Instruction.LDA: {
                    // 取得した値をA regに読み込み
                    var srcAddr = this.GetAddr(bus, cpu);
                    var srcData = readFromBus(srcAddr);
                    cpu.AConsideringMemoryReg = srcData;
                    // CPU Flag, PCを更新
                    cpu.UpdateZeroFlag(srcData);
                    cpu.PC += (ushort)this.GetTotalArrangeBytes(cpu);
                    break;
                }
            }
            // 処理にかかったCPU Clock Cycle数を返す
            return this.GetTotalCycles(cpu);
        }

    }
}