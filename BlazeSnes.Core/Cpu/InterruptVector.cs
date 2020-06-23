using System;
using System.IO;

namespace BlazeSnes.Core.Cpu {
    /// <summary>
    /// 割り込みベクタを示します
    /// </summary>
    public class InterruptVector {
        public ushort CopAddr(bool isEmulate) => isEmulate ? CopAddrInEmulation : CopAddrInNative;
        public ushort BreakAddr(bool isEmulate) => isEmulate ? BreakAddrInEmulation : BreakAddrInNative;
        public ushort AbortAddr(bool isEmulate) => isEmulate ? AbortAddrInEmulation : AbortAddrInNative;
        public ushort NmiAddr(bool isEmulate) => isEmulate ? NmiAddrInEmulation : NmiAddrInNative;
        public ushort ResetAddr(bool isEmulate) => isEmulate ? ResetAddrInEmulation : ResetAddrInNative;
        public ushort IrqAddr(bool isEmulate) => isEmulate ? IrqAddrInEmulation : IrqAddrInNative;

        public ushort CopAddrInNative { get; set; }
        /// <summary>
        /// BRK割り込み
        /// </summary>
        /// <value></value>
        public ushort BreakAddrInNative { get; set; }
        /// <summary>
        /// ハードウェア割り込み(Abort)
        /// </summary>
        /// <value></value>
        public ushort AbortAddrInNative { get; set; }
        /// <summary>
        /// ハードウェア割り込み(NMI)
        /// </summary>
        /// <value></value>
        public ushort NmiAddrInNative { get; set; }
        /// <summary>
        /// (65812では未使用)
        /// </summary>
        /// <value></value>
        public ushort ResetAddrInNative { get; set; }
        /// <summary>
        /// ハードウェア割り込み
        /// </summary>
        /// <value></value>
        public ushort IrqAddrInNative { get; set; }
        /// <summary>
        /// コプロセッサ割り込み
        /// </summary>
        /// <value></value>
        public ushort CopAddrInEmulation { get; set; }
        /// <summary>
        /// BRK割り込み
        /// </summary>
        /// <value></value>
        public ushort BreakAddrInEmulation { get; set; }
        /// <summary>
        /// ハードウェア割り込み(Abort)
        /// </summary>
        /// <value></value>
        public ushort AbortAddrInEmulation { get; set; }
        /// <summary>
        /// ハードウェア割り込み(NMI)
        /// </summary>
        /// <value></value>
        public ushort NmiAddrInEmulation { get; set; }
        /// <summary>
        /// Reset割り込み
        /// </summary>
        /// <value></value>
        public ushort ResetAddrInEmulation { get; set; }
        /// <summary>
        /// ハードウェア割り込み
        /// </summary>
        /// <value></value>
        public ushort IrqAddrInEmulation { get; set; }
    }
}