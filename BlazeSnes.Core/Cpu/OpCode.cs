using System;
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
            None, // 0.追加はない
            Add1CycleIf16bitAcccess, // 1.16bit Access時に1cycle追加
            Add1CycleIfDPRegNonZero, // 2.Direct Page Registerが非Zeroなら1cycle追加
            Add1CycleIfPageBoundaryOrXRegZero, // 3.page境界を跨ぐ、x=0で1cycle追加
            Add2CycleIf16bitaccess, // 4.16bit Access時に2cycle追加
            Add1CycleIfBranchIsTaken, // 5.分岐した場合に1cycle追加
            Add1CycleIfBranchIsTakenAndPageCrossesInEmuMode, // 6.EmulationModeでページ跨ぎの分岐が発生した場合に1cycle追加
            Add1CycleIfNativeMode, // 7.NativeModeで1cycle追加
            Add1CycleIfXZero, // 8.X=0で1cycle追加
            Add3CycleToShutdownByReset, // 9.Processor停止に3cycle
            Add3CycleToShutdownByInterrupt, // 10.Processor停止に3cycle
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
        public int GetTotalCycles(in IBusAccessible bus, in CpuRegister cpu) {
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
            if (Option.HasFlag(CycleOption.Add1CycleIfPageBoundaryOrXRegZero) && cpu.X == 0) c += 1;
            if (Option.HasFlag(CycleOption.Add1CycleIfPageBoundaryOrXRegZero | CycleOption.Add1CycleIfXZero) && cpu.X == 0) c += 1;
            if (Option.HasFlag(CycleOption.Add1CycleIfNativeMode) && !cpu.P.Value.HasFlag(ProcessorStatusFlag.E)) c += 1;

            return c;
        }
        /// <summary>
        /// 今回のオペランド取得で進むPCのbyte数を取得します
        /// FetchByte.AddModeはフラグではないので分岐して取得可
        /// /// </summary>
        /// <param name="cpu"></param>
        /// <returns></returns>
        public int GetTotalArrangeBytes(in CpuRegister cpu) => this.FetchBytes.Mode switch
        {
            FetchByte.AddMode.Fixed => FetchBytes.Bytes,
            FetchByte.AddMode.Add1ByteIfMRegZero when cpu.Is16bitMemoryAccess => FetchBytes.Bytes + 1,
            FetchByte.AddMode.Add1ByteIfXRegZero when cpu.Is16bitIndexAccess => FetchBytes.Bytes + 1,
            FetchByte.AddMode.Add1ByteForSignatureByte => FetchBytes.Bytes + 1,
            _ => throw new ArgumentException("存在しないenum値が指定されています"),
        };

        /// <summary>
        /// アドレッシング解決して値を取得します
        /// </summary>
        /// <param name="bus">Mmu</param>
        /// <param name="bus">Cpu, 変更は行わない</param>
        /// <returns>読み出し先ベースアドレス</returns>
        public uint GetAddr(IBusAccessible bus, in CpuRegister cpu) {
            // PCは現在の命令を指した状態で呼ばれるので+1した位置から読む
            var operandBaseAddr = (uint)(cpu.PC + 1);

            // Addressing modeごとに実装
            switch (this.AddressingMode) {
                case Addressing.Immediate:
                    return operandBaseAddr; // OpCodeの次のアドレスそのまま
                case Addressing.Direct:
                    return (uint)(cpu.DP + bus.Read8(operandBaseAddr));
                case Addressing.DirectPageIndexedX:
                    return (uint)(cpu.DP + cpu.X + bus.Read8(operandBaseAddr));
                case Addressing.DirectPageIndexedY:
                    return (uint)(cpu.DP + cpu.Y + bus.Read8(operandBaseAddr));
                case Addressing.Absolute:
                    return (uint)(cpu.DataBankAddr | bus.Read16(operandBaseAddr));
                case Addressing.AbsoluteIndexedX:
                    return (uint)(cpu.DataBankAddr | (uint)(bus.Read16(operandBaseAddr) + cpu.X));
                case Addressing.AbsoluteIndexedY:
                    return (uint)(cpu.DataBankAddr | (uint)(bus.Read16(operandBaseAddr) + cpu.Y));
                case Addressing.AbsoluteLong:
                    return (uint)(bus.Read24(operandBaseAddr));
                case Addressing.AbsoluteLongIndexedX:
                    return (uint)(bus.Read24(operandBaseAddr) + cpu.X);
                case Addressing.DirectPageIndirect:
                    var a = bus.Read16((uint)(cpu.DP + bus.Read8(operandBaseAddr)));
                    return (cpu.DataBankAddr | a);
                case Addressing.Implied:
                case Addressing.Accumulator:
                    throw new ArgumentException("Implied, Accumulatorではアドレス解決できません");
                default:
                    throw new NotImplementedException(); // TODO: 全部やる
            }
        }

        /// <summary>
        /// アドレッシング解決して、読みだしたいデータアドレス、かかったクロックサイクル、すすめるPCを取得します
        /// </summary>
        /// <param name="bus"></param>
        /// <param name="cpu"></param>
        /// <returns></returns>
        public Operand ResolveAddressing(IBusAccessible bus, in CpuRegister cpu) =>
            new Operand(this.AddressingMode, this.GetAddr(bus, cpu), this.GetTotalCycles(bus, cpu), this.GetTotalArrangeBytes(cpu));

    }
}