using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using BlazeSnes.Core.Bus;
using BlazeSnes.Core.Common;

using Xunit;
using Xunit.Sdk;

namespace BlazeSnes.Core.Test.Common {
    public class WorkRamTest {
        /// <summary>
        /// テストする値
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<object[]> GetAccessPattern() {
            yield return new object[] { 0x00_0000, 0x2000 };
            yield return new object[] { 0x00_0123, 0x2000 - 0x0123 };
            yield return new object[] { 0x10_0000, 0x2000 };
            yield return new object[] { 0x3f_0456, 0x2000 - 0x0456 };
            yield return new object[] { 0x80_0789, 0x2000 - 0x0789 };
            yield return new object[] { 0xbf_0abc, 0x2000 - 0x0abc };
            yield return new object[] { 0x7e_0000, 0x20000 };
            yield return new object[] { 0x7f_0000, 0x1000 };
            yield return new object[] { 0x7e_0def, 0x20000 - 0x0def };
            yield return new object[] { 0x7f_ffff, 0x0001 };
        }

        /// <summary>
        /// インクリメンタルなデータパターンを作成します
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static byte[] CreateIncrementalData(int length) {
            return Enumerable.Range(0x0, length)
                .Select((_, i) => (byte)i)
                .ToArray();
        }

        /// <summary>
        /// 先頭を0埋めした期待値データを作成します
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="writeData"></param>
        /// <returns></returns>
        public static byte[] CreateExpectData(uint addr, byte[] writeData) {
            var offset = ((addr & 0xff0000) == 0x7f0000) ? (int)(addr & 0x1ffff) : (int)(addr & 0xffff); // 7E0000 ~ 7FFFFFには全面MapされているのでOffsetの付け方が異なる
            var expectReadData = Enumerable.Repeat((byte)0x0, offset)
                .Concat(writeData)
                .ToArray();
            return expectReadData;
        }


        /// <summary>
        /// AddressBus A経由でRead/Writeを使った単純読み書きテスト
        /// </summary>
        [Theory, MemberData(nameof(GetAccessPattern))]
        public void AllWriteReadFromBusA(uint addr, int length) {
            var wram = new WorkRam();

            // incremental pattern
            byte[] writeData = CreateIncrementalData(length);
            wram.Write(addr, writeData);

            // read
            var readData = new byte[writeData.Length];
            wram.Read(addr, readData);

            // verify
            Assert.Equal(writeData, readData);
        }

        /// <summary>
        /// AddressBusAで書いた内容をAddressBus B経由で読み出すテスト
        /// 毎回WMADDL, WMADDM, WMADDHを設定する
        /// </summary>
        /// <returns></returns>
        [Theory, MemberData(nameof(GetAccessPattern))]
        public void SingleReadFromBusB(uint addr, int length) {
            var wram = new WorkRam();

            // incremental pattern
            byte[] writeData = CreateIncrementalData(length);
            wram.Write(addr, writeData);

            // 先頭からのオフセット分を0埋めした期待データを作る
            byte[] expectReadData = CreateExpectData(addr, writeData);

            // read single access
            for (uint targetWramLocalAddr = 0; targetWramLocalAddr < expectReadData.Length; targetWramLocalAddr++) {
                var addrL = (byte)(targetWramLocalAddr & 0xff);
                var addrM = (byte)((targetWramLocalAddr >> 8) & 0xff);
                var addrH = (byte)((targetWramLocalAddr >> 16) & 0x01);
                if (addrL == 1 && addrM == 0 && addrH == 1) {
                    Console.WriteLine("Adsf");
                }
                // write dst addr
                wram.Write8(0x2181, addrL); // WMADDL
                wram.Write8(0x2182, addrM); // WMADDM
                wram.Write8(0x2183, addrH); // WMADDH
                // verify dst addr
                Assert.Equal(addrL, wram.Read8(0x2181).Value);
                Assert.Equal(addrM, wram.Read8(0x2182).Value);
                Assert.Equal(addrH, wram.Read8(0x2183).Value);
                // verify read data
                Assert.Equal(expectReadData[targetWramLocalAddr], wram.Read8(0x2180).Value); // WMDATA
            }
        }

        /// <summary>
        /// AddressBusAで書いた内容をAddressBus B経由で読み出すテスト
        /// WMADDL, WMADDM, WMADDHは初回のみセットし、インクリメンタルアクセスを使用する
        /// </summary>
        /// <returns></returns>
        [Theory, MemberData(nameof(GetAccessPattern))]
        public void BurstReadFromBusB(uint addr, int length) {
            var wram = new WorkRam();

            // incremental pattern
            byte[] writeData = CreateIncrementalData(length);
            wram.Write(addr, writeData);

            // 先頭からのオフセット分を0埋めした期待データを作る
            var expectReadData = CreateExpectData(addr, writeData);

            // 先頭に一回だけ書き込む
            wram.Write8(0x2181, 0x00); // WMADDL
            wram.Write8(0x2182, 0x00); // WMADDM
            wram.Write8(0x2183, 0x00); // WMADDH

            // 非破壊読み出しで進まないことを確認する
            wram.Read8(0x2180, true);
            wram.Read8(0x2180, true);
            wram.Read8(0x2180, true);
            Assert.Equal(0x00, wram.Read8(0x2181).Value);
            Assert.Equal(0x00, wram.Read8(0x2182).Value);
            Assert.Equal(0x00, wram.Read8(0x2183).Value);

            // read burst access
            for (uint targetWramLocalAddr = 0; targetWramLocalAddr < expectReadData.Length; targetWramLocalAddr++) {
                var addrL = (byte)(targetWramLocalAddr & 0xff);
                var addrM = (byte)((targetWramLocalAddr >> 8) & 0xff);
                var addrH = (byte)((targetWramLocalAddr >> 16) & 0x01);

                // verify dst addr
                Assert.Equal(addrL, wram.Read8(0x2181).Value);
                Assert.Equal(addrM, wram.Read8(0x2182).Value);
                Assert.Equal(addrH, wram.Read8(0x2183).Value);

                // verify read data
                Assert.Equal(expectReadData[targetWramLocalAddr], wram.Read8(0x2180)); // WMDATA
            }
        }

        /// <summary>
        /// AddressBus B経由でWMDATA regを使って順繰りアドレス指定で書き込みを行うテスト
        /// </summary>
        [Theory, MemberData(nameof(GetAccessPattern))]
        public void SingleWriteFromBusB(uint addr, int length) {
            var wram = new WorkRam();

            // incremental pattern
            byte[] writeData = CreateIncrementalData(length);

            // 順番に書き込み
            for (int i = 0; i < writeData.Length; i++) {
                // expect address
                var expectAddr = addr + i;
                var expectAddrL = (byte)(expectAddr & 0xff);
                var expectAddrM = (byte)((expectAddr >> 8) & 0xff);
                var expectAddrH = (byte)((expectAddr >> 16) & 0x01);
                wram.Write8(0x2181, expectAddrL);
                wram.Write8(0x2182, expectAddrM);
                wram.Write8(0x2183, expectAddrH);
                Assert.Equal(expectAddrL, wram.Read8(0x2181).Value);
                Assert.Equal(expectAddrM, wram.Read8(0x2182).Value);
                Assert.Equal(expectAddrH, wram.Read8(0x2183).Value);

                // write
                wram.Write8(0x2180, writeData[i]);
            }

            // Test用のaddrをそのまま書いた場合、read時にremapされてしまうのでWMDATA経由でRead Verifyもやる
            // SingleReadFromBusBで動作確認済
            for (int i = 0; i < writeData.Length; i++) {
                // expect address
                var expectAddr = addr + i;
                var expectAddrL = (byte)(expectAddr & 0xff);
                var expectAddrM = (byte)((expectAddr >> 8) & 0xff);
                var expectAddrH = (byte)((expectAddr >> 16) & 0x01);
                wram.Write8(0x2181, expectAddrL);
                wram.Write8(0x2182, expectAddrM);
                wram.Write8(0x2183, expectAddrH);
                Assert.Equal(expectAddrL, wram.Read8(0x2181).Value);
                Assert.Equal(expectAddrM, wram.Read8(0x2182).Value);
                Assert.Equal(expectAddrH, wram.Read8(0x2183).Value);

                // read verify
                Assert.Equal(writeData[i], wram.Read8(0x2180).Value);
            }
        }

        /// <summary>
        /// AddressBus B経由でWMDATA regを使って一気に書き込みを行うテスト
        /// </summary>
        [Theory, MemberData(nameof(GetAccessPattern))]
        public void BurstWriteFromBusB(uint addr, int length) {
            var wram = new WorkRam();

            // incremental pattern
            byte[] writeData = CreateIncrementalData(length);

            // 最初のアドレスをセット
            var addrL = (byte)(addr & 0xff);
            var addrM = (byte)((addr >> 8) & 0xff);
            var addrH = (byte)((addr >> 16) & 0x01);
            wram.Write8(0x2181, addrL);
            wram.Write8(0x2182, addrM);
            wram.Write8(0x2183, addrH);
            Assert.Equal(addrL, wram.Read8(0x2181).Value);
            Assert.Equal(addrM, wram.Read8(0x2182).Value);
            Assert.Equal(addrH, wram.Read8(0x2183).Value);

            // 順番に書き込み
            for (int i = 0; i < writeData.Length; i++) {
                // expect address
                var expectAddr = addr + i;
                var expectAddrL = (byte)(expectAddr & 0xff);
                var expectAddrM = (byte)((expectAddr >> 8) & 0xff);
                var expectAddrH = (byte)((expectAddr >> 16) & 0x01);
                Assert.Equal(expectAddrL, wram.Read8(0x2181).Value);
                Assert.Equal(expectAddrM, wram.Read8(0x2182).Value);
                Assert.Equal(expectAddrH, wram.Read8(0x2183).Value);

                // write
                wram.Write8(0x2180, writeData[i]);
            }

            // Test用のaddrをそのまま書いた場合、read時にremapされてしまうのでWMDATA経由でRead Verifyもやる
            // BurstReadFromBusBで動作確認済
            wram.Write8(0x2181, addrL);
            wram.Write8(0x2182, addrM);
            wram.Write8(0x2183, addrH);
            for (int i = 0; i < writeData.Length; i++) {
                // expect address
                var expectAddr = addr + i;
                var expectAddrL = (byte)(expectAddr & 0xff);
                var expectAddrM = (byte)((expectAddr >> 8) & 0xff);
                var expectAddrH = (byte)((expectAddr >> 16) & 0x01);
                Assert.Equal(expectAddrL, wram.Read8(0x2181).Value);
                Assert.Equal(expectAddrM, wram.Read8(0x2182).Value);
                Assert.Equal(expectAddrH, wram.Read8(0x2183).Value);

                // read verify
                Assert.Equal(writeData[i], wram.Read8(0x2180).Value);
            }
        }

    }
}