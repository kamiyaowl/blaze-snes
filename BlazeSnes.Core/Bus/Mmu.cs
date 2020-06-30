using System;
using System.Diagnostics;
using System.IO;

using BlazeSnes.Core.Common;
using BlazeSnes.Core.External;

namespace BlazeSnes.Core.Bus {
    /// <summary>
    /// CPUからのメモリアクセスを適切にリマップします
    /// ref: https://problemkaputt.de/fullsnes.htm#snesmemorycontrol
    /// </summary>
    public class Mmu : IBusAccessible {
        /// <summary>
        /// WRAM
        /// 00-3f: 0000-1fff: 8Kbytes
        /// 7e-7f: 0000-ffff: 128Kbytes all
        /// 80-bf: 0000-1fff: 8Kbytes
        /// </summary>
        /// <value></value>
        public IBusAccessible Wram { get; internal set; }
        /// <summary>
        /// PPU
        /// 00-3f, 80-bf: 2100-213f
        /// </summary>
        /// <value></value>
        public IBusAccessible PpuControlReg { get; internal set; }
        /// <summary>
        /// APU
        /// 00-3f, 80-bf: 2134-217f
        /// </summary>
        /// <value></value>
        public IBusAccessible ApuControlReg { get; internal set; }
        /// <summary>
        /// Joypad, Clock Div, Timer, etc.!--.!--.
        /// 00-3f, 80-bf: 4000-42ff
        /// </summary>
        /// <value></value>
        public IBusAccessible OnChipIoPort { get; internal set; }
        /// <summary>
        /// DMA Channle(0..7)
        /// 00-3f, 80-bf: 4300--5fff
        /// </summary>
        /// <value></value>
        public IBusAccessible DmaControlReg { get; internal set; }
        /// Cartridge ROM/RAM
        /// 00-3f: 8000-ffff: WS1 LoROM  2048Kbytes
        /// 40-7f: 0000-ffff: WS1 HiROM  3968Kbytes
        /// 80-bf: 8000-ffff: WS2 LoROM  2048Kbytes
        /// c0-ff: 0000-ffff: WS2 HiROM  3968Kbytes
        /// Expansion port
        /// 00-3f: 6000-7ffff
        /// 80-bf: 6000-7ffff
        /// </summary>
        /// <value></value>
        public IBusAccessible Cartridge { get; internal set; }
        /// <summary>
        /// 最後のRead値、OpenBusアクセス時に返す値
        /// </summary>
        /// <value></value>
        public byte LatestReadData { get; internal set; } = 0x0;

        public Mmu(IBusAccessible wram, IBusAccessible ppu, IBusAccessible apu, IBusAccessible onchip, IBusAccessible dma, IBusAccessible cartridge) {
            this.Wram = wram;
            this.PpuControlReg = ppu;
            this.ApuControlReg = apu;
            this.OnChipIoPort = onchip;
            this.DmaControlReg = dma;
            this.Cartridge = cartridge;
        }

        /// <summary>
        /// アクセス先のアドレスから対象のペリフェラルとバス種別を取得します
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        public IBusAccessible GetTarget(uint addr) {
            Debug.Assert((addr & 0xff00_0000) == 0x0); // 24bit以上のアクセスは存在しないはず

            var bank = (addr >> 16) & 0xff;
            var offset = (addr & 0xffff);
            var target = bank switch
            {
                var b when ((b <= 0x3f) || ((0x80 <= b) && (b <= 0xbf))) => offset switch
                {
                    var o when (o <= 0x1fff) => Wram,
                    var o when (o <= 0x20ff) => null, // unused
                    var o when (o <= 0x213f) => PpuControlReg,
                    var o when (o <= 0x217f) => ApuControlReg,
                    var o when (o <= 0x2183) => Wram,
                    var o when (o <= 0x21ff) => Cartridge, // Expansion(B-Bus)
                    var o when (o <= 0x3fff) => Cartridge, // unused, Expansion(A-Bus)
                    var o when (o <= 0x42ff) => OnChipIoPort,
                    var o when (o <= 0x5fff) => DmaControlReg, // DMA Channel 0..7
                    _ => Cartridge, // 6000-7fff: Expansion(e.g. Battery Backed RAM), 8000 - ffff: ROM
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
                Array.Fill(data, LatestReadData); // すべて最後に読めた値で埋める
                return false;
            }
            LatestReadData = data[^1]; // 最後に読めたデータを控える
            return true;
        }

        public bool Write(uint addr, in byte[] data) {
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