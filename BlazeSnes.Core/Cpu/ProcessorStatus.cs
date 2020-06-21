using BlazeSnes.Core.Common;
using System;
using System.IO;
using System.Text;

namespace BlazeSnes.Core.Cpu {
    /// <summary>
    /// 65816 P Register
    /// </summary>
    public class ProcessorStatus : Register<ushort> {
        /// <summary>
        /// P Registerの内訳です
        /// </summary>
        [Flags]
        public enum Flags {
            E = 0x8000, // 0:Native mode, 1:Emulation mode(本来は独立したレジスタだが、一緒に格納しておく)
            N = 0x0080, // Negative
            V = 0x0040, // Overflow
            M = 0x0020, // A reg size(0:16bit, 1:8bit) native mode only
            X = 0x0010, // X/Y reg size(0:16bit, 1:8bit) native mode only
            B = 0x0010, // Break emulation mode only
            I = 0x0004, // IRQ disable
            Z = 0x0002, // Zero
            C = 0x0001, // Carry
        }

        /// <summary>
        /// Enumに変換した値を取得する
        /// </summary>
        /// <returns></returns>
        public Flags Flag => (Flags)this.value;

        public override string ToString() {
            var sb = new StringBuilder();
            sb.Append(Flag.HasFlag(Flags.E) ? "E" : "-");
            sb.Append(Flag.HasFlag(Flags.N) ? "N" : "-");
            sb.Append(Flag.HasFlag(Flags.V) ? "V" : "-");
            sb.Append(Flag.HasFlag(Flags.M) ? "M" : "-");
            sb.Append(Flag.HasFlag(Flags.X) ? "X" : "-");
            sb.Append(Flag.HasFlag(Flags.B) ? "B" : "-");
            sb.Append(Flag.HasFlag(Flags.I) ? "I" : "-");
            sb.Append(Flag.HasFlag(Flags.Z) ? "Z" : "-");
            sb.Append(Flag.HasFlag(Flags.C) ? "C" : "-");
            return sb.ToString();
        }
    }
}