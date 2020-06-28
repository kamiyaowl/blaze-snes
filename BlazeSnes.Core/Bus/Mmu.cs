using System;
using System.Diagnostics;
using System.IO;

using BlazeSnes.Core.Common;
using BlazeSnes.Core.External;

namespace BlazeSnes.Core.Bus {
    /// <summary>
    /// CPUからのメモリアクセスを適切にリマップします
    /// ref: https://problemkaputt.de/fullsnes.htm#snesmemorycontrol
    /// 
    /// TODO: 特定アドレスのアクセスをフックする機能があってもいいかも for #33
    /// </summary>
    public class Mmu : IBusAccessible {
        /// <summary>
        /// WRAM
        /// 00-3f: 0000-1fff: 8Kbytes
        /// 7e-7f: 0000-ffff: 128Kbytes all
        /// 80-bf: 0000-1fff: 8Kbytes
        /// </summary>
        /// <value></value>
        public IBusAccessible Wram { get; set; }
        /// <summary>
        /// I/O Ports (A-Bus/B-Bus)
        /// 00-3f: 2100-21ff: I/O ports(B-Bus)
        /// 00-3f: 4000-5fff: I/O ports
        /// 80-bf: 2100-21ff: I/O ports(B-Bus)
        /// 80-bf: 4000-5fff: I/O ports
        /// </summary>
        /// <value></value>
        public IBusAccessible PpuControlReg { get; set; }
        public IBusAccessible ApuControlReg { get; set; }
        public IBusAccessible OnChipIoPort { get; set; }
        public IBusAccessible DmaControlReg { get; set; }
        /// <summary>
        /// Expansion port
        /// 00-3f: 6000-7ffff
        /// 80-bf: 6000-7ffff
        /// </summary>
        /// <value></value>
        public IBusAccessible Expansion { get; set; }
        /// <summary>
        /// Cartridge ROM/RAM
        /// 00-3f: 8000-ffff: WS1 LoROM  2048Kbytes
        /// 40-7f: 0000-ffff: WS1 HiROM  3968Kbytes
        /// 80-bf: 8000-ffff: WS2 LoROM  2048Kbytes
        /// c0-ff: 0000-ffff: WS2 HiROM  3968Kbytes
        /// </summary>
        /// <value></value>
        public IBusAccessible Cartridge { get; set; }

        /// <summary>
        /// アクセス先のアドレスから対象のペリフェラルとバス種別を取得します
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        public (IBusAccessible, BusAccess) GetTarget(uint addr) {
            Debug.Assert((addr & 0xff_ffff) == 0x0); // 24bit以上のアクセスは存在しないはず

            var bank = (addr >> 16) & 0xff;
            var offset = (addr & 0xffff);
            var (target, access) = bank switch // TODO: unused regionをもう少し厳格に定義してOpenBusで返す(例外にしない)
            {
                var b when ((b <= 0x3f) || ((0x80 <= b) && (b <= 0xbf))) => offset switch
                {
                    var o when (o <= 0x1fff) => (Wram, BusAccess.AddressA),
                    var o when (o <= 0x20ff) => throw new ArgumentOutOfRangeException($"unused 2000-20ff. addr:${addr:x}"),
                    var o when (o <= 0x21ff) => (OnChipIoPort, BusAccess.AddressB),
                    var o when (o <= 0x3fff) => throw new ArgumentOutOfRangeException($"unused 2200-3fff. addr:${addr:x}"),
                    var o when (o <= 0x41ff) => (OnChipIoPort, BusAccess.AddressA),
                    var o when (o <= 0x5fff) => (OnChipIoPort, BusAccess.AddressA),
                    var o when (o <= 0x7fff) => (Expansion, BusAccess.AddressA),
                    _ => (Cartridge, BusAccess.AddressA), // 8000 - ffff: WS1 LoROM
                },
                var b when ((b <= 0x7d) || (0xc0 <= b)) => (Cartridge, BusAccess.AddressA), // 40-7d, c0-ff
                var b when (b <= 0x7f) => (Wram, BusAccess.AddressA), // 7e-7f
                _ => throw new ArgumentOutOfRangeException($"リマップ不能なbankにアクセスが有りました addr:${addr:x}"),
            };

            return (target, access);
        }

        public void Read(uint addr, byte[] data, bool isNondestructive = false) => Read(BusAccess.Unspecified, addr, data, isNondestructive);
        public void Write(uint addr, byte[] data) => Write(BusAccess.Unspecified, addr, data);

        public void Read(BusAccess access, uint addr, byte[] data, bool isNondestructive = false) {
            Debug.Assert(access == BusAccess.Unspecified); // TODO: OpenBusの振る舞いを要確認。今のところは気にしなくていいはず...?
            Debug.Assert(data.Length > 0);

            var (target, targetAccess) = GetTarget(addr);
            // TODO: (Read)OpenBus対応を入れる
            target.Read(targetAccess, addr, data, isNondestructive);
        }

        public void Write(BusAccess access, uint addr, byte[] data) {
            Debug.Assert(access == BusAccess.Unspecified);
            Debug.Assert(data.Length > 0);

            var (target, targetAccess) = GetTarget(addr);
            // TODO: (Write)OpenBus対応を入れる
            target.Write(targetAccess, addr, data);
        }
    }
}