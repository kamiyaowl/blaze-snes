using System;
using System.IO;

using BlazeSnes.Core.Common;

namespace BlazeSnes.Core.Cpu {
    /// <summary>
    /// FetchするときのByte数を示します
    /// 代数データ構造がないので泣く泣く作成
    /// </summary>
    public class FetchByte {
        /// <summary>
        /// FetchByte数が可変する場合の情報を保持している
        /// Flagではないのでいずれかを選択
        /// </summary>
        public enum AddMode {
            Fixed, // 固定値、デフォルト
            Add1ByteIfMRegZero, // 12.M reg=0の場合、16bit accessなので1byte増える
            Add1ByteForSignatureByte, // 13.命令は1byteだが、2bytePCが進む。BRK, COP(Co Processor Enable)でのシグネチャ用
            Add1ByteIfXRegZero, // 14.X reg=0の場合、16bit accessなので1byte増える
        }
        /// <summary>
        /// Fetch Byte数が可変する条件
        /// </summary>
        /// <value></value>
        public AddMode Mode { get; internal set; }
        /// <summary>
        /// FetchするByte数。Mode分は考慮しない値を入れる
        /// </summary>
        /// <value></value>
        public int Bytes { get; internal set; }

        /// <summary>
        /// 固定Byte数のコンストラクタ
        /// </summary>
        /// <param name="bytes"></param>
        public FetchByte(int bytes) {
            this.Bytes = bytes;
            this.Mode = AddMode.Fixed;
        }

        /// <summary>
        /// Byte数可変のコンストラクタ
        /// </summary>
        /// <param name="bytes"></param>
        public FetchByte(int bytes, AddMode mode) {
            this.Bytes = bytes;
            this.Mode = mode;
        }

        public override string ToString() => (Mode == AddMode.Fixed) ? $"{Bytes}byte" : $"{Bytes}byte(+{Mode})";
    }
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
        /// アドレッシング解決して値を取得します
        /// </summary>
        /// <param name="bus">Mmu</param>
        /// <param name="bus">Cpu, 変更は行わない</param>
        /// <returns>(取得したデータ, クロックサイクル)</returns>
        public (uint, int) GetOperand(IBusAccessible bus, in CpuRegister cpu) {
            // PCは現在の命令を指した状態で呼ばれるので+1した位置から読む
            var operandBaseAddr = (uint)(cpu.PC + 1);
            var is16bitAccess = cpu.Is16bitAccess;

            // 事前計算可能なcycle数
            int c = this.Cycles;
            if (!Option.HasFlag(CycleOption.None)) {
                if (is16bitAccess) {
                    if (Option.HasFlag(CycleOption.Add1CycleIf16bitAcccess)) c += 1;
                    if (Option.HasFlag(CycleOption.Add2CycleIf16bitaccess)) c += 2;
                }
                if (Option.HasFlag(CycleOption.Add1CycleIfDPRegNonZero) && cpu.DP != 0) c += 1;
                if (Option.HasFlag(CycleOption.Add1CycleIfPageBoundaryOrXRegZero) && cpu.X == 0) c += 1;
                if (Option.HasFlag(CycleOption.Add1CycleIfPageBoundaryOrXRegZero | CycleOption.Add1CycleIfXZero) && cpu.X == 0) c += 1;
                if (Option.HasFlag(CycleOption.Add1CycleIfNativeMode) && !cpu.P.Value.HasFlag(ProcessorStatusFlag.E)) c += 1;
                // TODO: 未実装: Add1CycleIfPageBoundaryOrXRegZero, Add1CycleIfBranchIsTaken, Add1CycleIfBranchIsTakenAndPageCrossesInEmuMode
            }

            // 同じような実装を散りばめるのは気分が悪いので
            Func<uint, (uint, int)> readDstData = (x) => ((is16bitAccess ? (uint)bus.Read16(x) : (uint)bus.Read8(x)), c);

            // Addressing modeごとに実装
            switch (this.AddressingMode) {
                case Addressing.Implied:
                    return (0x0, c);
                case Addressing.Accumulator:
                    return ((is16bitAccess ? (cpu.A) : (byte)(cpu.A & 0xff)), c);
                case Addressing.Immediate:
                    return readDstData(operandBaseAddr);
                case Addressing.Direct:
                    return readDstData((uint)(cpu.DP + bus.Read8(operandBaseAddr)));
                case Addressing.DirectIndexedX:
                    return readDstData((uint)(cpu.DP + cpu.X + bus.Read8(operandBaseAddr)));
                case Addressing.DirectIndexedY:
                    return readDstData((uint)(cpu.DP + cpu.Y + bus.Read8(operandBaseAddr)));
                case Addressing.Absolute:
                    return readDstData((uint)(cpu.DataBankAddr | bus.Read16(operandBaseAddr)));
                case Addressing.AbsoluteIndexedX:
                    return readDstData((uint)(cpu.DataBankAddr | (uint)(bus.Read16(operandBaseAddr) + cpu.X)));
                case Addressing.AbsoluteIndexedY:
                    return readDstData((uint)(cpu.DataBankAddr | (uint)(bus.Read16(operandBaseAddr) + cpu.Y)));
                case Addressing.AbsoluteLong:
                    return readDstData((uint)(bus.Read24(operandBaseAddr)));
                case Addressing.AbsoluteLongIndexedX:
                    return readDstData((uint)(bus.Read24(operandBaseAddr) + cpu.X));
                default:
                    throw new NotImplementedException(); // TODO: 全部やる

            }
        }
    }
}