using System;
using System.Diagnostics;
using System.IO;

using BlazeSnes.Core.Common;

namespace BlazeSnes.Core.Bus {
    /// <summary>
    /// WRAMを示します
    /// xx0000~xx1FFF, xx=00~3f, 80~bf に先頭8Kがミラー
    /// 7E0000-7FFFFF に全体がマップ
    /// 以下のレジスタ経由でのアクセスも可能(Addres Bus B)
    /// 2180 WMDATA - { WMADDH[0], WMADDRM[7:0], WMADDL[7:0] } のデータに読み書き。オートインクリメントされる
    /// 2181 WMADDL
    /// 2182 WMADDM
    /// 2183 WMADDH
    /// </summary>
    public class WorkRam : IBusAccessible {
        /// <summary>
        /// 128Kbytes, 17bit
        /// </summary>
        public const int SIZE = 0x20000;
        /// <summary>
        /// 128Kbytes, 17bit
        /// </summary>
        public const int MASK = 0x1ffff;
        /// <summary>
        /// Work Buffer実体
        /// </summary>
        /// <value></value>
        public byte[] WorkBuffer { get; internal set; } = new byte[SIZE];
        /// <summary>
        /// WMADDL, WMADDM, WMADDLの元データ
        /// </summary>
        /// <value></value>
        public uint WmAddr { get; internal set; }
        /// <summary>
        /// 2181 WMADDRL
        /// </summary>
        /// <returns></returns>
        public byte WmAddrL {
            get => (byte)(WmAddr & 0xff);
            set {
                WmAddr &= 0x01ff00; // 現在の内容をクリア
                WmAddr |= value;
            }
        }
        /// <summary>
        /// 2182 WMADDRM
        /// </summary>
        /// <returns></returns>
        public byte WmAddrM {
            get => (byte)((WmAddr >> 8) & 0xff);
            set {
                WmAddr &= 0x0100ff; // 現在の内容をクリア
                WmAddr |= (uint)(value << 8);
            }
        }
        /// <summary>
        /// 2183 WMADDRH
        /// </summary>
        /// <value></value>
        public byte WmAddrH {
            get => (byte)((WmAddr >> 16) & 0xff);
            set {
                WmAddr &= 0x0ffff; // 現在の内容をクリア
                WmAddr |= (uint)((value & 0x1) << 16);
            }
        }

        /// <summary>
        /// Bufのローカルアドレスに変換します
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        public static uint ConvertToLocalAddr(uint addr) {
            var bank = (addr >> 16) & 0xff;
            var localAddr = bank switch
            {
                var x when (x <= 0x3f) => addr & 0x1fff, // 先頭8Kミラー
                var x when (0x7e <= x && x <= 0x7f) => addr & MASK, // 全域remap
                var x when (0x80 <= x && x <= 0xbf) => addr & 0x1fff, // 先頭8Kミラー
                _ => throw new ArgumentOutOfRangeException($"WRAMへの範囲外アクセス ${addr:x}"),
            };
            return localAddr;
        }

        /// <summary>
        /// WorkRAMへの読み出しを処理します
        /// </summary>
        /// <param name="access"></param>
        /// <param name="addr"></param>
        /// <param name="data"></param>
        /// <param name="isNondestructive"></param>
        public void Read(BusAccess access, uint addr, byte[] data, bool isNondestructive = false) {
            Debug.Assert(data.Length > 0);

            switch (access) {
                case BusAccess.AddressA:
                    // ローカルバッファのアドレス変換だけ行って読み込む
                    var localAddr = ConvertToLocalAddr(addr);
                    Buffer.BlockCopy(this.WorkBuffer, (int)localAddr, data, 0, data.Length);
                    break;
                case BusAccess.AddressB:
                    // DMA向けの読み出しレジスタにアクセス
                    for (uint i = 0; i < data.Length; i++) {
                        switch (addr + i) {
                            case 0x2180: // WMDATA
                                data[i] = this.WorkBuffer[this.WmAddr];
                                if (!isNondestructive) {
                                    this.WmAddr++; // address auto increment
                                }
                                break;
                            case 0x2181: // WMADDL
                                data[i] = this.WmAddrL;
                                break;
                            case 0x2182: // WMADDM
                                data[i] = this.WmAddrM;
                                break;
                            case 0x2183: // WMADDH
                                data[i] = this.WmAddrH;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException($"AddressBus Bからの範囲外アクセス addr:{addr:x}");
                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"AddressBus A/Bのみ受付 addr:{addr:x}");
            }

        }

        /// <summary>
        /// WorkRAMの書き込みを処理します
        /// </summary>
        /// <param name="access"></param>
        /// <param name="addr"></param>
        /// <param name="data"></param>
        public void Write(BusAccess access, uint addr, byte[] data) {
            Debug.Assert(data.Length > 0);

            switch (access) {
                case BusAccess.AddressA:
                    // ローカルバッファのアドレス変換だけ行って書き込む
                    var localAddr = ConvertToLocalAddr(addr);
                    Buffer.BlockCopy(data, 0, this.WorkBuffer, (int)localAddr, data.Length);
                    break;
                case BusAccess.AddressB:
                    // DMA向けの読み出しレジスタにアクセス
                    for (uint i = 0; i < data.Length; i++) {
                        switch (addr + i) {
                            case 0x2180: // WMDATA
                                this.WorkBuffer[this.WmAddr] = data[i];
                                this.WmAddr++; // address auto increment
                                break;
                            case 0x2181: // WMADDL
                                this.WmAddrL = data[i];
                                break;
                            case 0x2182: // WMADDM
                                this.WmAddrM = data[i];
                                break;
                            case 0x2183: // WMADDH
                                this.WmAddrH = data[i];
                                break;
                            default:
                                throw new ArgumentOutOfRangeException($"AddressBus Bからの範囲外アクセス addr:{addr:x}");
                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"AddressBus A/Bのみ受付 addr:{addr:x}");
            }

        }
    }
}