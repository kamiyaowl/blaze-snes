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
        /// 最後のRead値、OpenBusアクセス時に返す値
        /// </summary>
        /// <value></value>
        public byte LatestReadData { get; internal set; } = 0x0;

        /// <summary>
        /// アクセス先のアドレスから対象のペリフェラルとバス種別を取得します
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        public IBusAccessible GetTarget(uint addr) {
            Debug.Assert((addr & 0xff_ffff) == 0x0); // 24bit以上のアクセスは存在しないはず

            var bank = (addr >> 16) & 0xff;
            var offset = (addr & 0xffff);
            var target = bank switch
            {
                var b when ((b <= 0x3f) || ((0x80 <= b) && (b <= 0xbf))) => offset switch
                {
                    var o when (o <= 0x1fff) => Wram,
                    var o when (o <= 0x20ff) => null, // unused
                    var o when (o <= 0x21ff) => OnChipIoPort,
                    var o when (o <= 0x3fff) => null, // unused
                    var o when (o <= 0x41ff) => OnChipIoPort,
                    var o when (o <= 0x5fff) => OnChipIoPort,
                    var o when (o <= 0x7fff) => Expansion,
                    _ => Cartridge, // 8000 - ffff: WS1 LoROM
                },
                var b when ((b <= 0x7d) || (0xc0 <= b)) => Cartridge, // 40-7d, c0-ff
                var b when (b <= 0x7f) => Wram, // 7e-7f
                _ => null, // open bus
            };

            return target;
        }

        public bool Read(uint addr, byte[] data, bool isNondestructive = false) {
            Debug.Assert(data.Length > 0);

            var target = GetTarget(addr);
            // OpenBus対応
            if (!target?.Read(addr, data, isNondestructive) ?? false) {
                Debug.Fail($"Open Bus Readを検出 ${addr:x}"); // TODO: デバッグ用に入れてあるが適正なOpen Busアクセスであれば外す
                // OpenBusは最後に読めたデータを返す
                Debug.Assert(data.Length == 1);
                data[0] = LatestReadData;
                return false;
            }
            LatestReadData = data[^0]; // 最後に読めたデータを控える
            return true;
        }

        public bool Write(uint addr, byte[] data) {
            Debug.Assert(data.Length > 0);

            var target = GetTarget(addr);
            // OpenBus対応
            if (!target?.Write(addr, data) ?? false) {
                Debug.Fail($"Open Bus Writeを検出 ${addr:x}"); // TODO: デバッグ用に入れてあるが適正なOpen Busアクセスであれば外す
                return false;
            }
            return true;
        }
    }
}