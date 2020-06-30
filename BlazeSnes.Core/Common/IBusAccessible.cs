using System;
using System.Diagnostics;
using System.IO;

namespace BlazeSnes.Core.Common {
    /// <summary>
    /// Bus Access可能であることを示します
    /// </summary>
    public interface IBusAccessible {
        /// <summary>
        /// 指定されたアドレスの内容を読み出します
        /// </summary>
        /// <param name="addr">アクセス先、Bankも含む</param>
        /// <param name="data">読み出し先</param>
        /// <param name="isNondestructive">非破壊読み出しならtrue(e.g. BIT など)</param>
        bool Read(uint addr, byte[] data, bool isNondestructive = false);

        /// <summary>
        /// 指定されたアドレスにdataの内容を書き込みます
        /// </summary>
        /// <param name="addr">アクセス先、Bankも含む</param>
        /// <param name="data">書き込むデータ</param>
        bool Write(uint addr, in byte[] data);
    }

    /// <summary>
    /// IBusAccessibleの拡張メソッド
    /// Read8/16/32などをおいているのはLong AccessなどでRead8を複数回やられるたびにアドレス解決するとパフォーマンス上問題があるため
    /// </summary>
    public static class BusAccessibleExtension {
        /// <summary>
        /// 指定したアドレスから1byte読み出します
        /// </summary>
        /// <param name="bus">読み出し対象</param>
        /// <param name="addr">読み出し先</param>
        /// <param name="isNondestructive">非破壊読み出しならtrue</param>
        /// <returns></returns>
        public static byte Read8(this IBusAccessible bus, uint addr, bool isNondestructive = false) {
            var dst = new byte[1];
            bus.Read(addr, dst, isNondestructive);
            return dst[0];
        }

        /// <summary>
        /// 指定したアドレスから2byte読み出します
        /// </summary>
        /// <param name="bus">読み出し対象</param>
        /// <param name="addr">読み出し先</param>
        /// <param name="isNondestructive">非破壊読み出しならtrue</param>
        /// <returns></returns>
        public static ushort Read16(this IBusAccessible bus, uint addr, bool isNondestructive = false) {
            var dst = new byte[2];
            bus.Read(addr, dst, isNondestructive);
            return (ushort)(dst[0] | (dst[1] << 8));
        }

        /// <summary>
        /// 指定したアドレスから3byte読み出します
        /// </summary>
        /// <param name="bus">読み出し対象</param>
        /// <param name="addr">読み出し先</param>
        /// <param name="isNondestructive">非破壊読み出しならtrue</param>
        /// <returns></returns>
        public static uint Read24(this IBusAccessible bus, uint addr, bool isNondestructive = false) {
            var dst = new byte[3];
            bus.Read(addr, dst, isNondestructive);
            return (uint)(dst[0] | (dst[1] << 8) | (dst[2] << 16));
        }

        /// <summary>
        /// 指定したアドレスから4byte読み出します
        /// </summary>
        /// <param name="bus">読み出し対象</param>
        /// <param name="addr">読み出し先</param>
        /// <param name="isNondestructive">非破壊読み出しならtrue</param>
        /// <returns></returns>
        public static uint Read32(this IBusAccessible bus, uint addr, bool isNondestructive = false) {
            var dst = new byte[4];
            bus.Read(addr, dst, isNondestructive);
            return (uint)(dst[0] | (dst[1] << 8) | (dst[2] << 16) | (dst[3] << 24));
        }

        /// <summary>
        /// 指定したアドレスに1byte書き込みます
        /// </summary>
        /// <param name="bus">書き込み対象</param>
        /// <param name="addr">書き込み先</param>
        /// <param name="data">書き込みデータ</param>
        /// <returns></returns>
        public static bool Write8(this IBusAccessible bus, uint addr, byte data) {
            var src = new byte[] {
                data,
            };
            return bus.Write(addr, src);
        }

        /// <summary>
        /// 指定したアドレスに2byte書き込みます
        /// </summary>
        /// <param name="bus">書き込み対象</param>
        /// <param name="addr">書き込み先</param>
        /// <param name="data">書き込みデータ</param>
        /// <returns></returns>
        public static bool Write16(this IBusAccessible bus, uint addr, ushort data) {
            var src = new byte[] {
                (byte)(data & 0xff),
                (byte)((data >> 8) & 0xff),
            };
            return bus.Write(addr, src);
        }

        /// <summary>
        /// 指定したアドレスに3byte書き込みます
        /// </summary>
        /// <param name="bus">書き込み対象</param>
        /// <param name="access">バスの種類</param>
        /// <param name="addr">書き込み先</param>
        /// <param name="data">書き込みデータ</param>
        /// <returns></returns>
        public static bool Write24(this IBusAccessible bus, uint addr, uint data) {
            var src = new byte[] {
                (byte)(data & 0xff),
                (byte)((data >> 8) & 0xff),
                (byte)((data >> 16) & 0xff),
            };
            return bus.Write(addr, src);
        }

        /// <summary>
        /// 指定したアドレスに4byte書き込みます
        /// </summary>
        /// <param name="bus">書き込み対象</param>
        /// <param name="access">バスの種類</param>
        /// <param name="addr">書き込み先</param>
        /// <param name="data">書き込みデータ</param>
        /// <returns></returns>
        public static bool Write32(this IBusAccessible bus, uint addr, uint data) {
            var src = new byte[] {
                (byte)(data & 0xff),
                (byte)((data >> 8) & 0xff),
                (byte)((data >> 16) & 0xff),
                (byte)((data >> 24) & 0xff),
            };
            return bus.Write(addr, src);
        }
    }
}