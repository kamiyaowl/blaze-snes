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
        public ProcessorStatus P { get; set; }
        /// <summary>
        /// Program Counter
        /// </summary>
        /// <value></value>
        public ushort PC { get; set; }
    }
}