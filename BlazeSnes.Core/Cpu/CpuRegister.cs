using System;
using System.IO;

namespace BlazeSnes.Core.Cpu {
    /// <summary>
    /// 65816 のレジスタセット
    /// </summary>
    public class CpuRegister {
        /// <summary>
        /// Accumulator Register
        /// </summary>
        /// <value></value>
        public ushort A { get; set; }
        /// <summary>
        /// Index Register
        /// </summary>
        /// <value></value>
        public ushort X { get; set; }
        /// <summary>
        /// Index Register
        /// </summary>
        /// <value></value>
        public ushort Y { get; set; }
        /// <summary>
        /// Stack Pointer
        /// </summary>
        /// <value></value>
        public ushort SP { get; set; }
        /// <summary>
        /// Data Bank Register
        /// </summary>
        /// <value></value>
        public ushort DB { get; set; }
        /// <summary>
        /// Direct Page Register
        /// </summary>
        /// <value></value>
        public ushort DP { get; set; }
        /// <summary>
        /// Program Bank Register
        /// </summary>
        /// <value></value>
        public ushort PB { get; set; }
        /// <summary>
        /// Processor Status
        /// </summary>
        /// <value></value>
        public ProcessorStatus P { get; set; } = new ProcessorStatus();
        /// <summary>
        /// Program Counter
        /// </summary>
        /// <value></value>
        public ushort PC { get; set; }

        /// <summary>
        /// Memory/Acccumulatorの16bit Accessが有効ならtrueを返します
        /// </summary>
        /// <returns></returns>
        public bool Is16bitMemoryAccess => !this.P.Value.HasFlag(ProcessorStatusFlag.E) && !this.P.Value.HasFlag(ProcessorStatusFlag.M);
        /// <summary>
        /// Indexに16bit Accesが有効ならtrueを返します
        /// </summary>
        /// <returns></returns>
        public bool Is16bitIndexAccess => !this.P.Value.HasFlag(ProcessorStatusFlag.E) && !this.P.Value.HasFlag(ProcessorStatusFlag.X);
        /// <summary>
        /// ダイレクトページの値をSystemAddrに変換します
        /// TODO: 不要かもしれない
        /// </summary>
        /// <returns></returns>
        public uint DirectPageAddr => (uint)this.DP << 8;
        /// <summary>
        /// データバンクの値をSystemAddrに変換します
        /// </summary>
        /// <returns></returns>
        public uint DataBankAddr => (uint)this.DB << 16;
        /// <summary>
        /// ページバンクの値をSystemAddrに変換します
        /// </summary>
        /// <returns></returns>
        public uint PageBankAddr => (uint)this.PB << 16;
        /// <summary>
        /// Memory Registerのビット幅を考慮して入出力します
        /// </summary>
        /// <value></value>
        public ushort AConsideringMemoryReg {
            get => Is16bitMemoryAccess ? (ushort)A : (ushort)(A & 0xff);
            set => A = Is16bitMemoryAccess ? value : (ushort)(value & 0xff);
        }
        /// <summary>
        /// Index Registerのビット幅を考慮して入出力します
        /// </summary>
        /// <value></value>
        public ushort XConsideringIndexReg {
            get => Is16bitIndexAccess ? (ushort)X : (ushort)(X & 0xff);
            set => X = Is16bitIndexAccess ? value : (ushort)(value & 0xff);
        }
        /// <summary>
        /// Index Registerのビット幅を考慮して入出力します
        /// </summary>
        /// <value></value>
        public ushort YConsideringIndexReg {
            get => Is16bitIndexAccess ? (ushort)Y : (ushort)(Y & 0xff);
            set => Y = Is16bitIndexAccess ? value : (ushort)(value & 0xff);
        }
    }
}