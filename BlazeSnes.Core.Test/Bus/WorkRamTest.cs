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

        public static IEnumerable<object[]> GetAccessPattern() {
            yield return new object[] {0x00_0000, 0x2000};
            yield return new object[] {0x00_0123, 0x2000 - 0x0123};
            yield return new object[] {0x10_0000, 0x2000};
            yield return new object[] {0x3f_0456, 0x2000 - 0x0456};
            yield return new object[] {0x80_0789, 0x2000 - 0x0789};
            yield return new object[] {0xbf_0abc, 0x2000 - 0x0abc};
            yield return new object[] {0x7e_0000, 0x20000};
            yield return new object[] {0x7f_0000, 0x1000};
            yield return new object[] {0x7e_0def, 0x20000 - 0x0def};
            yield return new object[] {0x7f_ffff, 0x0001};
        }
        
        /// <summary>
        /// AddressBus A経由でRead/Writeを使った単純読み書きテスト
        /// </summary>
        [Theory, MemberData(nameof(GetAccessPattern))]
        public void AllWriteReadFromBusA(uint addr, int length) {
            var wram = new WorkRam();

            // incremental pattern
            var writeData = Enumerable.Range(0x0, length)
                .Select((_, i) => (byte)i)
                .ToArray();
            wram.Write(BusAccess.AddressA, addr,writeData);

            // read
            var readData = new byte[writeData.Length];
            wram.Read(BusAccess.AddressA, addr, readData);

            // verify
            Assert.Equal(writeData, readData);
        }

        /// <summary>
        /// AddressBusAで書いた内容をAddressBus B経由で読み出すテスト
        /// </summary>
        /// <returns></returns>
        [Theory, MemberData(nameof(GetAccessPattern))]
        public void SingleAccessFromBusB(uint addr, int length) {
            var wram = new WorkRam();

            // incremental pattern
            var writeData = Enumerable.Range(0x0, length)
                .Select((_, i) => (byte)i)
                .ToArray();
            wram.Write(BusAccess.AddressA, addr,writeData);

            // 先頭からのオフセット分を0埋めした期待データを作る
            var offset = ((addr & 0xff0000) == 0x7f0000) ? (int)(addr & 0x1ffff) : (int)(addr & 0xffff); // 7E0000 ~ 7FFFFFには全面MapされているのでOffsetの付け方が異なる
            var expectReadData = Enumerable.Repeat((byte)0x0, offset)
                .Concat(writeData)
                .ToArray();

            // read single access
            for (uint targetWramLocalAddr = 0; targetWramLocalAddr < expectReadData.Length; targetWramLocalAddr++) {
                var addrL = (byte)(targetWramLocalAddr & 0xff);
                var addrM = (byte)((targetWramLocalAddr >> 8) & 0xff);
                var addrH = (byte)((targetWramLocalAddr >> 16) & 0x01);
                // write dst addr
                wram.Write8(BusAccess.AddressB, 0x2181, addrL); // WMADDL
                wram.Write8(BusAccess.AddressB, 0x2182, addrM); // WMADDM
                wram.Write8(BusAccess.AddressB, 0x2183, addrH); // WMADDH
                // verify dst addr
                Assert.Equal(addrL, wram.Read8(BusAccess.AddressB, 0x2181));
                Assert.Equal(addrM, wram.Read8(BusAccess.AddressB, 0x2182));
                Assert.Equal(addrH, wram.Read8(BusAccess.AddressB, 0x2183));
                // verify read data
                Assert.Equal(expectReadData[targetWramLocalAddr], wram.Read8(BusAccess.AddressB, 0x2180)); // WMDATA
            }
        }

    }
}