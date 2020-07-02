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
            yield return new object[] { CycleOption.None, new CpuRegister() { P = new ProcessorStatus() }, 1, };
            // 16bit memory/accumulator access +1cyc
            yield return new object[] { CycleOption.Add1CycleIf16bitAcccess, new CpuRegister() { P = new ProcessorStatus(ProcessorStatusFlag.E | ProcessorStatusFlag.M) } , 1, };
            yield return new object[] { CycleOption.Add1CycleIf16bitAcccess, new CpuRegister() { P = new ProcessorStatus(ProcessorStatusFlag.E) } , 1, }; // emulationが有効だと1byte
            yield return new object[] { CycleOption.Add1CycleIf16bitAcccess, new CpuRegister() { P = new ProcessorStatus(ProcessorStatusFlag.M) }, 1, }; // nativeだけど1byte
            yield return new object[] { CycleOption.Add1CycleIf16bitAcccess, new CpuRegister() { P = new ProcessorStatus() }, 2, };
            // 16bit memory/accumulator access +2cyc
            yield return new object[] { CycleOption.Add2CycleIf16bitaccess, new CpuRegister() { P = new ProcessorStatus(ProcessorStatusFlag.E | ProcessorStatusFlag.M) }, 1, };
            yield return new object[] { CycleOption.Add2CycleIf16bitaccess, new CpuRegister() { P = new ProcessorStatus(ProcessorStatusFlag.E) } , 1, }; // emulationが有効だと1byte
            yield return new object[] { CycleOption.Add2CycleIf16bitaccess, new CpuRegister() { P = new ProcessorStatus(ProcessorStatusFlag.M) }, 1, }; // nativeだけど1byte
            yield return new object[] { CycleOption.Add2CycleIf16bitaccess, new CpuRegister() { P = new ProcessorStatus() }, 3, };
            // direct page/zero page
            yield return new object[] { CycleOption.Add1CycleIfDPRegNonZero, new CpuRegister() { DP = 0x0 }, 1, };
            yield return new object[] { CycleOption.Add1CycleIfDPRegNonZero, new CpuRegister() { DP = 0x1 }, 2, };
            yield return new object[] { CycleOption.Add1CycleIfPageBoundaryOrXRegZero, new CpuRegister() { X = 0x0, P = new ProcessorStatus(ProcessorStatusFlag.E) }, 2, };
            yield return new object[] { CycleOption.Add1CycleIfPageBoundaryOrXRegZero, new CpuRegister() { X = 0x1, P = new ProcessorStatus(ProcessorStatusFlag.E) }, 1, };
            yield return new object[] { CycleOption.Add1CycleIfXZero, new CpuRegister() { X = 0x0, P = new ProcessorStatus(ProcessorStatusFlag.E) }, 2, };
            yield return new object[] { CycleOption.Add1CycleIfXZero, new CpuRegister() { X = 0x1, P = new ProcessorStatus(ProcessorStatusFlag.E) }, 1, };
            // native/emulation
            yield return new object[] { CycleOption.Add1CycleIfNativeMode, new CpuRegister() { P = new ProcessorStatus(ProcessorStatusFlag.E | ProcessorStatusFlag.M) }, 1, };
            yield return new object[] { CycleOption.Add1CycleIfNativeMode, new CpuRegister() { P = new ProcessorStatus(ProcessorStatusFlag.M) }, 2, };
        }

        /// <summary>
        /// 処理にかかるClock Cycle数が期待値通りか検証します
        /// </summary>
        /// <param name="options"></param>
        /// <param name="cpu"></param>
        /// <param name="expectCycles"></param>
        [Theory, MemberData(nameof(TotalCycleParams))]
        public void TotalCycles(CycleOption options, CpuRegister cpu, int expectCycles) {
            // default 1cycのテスト用のOpCodeを設定
            var opcode = new OpCode(0x00, Instruction.ADC, Addressing.Implied, new FetchByte(1), 1, options);

            var cylces = opcode.GetTotalCycles(cpu);
            Assert.Equal(expectCycles, cylces);
        }

        // TODO: #39 残りのテストも書く

    }
}