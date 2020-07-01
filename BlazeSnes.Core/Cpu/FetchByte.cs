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
}