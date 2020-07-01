using System;
using System.Collections.Generic;
using System.IO;

using BlazeSnes.Core.Common;
using BlazeSnes.Core.Cpu;

using Xunit;
using Xunit.Sdk;

using static BlazeSnes.Core.Cpu.OpCode;

namespace BlazeSnes.Core.Test.Cpu {
    public class OpCodeTest {
        /// <summary>
        ///  TotalCyclesの期待値
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<object[]> TotalCycleParams() {
            // default
            yield return new object[] { CycleOption.None, 0x0, 1, };
            // 16bit memory/accumulator access +1cyc
            yield return new object[] { CycleOption.Add1CycleIf16bitAcccess, ProcessorStatusFlag.E | ProcessorStatusFlag.M, 1, };
            yield return new object[] { CycleOption.Add1CycleIf16bitAcccess, ProcessorStatusFlag.E , 1, }; // emulationが有効だと1byte
            yield return new object[] { CycleOption.Add1CycleIf16bitAcccess, ProcessorStatusFlag.M, 1, }; // nativeだけど1byte
            yield return new object[] { CycleOption.Add1CycleIf16bitAcccess, 0x0, 2, };
            // 16bit memory/accumulator access +2cyc
            yield return new object[] { CycleOption.Add2CycleIf16bitaccess, ProcessorStatusFlag.E | ProcessorStatusFlag.M, 1, };
            yield return new object[] { CycleOption.Add2CycleIf16bitaccess, ProcessorStatusFlag.E , 1, }; // emulationが有効だと1byte
            yield return new object[] { CycleOption.Add2CycleIf16bitaccess, ProcessorStatusFlag.M, 1, }; // nativeだけど1byte
            yield return new object[] { CycleOption.Add2CycleIf16bitaccess, 0x0, 3, };
            // TODO: 残りのパターン
        }

        /// <summary>
        /// 処理にかかるClock Cycle数が期待値通りか検証します
        /// </summary>
        /// <param name="options"></param>
        /// <param name="flags"></param>
        /// <param name="expectCycles"></param>
        [Theory, MemberData(nameof(TotalCycleParams))]
        public void TotalCycles(CycleOption options, ProcessorStatusFlag flags, int expectCycles) {
            // CPU Statusを模倣して作る
            var cpu = new CpuRegister();
            cpu.P.Value = flags;
            // default 1cycのテスト用のOpCodeを設定
            var opcode = new OpCode(0x00, Instruction.ADC, Addressing.Implied, new FetchByte(1), 1, options);

            var cylces = opcode.GetTotalCycles(cpu);
            Assert.Equal(expectCycles, cylces);
        }

        // TODO: #39 残りのテストも書く

    }
}