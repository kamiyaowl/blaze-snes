using System;
using System.IO;

namespace BlazeSnes.Core.Cpu {
    /// <summary>
    /// CPU割り込みの種類を示します
    /// </summary>
    public enum Interrupt {
        CoProcessor,
        Break,
        Abort,
        NonMaskable,
        Reset,
        OtherIrq,
    }
}