namespace BlazeSnes.Core.Cpu {
    /// <summary>
    /// アドレッシング解決した結果を示します
    /// </summary>
    public class Operand {
        /// <summary>
        /// 使用したアドレッシングモード
        /// </summary>
        /// <value></value>
        public Addressing AddressingMode { get; internal set; }
        /// <summary>
        /// 読みだしたデータ、読み出す内容がなければZero固定
        /// </summary>
        /// <value></value>
        public uint Data { get; internal set; } = 0x0;
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

        public Operand(Addressing a, uint d, int c, int bytes) {
            this.AddressingMode = a;
            this.Data = d;
            this.Cycles = c;
            this.ArrangeBytes = bytes;
        }

        public override string ToString() => $"{AddressingMode}: {Data:x}({Cycles}cyc, {ArrangeBytes}byte)";

    }
}