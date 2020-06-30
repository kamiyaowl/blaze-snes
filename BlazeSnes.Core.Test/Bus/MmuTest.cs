using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using BlazeSnes.Core.Bus;
using BlazeSnes.Core.Common;

using Moq;

using Xunit;
using Xunit.Sdk;

namespace BlazeSnes.Core.Test.Bus {
    public class MmuTest {
        public enum Peripheral {
            Wram, Ppu, Apu, Onchip, Dma, Cartridge, Unused,
        }
        public enum Operation {
            Read, ReadNondestructive, Write
        }

        /// <summary>
        /// 引数に使うMock群
        /// </summary>
        public class MmuTargetMocks {
            public Mock<IBusAccessible> Wram { get; set; }
            public Mock<IBusAccessible> Ppu { get; set; }
            public Mock<IBusAccessible> Apu { get; set; }
            public Mock<IBusAccessible> OnChip { get; set; }
            public Mock<IBusAccessible> Dma { get; set; }
            public Mock<IBusAccessible> Cartridge { get; set; }

            /// <summary>
            /// すべてのMockを取得します
            /// </summary>
            /// <value></value>
            public IEnumerable<Mock<IBusAccessible>> All {
                get {
                    yield return Wram;
                    yield return Ppu;
                    yield return Apu;
                    yield return OnChip;
                    yield return Dma;
                    yield return Cartridge;
                }
            }

            /// <summary>
            /// 指定された期待値読み出しをモックに設定します
            /// </summary>
            /// <param name="p"></param>
            /// <param name="o"></param>
            /// <param name="addr"></param>
            public void Expect(Peripheral p, Operation o, uint addr) {
                var m = p switch
                {
                    Peripheral.Wram => this.Wram,
                    Peripheral.Ppu => this.Ppu,
                    Peripheral.Apu => this.Apu,
                    Peripheral.Onchip => this.OnChip,
                    Peripheral.Dma => this.Dma,
                    Peripheral.Cartridge => this.Cartridge,
                    Peripheral.Unused => null,
                    _ => throw new ArgumentException(),
                };
                // openbus では期待する読み出しは存在しない
                if (m == null) {
                    return;
                }

                switch (o) {
                    case Operation.Read:
                        m.Setup(x => x.Read(addr, It.Is<byte[]>(x => x.Length > 0), false)).Returns(true).Verifiable();
                        break;
                    case Operation.ReadNondestructive:
                        m.Setup(x => x.Read(addr, It.Is<byte[]>(x => x.Length > 0), true)).Returns(true).Verifiable();
                        break;
                    case Operation.Write:
                        m.Setup(x => x.Write(addr, It.Ref<byte[]>.IsAny)).Returns(true).Verifiable();
                        break;
                    default:
                        throw new ArgumentException();
                }
            }

            /// <summary>
            /// MockとMMUを作成して返します
            /// </summary>
            /// <returns></returns>
            public static (Mmu, MmuTargetMocks) Create() {
                var mocks = new MmuTargetMocks() {
                    Wram = new Mock<IBusAccessible>(),
                    Ppu = new Mock<IBusAccessible>(),
                    Apu = new Mock<IBusAccessible>(),
                    OnChip = new Mock<IBusAccessible>(),
                    Dma = new Mock<IBusAccessible>(),
                    Cartridge = new Mock<IBusAccessible>(),
                };
                var target = new Mmu(mocks.Wram.Object, mocks.Ppu.Object, mocks.Apu.Object, mocks.OnChip.Object, mocks.Dma.Object, mocks.Cartridge.Object);
                return (target, mocks);
            }
        }

        /// <summary>
        /// 期待値設定一覧
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<object[]> GetMappingPattern() {
            yield return new object[] { Peripheral.Wram, Operation.Read, 0x00_0000 };
            yield return new object[] { Peripheral.Wram, Operation.Write, 0x00_1fff };
            yield return new object[] { Peripheral.Unused, Operation.Read, 0x00_2000 };
            yield return new object[] { Peripheral.Unused, Operation.Write, 0x00_20ff };
            yield return new object[] { Peripheral.Ppu, Operation.Read, 0x00_2100 };
            yield return new object[] { Peripheral.Ppu, Operation.Write, 0x00_213f };
            yield return new object[] { Peripheral.Apu, Operation.Read, 0x00_2140 };
            yield return new object[] { Peripheral.Apu, Operation.Write, 0x00_217f };
            yield return new object[] { Peripheral.Wram, Operation.Read, 0x00_2180 };
            yield return new object[] { Peripheral.Wram, Operation.Write, 0x00_2183 };
            yield return new object[] { Peripheral.Cartridge, Operation.Read, 0x00_2184 };
            yield return new object[] { Peripheral.Cartridge, Operation.Write, 0x00_3fff };
            yield return new object[] { Peripheral.Onchip, Operation.Read, 0x00_4000 };
            yield return new object[] { Peripheral.Onchip, Operation.Write, 0x00_42ff };
            yield return new object[] { Peripheral.Dma, Operation.Read, 0x00_4300 };
            yield return new object[] { Peripheral.Dma, Operation.Write, 0x00_5fff };
            yield return new object[] { Peripheral.Cartridge, Operation.Read, 0x00_6000 };
            yield return new object[] { Peripheral.Cartridge, Operation.Write, 0x00_ffff };

            yield return new object[] { Peripheral.Wram, Operation.Read, 0x3f_0000 };
            yield return new object[] { Peripheral.Wram, Operation.Write, 0x3f_1fff };
            yield return new object[] { Peripheral.Unused, Operation.Read, 0x3f_2000 };
            yield return new object[] { Peripheral.Unused, Operation.Write, 0x3f_20ff };
            yield return new object[] { Peripheral.Ppu, Operation.Read, 0x3f_2100 };
            yield return new object[] { Peripheral.Ppu, Operation.Write, 0x3f_213f };
            yield return new object[] { Peripheral.Apu, Operation.Read, 0x3f_2140 };
            yield return new object[] { Peripheral.Apu, Operation.Write, 0x3f_217f };
            yield return new object[] { Peripheral.Wram, Operation.Read, 0x3f_2180 };
            yield return new object[] { Peripheral.Wram, Operation.Write, 0x3f_2183 };
            yield return new object[] { Peripheral.Cartridge, Operation.Read, 0x3f_2184 };
            yield return new object[] { Peripheral.Cartridge, Operation.Write, 0x3f_3fff };
            yield return new object[] { Peripheral.Onchip, Operation.Read, 0x3f_4000 };
            yield return new object[] { Peripheral.Onchip, Operation.Write, 0x3f_42ff };
            yield return new object[] { Peripheral.Dma, Operation.Read, 0x3f_4300 };
            yield return new object[] { Peripheral.Dma, Operation.Write, 0x3f_5fff };
            yield return new object[] { Peripheral.Cartridge, Operation.Read, 0x3f_6000 };
            yield return new object[] { Peripheral.Cartridge, Operation.Write, 0x3f_ffff };

            yield return new object[] { Peripheral.Cartridge, Operation.Read, 0x40_0000 };
            yield return new object[] { Peripheral.Cartridge, Operation.Write, 0x40_ffff };
            yield return new object[] { Peripheral.Cartridge, Operation.Read, 0x7d_0000 };
            yield return new object[] { Peripheral.Cartridge, Operation.Write, 0x7d_ffff };
            yield return new object[] { Peripheral.Wram, Operation.Read, 0x7e_0000 };
            yield return new object[] { Peripheral.Wram, Operation.Write, 0x7e_ffff };
            yield return new object[] { Peripheral.Wram, Operation.Read, 0x7f_0000 };
            yield return new object[] { Peripheral.Wram, Operation.Write, 0x7f_ffff };

            yield return new object[] { Peripheral.Wram, Operation.Read, 0x80_0000 };
            yield return new object[] { Peripheral.Wram, Operation.Write, 0x80_1fff };
            yield return new object[] { Peripheral.Unused, Operation.Read, 0x80_2000 };
            yield return new object[] { Peripheral.Unused, Operation.Write, 0x80_20ff };
            yield return new object[] { Peripheral.Ppu, Operation.Read, 0x80_2100 };
            yield return new object[] { Peripheral.Ppu, Operation.Write, 0x80_213f };
            yield return new object[] { Peripheral.Apu, Operation.Read, 0x80_2140 };
            yield return new object[] { Peripheral.Apu, Operation.Write, 0x80_217f };
            yield return new object[] { Peripheral.Wram, Operation.Read, 0x80_2180 };
            yield return new object[] { Peripheral.Wram, Operation.Write, 0x80_2183 };
            yield return new object[] { Peripheral.Cartridge, Operation.Read, 0x80_2184 };
            yield return new object[] { Peripheral.Cartridge, Operation.Write, 0x80_3fff };
            yield return new object[] { Peripheral.Onchip, Operation.Read, 0x80_4000 };
            yield return new object[] { Peripheral.Onchip, Operation.Write, 0x80_42ff };
            yield return new object[] { Peripheral.Dma, Operation.Read, 0x80_4300 };
            yield return new object[] { Peripheral.Dma, Operation.Write, 0x80_5fff };
            yield return new object[] { Peripheral.Cartridge, Operation.Read, 0x80_6000 };
            yield return new object[] { Peripheral.Cartridge, Operation.Write, 0x80_ffff };

            yield return new object[] { Peripheral.Wram, Operation.Read, 0xbf_0000 };
            yield return new object[] { Peripheral.Wram, Operation.Write, 0xbf_1fff };
            yield return new object[] { Peripheral.Unused, Operation.Read, 0xbf_2000 };
            yield return new object[] { Peripheral.Unused, Operation.Write, 0xbf_20ff };
            yield return new object[] { Peripheral.Ppu, Operation.Read, 0xbf_2100 };
            yield return new object[] { Peripheral.Ppu, Operation.Write, 0xbf_213f };
            yield return new object[] { Peripheral.Apu, Operation.Read, 0xbf_2140 };
            yield return new object[] { Peripheral.Apu, Operation.Write, 0xbf_217f };
            yield return new object[] { Peripheral.Wram, Operation.Read, 0xbf_2180 };
            yield return new object[] { Peripheral.Wram, Operation.Write, 0xbf_2183 };
            yield return new object[] { Peripheral.Cartridge, Operation.Read, 0xbf_2184 };
            yield return new object[] { Peripheral.Cartridge, Operation.Write, 0xbf_3fff };
            yield return new object[] { Peripheral.Onchip, Operation.Read, 0xbf_4000 };
            yield return new object[] { Peripheral.Onchip, Operation.Write, 0xbf_42ff };
            yield return new object[] { Peripheral.Dma, Operation.Read, 0xbf_4300 };
            yield return new object[] { Peripheral.Dma, Operation.Write, 0xbf_5fff };
            yield return new object[] { Peripheral.Cartridge, Operation.Read, 0xbf_6000 };
            yield return new object[] { Peripheral.Cartridge, Operation.Write, 0xbf_ffff };

            yield return new object[] { Peripheral.Cartridge, Operation.Read, 0xc0_0000 };
            yield return new object[] { Peripheral.Cartridge, Operation.Read, 0xc0_ffff };
            yield return new object[] { Peripheral.Cartridge, Operation.Read, 0xff_0000 };
            yield return new object[] { Peripheral.Cartridge, Operation.Read, 0xff_ffff };
        }

        /// <summary>
        /// 期待した領域にRead/Write要求が来るか確認
        /// </summary>
        /// <param name="p">対象のペリフェラル</param>
        /// <param name="o">Read/Write</param>
        /// <param name="addr">SystemAddress</param>
        [Theory, MemberData(nameof(GetMappingPattern))]
        public void ReadWriteMapping(Peripheral p, Operation o, uint addr) {
            var (mmu, mocks) = MmuTargetMocks.Create();

            // 呼び出し期待値を設定
            mocks.Expect(p, o, addr);
            // operationに従ってRead/Writeを行う
            switch (o) {
                case Operation.Read:
                    mmu.Read8(addr);
                    break;
                case Operation.ReadNondestructive:
                    mmu.Read8(addr, true);
                    break;
                case Operation.Write:
                    mmu.Write8(addr, 0xa5); // dummy data
                    break;
                default:
                    throw new ArgumentException();
            }
            // 期待通り呼ばれているか確認
            Mock.VerifyAll(mocks.All.ToArray());
        }
    }
}