using System;
using System.IO;

namespace BlazeSnes.Core.Cpu {
    /// <summary>
    /// アドレッシングモード一覧
    /// </summary>
    public enum Addressing {       
        Implied, // 指定なし
        Accumulator, // (A)の値
        Immediate, // (IM8)の値 IM8
        Direct,// *(IM8)の値 IM8
        DirectIndexedX, // *(IM+X)の値 IM8, X
        DirectIndexedY, // *(IM+Y)の値 IM8, Y
        Relative, // *(PC+IM8)の値 IM8
        Absolute,// *(DB * $1_0000 + IM16)の値 IM16
        AbsoluteIndexedX, // *(DB * $1_0000 + IM16 + X)の値 IM16, X
        AbsoluteIndexedY, // *(DB * $1_0000 + IM16 + Y)の値 IM16, Y
        AbsoluteLong, // *(IM24)の値 IM24
        AbsoluteLongIndexedX, // *(IM24+X)の値 IM24, X
        DirectIndexedIndirectX, // *(DB * $1_0000 + *(IM8+X))の値 IM8, X
        DirectIndirectIndexedY, // *(DB * $1_0000 + *IM8 + Y)の値 (IM8), Y
        DirectIndirectLong, // *(*(IM8))の値 [IM8]
        DirectIndirectLongIndexed, // *(*(IM8) + Y)の値 [IM8]
        AbsoluteIndirect, // *(IM16)の値 (IM16)
        AbsoluteIndexedIndirectX, // *(IM16 + X)の値 (IM16, X)
        AbsoluteIndirectLong, // *(IM24)の値 [IM24]
        // ProgramCounterRelative, // *(PC+IM16)の値
        // ProgramCounterRelativeLong, // *(PC+IM24)の値
        StackRelative, // 0: 最後にpushされた値, 1: 最後にpopされた値 $00,s or $01,s
        StackRelativeIndirectIndexedY, // *(*(s) + Y) の値、StackRelativeから相対間接参照を実装したもの
        BlockMove, // for MVN/MVP
    }
}