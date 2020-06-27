using System;
using System.IO;
using System.Linq;

using BlazeSnes.Core.Common;
using BlazeSnes.Core.Cpu;

using Xunit;
using Xunit.Sdk;

namespace BlazeSnes.Core.Test.Common {
    public class IBusAccessibleTest {
        /// <summary>
        /// テスト用のシンプルな実装
        /// </summary>
        class BusAccessibleSample : IBusAccessible {
            /// <summary>
            /// データ格納先, Test Fixtureから見えるように公開してある
            /// </summary>
            public byte[] InternalBuf { get; set; }

            public BusAccessibleSample(int size) {
                this.InternalBuf = new byte[size];
            }

            public void Read(uint addr, byte[] data, bool isNondestructive = false) {
                for (int i = 0; i < data.Length; i++) {
                    data[i] = this.InternalBuf[i + addr];
                }
            }

            public void Write(uint addr, byte[] data) {
                for (int i = 0; i < data.Length; i++) {
                    this.InternalBuf[i + addr] = data[i];
                }
            }
        }

        /// <summary>
        /// 指定されたアドレスオフセットを使ってWriteしたデータがそのまま読み出せるか確認
        /// </summary>
        [Theory, InlineData(0), InlineData(0x1), InlineData(0x10), InlineData(0xaa), InlineData(0x100),]
        public void AllWriteRead(uint addr) {
            // incremental pattern
            var writeData = Enumerable.Range(0, 0x100)
                .Select((_, i) => (byte)i)
                .ToArray();

            // test target
            var target = new BusAccessibleSample((int)addr + writeData.Length);

            // write all
            target.Write(addr, writeData);
            // read all
            var readData = new byte[writeData.Length];
            target.Read(addr, readData, false);

            // verify
            Assert.Equal(writeData, readData);

            // verify internal buf
            // 書いてない部分は初期値0で埋められているはず
            var expectInternalData = (addr > 0) ? Enumerable.Repeat((byte)0, (int)addr).Concat(writeData).ToArray() : writeData;
            Assert.Equal(expectInternalData, target.InternalBuf);
        }

        /// <summary>
        /// 破壊読み出しレジスタを持った対象のテスト
        /// </summary>
        class BusAccessibleVolatileRegSample : IBusAccessible {
            /// <summary>
            /// データ格納先, Test Fixtureから見えるように公開してある
            /// </summary>
            public byte Data = 0x00;

            public BusAccessibleVolatileRegSample() {
            }

            public void Read(uint addr, byte[] data, bool isNondestructive = false) {
                data[0] = this.Data;
                if (!isNondestructive) {
                    this.Data = 0x00; // negate
                }
            }

            public void Write(uint addr, byte[] data) {
                this.Data = data[0];
            }
        }

        /// <summary>
        /// 非破壊読み出しが正常に機能することを確認するテスト
        /// </summary>
        [Fact]
        public void NondestructiveRead() {
            var expectValue = (byte)0xaa;
            var target = new BusAccessibleVolatileRegSample();

            // write sampledata
            target.Write8(0, expectValue);

            // read nondestructive
            Assert.Equal(expectValue, target.Read8(0, true));
            Assert.Equal(expectValue, target.Read8(0, true));
            Assert.Equal(expectValue, target.Read8(0, true));
            Assert.Equal(expectValue, target.Read8(0, true));

            // read destructive
            Assert.Equal(expectValue, target.Read8(0, false));
            Assert.Equal(0x00, target.Read8(0, false)); // destruction
        }
    }
}