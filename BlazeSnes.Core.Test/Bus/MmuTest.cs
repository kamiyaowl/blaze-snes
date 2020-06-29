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
            Wram, Ppu, Apu, Onchip, Dma, Expansion, Cartridge, OpenBus,
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
            public Mock<IBusAccessible> Expansion { get; set; }
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
                    yield return Expansion;
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
                    Peripheral.Expansion => this.Expansion,
                    Peripheral.Cartridge => this.Cartridge,
                    Peripheral.OpenBus => null,
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
                        m.Setup(x => x.Write(addr, It.Is<byte[]>(x => x.Length > 0))).Returns(true).Verifiable();
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
                    Expansion = new Mock<IBusAccessible>(),
                    Cartridge = new Mock<IBusAccessible>(),
                };
                var target = new Mmu(mocks.Wram.Object, mocks.Ppu.Object, mocks.Apu.Object, mocks.OnChip.Object, mocks.Dma.Object, mocks.Expansion.Object, mocks.Cartridge.Object);
                return (target, mocks);
            }
        }

        /// <summary>
        /// 期待値設定一覧
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<object[]> GetMappingPattern() {
            yield return new object[] { Peripheral.Wram, Operation.Read, 0x00_0000 };
        }

        /// <summary>
        /// Readした領域ごとのインスタンスに受け渡されるか確認
        /// </summary>
        [Theory, MemberData(nameof(GetMappingPattern))]
        public void ReadWriteMapping(Peripheral p, Operation o, uint addr) {
            var (mmu, mocks) = MmuTargetMocks.Create();
            mocks.Expect(p, o, addr);
            mmu.Read8(addr);

            Mock.VerifyAll(mocks.All.ToArray());
        }
    }
}