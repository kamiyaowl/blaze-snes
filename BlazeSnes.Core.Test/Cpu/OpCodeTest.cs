using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using BlazeSnes.Core.Common;
using BlazeSnes.Core.Cpu;

using Moq;

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
            yield return new object[] { CycleOption.Add1CycleIf16bitAcccess, new CpuRegister() { P = new ProcessorStatus(ProcessorStatusFlag.E | ProcessorStatusFlag.M) }, 1, };
            yield return new object[] { CycleOption.Add1CycleIf16bitAcccess, new CpuRegister() { P = new ProcessorStatus(ProcessorStatusFlag.E) }, 1, }; // emulationが有効だと1byte
            yield return new object[] { CycleOption.Add1CycleIf16bitAcccess, new CpuRegister() { P = new ProcessorStatus(ProcessorStatusFlag.M) }, 1, }; // nativeだけど1byte
            yield return new object[] { CycleOption.Add1CycleIf16bitAcccess, new CpuRegister() { P = new ProcessorStatus() }, 2, };
            // 16bit memory/accumulator access +2cyc
            yield return new object[] { CycleOption.Add2CycleIf16bitaccess, new CpuRegister() { P = new ProcessorStatus(ProcessorStatusFlag.E | ProcessorStatusFlag.M) }, 1, };
            yield return new object[] { CycleOption.Add2CycleIf16bitaccess, new CpuRegister() { P = new ProcessorStatus(ProcessorStatusFlag.E) }, 1, }; // emulationが有効だと1byte
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

        /// <summary>
        /// TotalFetchBytesのテストパラメータ
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<object[]> GetTotalArrangeBytesParams() {
            // default
            yield return new object[] { new FetchByte(1), new CpuRegister(), 1 };
            yield return new object[] { new FetchByte(2), new CpuRegister(), 2 };
            // Memory Mode
            yield return new object[] { new FetchByte(1, FetchByte.AddMode.Add1ByteIfMRegZero), new CpuRegister() { P = new ProcessorStatus(ProcessorStatusFlag.E | ProcessorStatusFlag.M) }, 1 };
            yield return new object[] { new FetchByte(1, FetchByte.AddMode.Add1ByteIfMRegZero), new CpuRegister() { P = new ProcessorStatus(ProcessorStatusFlag.M) }, 1 };
            yield return new object[] { new FetchByte(1, FetchByte.AddMode.Add1ByteIfMRegZero), new CpuRegister() { P = new ProcessorStatus(ProcessorStatusFlag.E) }, 1 };
            yield return new object[] { new FetchByte(1, FetchByte.AddMode.Add1ByteIfMRegZero), new CpuRegister() { P = new ProcessorStatus() }, 2 };
            // Index Register Mode
            yield return new object[] { new FetchByte(1, FetchByte.AddMode.Add1ByteIfXRegZero), new CpuRegister() { P = new ProcessorStatus(ProcessorStatusFlag.E | ProcessorStatusFlag.X) }, 1 };
            yield return new object[] { new FetchByte(1, FetchByte.AddMode.Add1ByteIfXRegZero), new CpuRegister() { P = new ProcessorStatus(ProcessorStatusFlag.X) }, 1 };
            yield return new object[] { new FetchByte(1, FetchByte.AddMode.Add1ByteIfXRegZero), new CpuRegister() { P = new ProcessorStatus(ProcessorStatusFlag.E) }, 1 };
            yield return new object[] { new FetchByte(1, FetchByte.AddMode.Add1ByteIfXRegZero), new CpuRegister() { P = new ProcessorStatus() }, 2 };
            // signature
            yield return new object[] { new FetchByte(1, FetchByte.AddMode.Add1ByteForSignatureByte), new CpuRegister(), 2 };
        }

        /// <summary>
        /// 処理で進むPCのbyte数を返します
        /// </summary>
        /// <param name="fetchByte"></param>
        /// <param name="cpu"></param>
        /// <param name="expectFetchByte"></param>
        [Theory, MemberData(nameof(GetTotalArrangeBytesParams))]
        public void TotalFetchBytes(FetchByte fetchByte, CpuRegister cpu, int expectFetchByte) {
            // 適当なOpcodeを生成
            var opcode = new OpCode(0x00, Instruction.ADC, Addressing.Absolute, fetchByte, 1, CycleOption.None);

            var actual = opcode.GetTotalArrangeBytes(cpu);
            Assert.Equal(expectFetchByte, actual);
        }

        /// <summary>
        /// あドレッシンモード検証用のパラメータ
        /// </summary>
        public class AddressingTestParam {
            /// <summary>
            /// 対象のモード
            /// </summary>
            /// <value></value>
            public Addressing Mode { get; set; }
            /// <summary>
            /// CPUの状態
            /// </summary>
            /// <value></value>
            public CpuRegister Cpu { get; set; }
            /// <summary>
            /// 期待値
            /// </summary>
            /// <value></value>
            public uint ExpectAddr { get; set; }
            /// <summary>
            /// 読み出し期待アドレスと、読み出せる結果のペア
            /// </summary>
            public IEnumerable<(uint, byte[])> ReadDatas { get; set; }

            public AddressingTestParam() { }
            public AddressingTestParam(Addressing mode, CpuRegister cpu, uint expect) {
                this.Mode = mode;
                this.Cpu = cpu;
                this.ExpectAddr = expect;
            }
            public AddressingTestParam(Addressing mode, CpuRegister cpu, uint expect, IEnumerable<(uint, byte[])> readDatas) {
                this.Mode = mode;
                this.Cpu = cpu;
                this.ExpectAddr = expect;
                this.ReadDatas = readDatas;
            }

            /// <summary>
            /// 引数に渡されたMockに期待アドレスの読み出しを追加します
            /// </summary>
            /// <param name="bus"></param>
            public void Setup(Mock<IBusAccessible> bus) {
                foreach (var (addr, datas) in ReadDatas ?? Enumerable.Empty<(uint, byte[])>()) {
                    var setup = bus.Setup(x => x.Read(addr, It.Is<byte[]>(x => x.Length == datas.Length), false));
                    setup.Callback((uint argAddr, byte[] argData, bool _) => {
                        // 期待値をコピー
                        Buffer.BlockCopy(datas, 0, argData, 0, argData.Length);
                    });
                    setup.Returns(true);
                    setup.Verifiable();
                }
            }
        }

        /// <summary>
        /// アドレッシングモード検証用のテストパラメータ
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<object[]> GetAddrParams() {
            // Immediate
            yield return new object[] { new AddressingTestParam(
                Addressing.Immediate,
                new CpuRegister(),
                1
            )};
            yield return new object[] { new AddressingTestParam(
                Addressing.Immediate,
                new CpuRegister() { PC = 0xa5 },
                0xa6
            )};
            // Direct Page
            yield return new object[] { new AddressingTestParam(
                Addressing.DirectPage,
                new CpuRegister(),
                0xaa,
                new (uint, byte[])[] {
                    (1, new byte[] { 0xaa, }),
                }
            )};
            // TODO: #39 色々増やす
        }

        /// <summary>
        /// アドレッシングモードごとのテスト
        /// </summary>
        /// <param name="param"></param>
        [Theory, MemberData(nameof(GetAddrParams))]
        public void GetAddr(AddressingTestParam param) {
            // アドレッシング実行時のSystem BusをMockで用意してあげる
            var bus = new Mock<IBusAccessible>();
            param.Setup(bus);
            // Addressing Mode以外適当なOpCodeを生成
            var opcode = new OpCode(0x00, Instruction.ADC, param.Mode, new FetchByte(1), 1, CycleOption.None);

            // 実行してきた位置チェック
            var actualAddr = opcode.GetAddr(bus.Object, param.Cpu);
            Assert.Equal(param.ExpectAddr, actualAddr);
        }
    }
}