using System;
using System.IO;

namespace BlazeSnes.Core.Cpu {
    /// <summary>
    /// アドレッシングモード一覧
    /// </summary>
    public enum Addressing {       
        Implied,
        ImmediateMemoryFlag,
        ImmediateIndexFlag,
        Immediate,
        Relative,
        RelativeLong,
        Direct,
        DirectIndexedX,
        DirectIndexedY,
        DirectIndexedIndirectX,
        DirectIndirectIndexedLong,
        Absolute,
        AbsoluteX,
        AbsoluteY,
        AbsoluteLong,
        AbsoluteIndexedLong,
        StackRelative,
        StackRelativeIndirectIndexed,
        AbsoluteIndirect,
        AbsoluteIndirectLong,
        AbsoluteIndexedIndirect,
        ImpliedAccumulator,
        BlockMove,
    }
}