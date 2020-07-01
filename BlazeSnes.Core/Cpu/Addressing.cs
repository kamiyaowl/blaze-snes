using System;
using System.IO;

using BlazeSnes.Core.Common;

namespace BlazeSnes.Core.Cpu {
    /// <summary>
    /// アドレッシングモード一覧
    /// </summary>
    public enum Addressing {
        Implied, // 指定なし
        Accumulator, // (A)の値
        Immediate, // (IM8)の値 IM8
        Direct,// *(IM8)の値 IM8
        DirectPageIndexedX, // *(IM+X)の値 IM8, X
        DirectPageIndexedY, // *(IM+Y)の値 IM8, Y
        Absolute,// *(DB * $1_0000 + IM16)の値 IM16
        AbsoluteIndexedX, // *(DB * $1_0000 + IM16 + X)の値 IM16, X
        AbsoluteIndexedY, // *(DB * $1_0000 + IM16 + Y)の値 IM16, Y
        AbsoluteLong, // *(IM24)の値 IM24
        AbsoluteLongIndexedX, // *(IM24+X)の値 IM24, X
        DirectPageIndirect, // *(*(IM8))の値 (IM8)
        DirectPageIndirectLong, // *(*(IM16))の値 [IM16]
        DirectPageIndirectLongIndexedY, // *(*(IM8) + Y)の値 [IM8]
        DirectPageIndexedIndirectX, // *(DB * $1_0000 + *(IM8+X))の値 IM8, X
        DirectPageIndirectIndexedY, // *(DB * $1_0000 + *IM8 + Y)の値 (IM8), Y
        AbsoluteIndirect, // *(IM16)の値 (IM16)
        AbsoluteIndexedIndirectX, // *(IM16 + X)の値 (IM16, X)
        AbsoluteIndirectLong, // *(IM24)の値 [IM24]
        ProgramCounterRelative, // *(PC+IM16)の値
        ProgramCounterRelativeLong, // *(PC+IM24)の値
        StackRelative, // 0: 最後にpushされた値, 1: 最後にpopされた値 $00,s or $01,s
        StackRelativeIndirectIndexedY, // *(*(s) + Y) の値、StackRelativeから相対間接参照を実装したもの
        BlockMove, // for MVN/MVP
    }
}