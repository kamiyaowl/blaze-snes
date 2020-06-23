using System;
using System.IO;

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
    }
}