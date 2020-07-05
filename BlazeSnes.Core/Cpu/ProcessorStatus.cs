using System;
using System.IO;
using System.Text;

using BlazeSnes.Core.Common;

namespace BlazeSnes.Core.Cpu {

    /// <summary>
    /// P Registerの内訳です
    /// </summary>
    [Flags]
    public enum ProcessorStatusFlag {
        E = 0x8000, // 0:Native mode, 1:Emulation mode(本来は独立したレジスタだが、一緒に格納しておく)
        N = 0x0080, // Negative
        V = 0x0040, // Overflow
        M = 0x0020, // memory mode(0:16bit, 1:8bit) native mode only
        X = 0x0010, // index mode(0:16bit, 1:8bit) native mode only
        D = 0x0008, // 0: default, 1:bcd mode
        I = 0x0004, // IRQ disable
        Z = 0x0002, // Zero
        C = 0x0001, // Carry
    }

    /// <summary>
    /// 65816 P Register
    /// </summary>
    public class ProcessorStatus : Register<ProcessorStatusFlag> {

        public ProcessorStatus() { }
        public ProcessorStatus(ProcessorStatusFlag flag) {
            this.Value = flag;
        }

        /// <summary>
        /// 引数で指定されたフラグをセット、及びクリアします
        /// </summary>
        /// <param name="isSet"></param>
        /// <param name="flags"></param>
        public void Update(bool isSet, ProcessorStatusFlag flags) {
            if (isSet) {
                this.Value |= flags;
            } else {
                this.Value &= ~flags;
            }
        }

        /// <summary>
        /// 引数に指定されたフラグがセットされていた場合にtrueを返します
        /// </summary>
        /// <param name="flags"></param>
        public bool HasSet(ProcessorStatusFlag flags) => this.value.HasFlag(flags);

        public override string ToString() {
            var sb = new StringBuilder();
            sb.Append(this.HasSet(ProcessorStatusFlag.E) ? "E" : "-");
            sb.Append(this.HasSet(ProcessorStatusFlag.N) ? "N" : "-");
            sb.Append(this.HasSet(ProcessorStatusFlag.V) ? "V" : "-");
            sb.Append(this.HasSet(ProcessorStatusFlag.M) ? "M" : "-");
            sb.Append(this.HasSet(ProcessorStatusFlag.X) ? "X" : "-");
            sb.Append(this.HasSet(ProcessorStatusFlag.D) ? "D" : "-");
            sb.Append(this.HasSet(ProcessorStatusFlag.I) ? "I" : "-");
            sb.Append(this.HasSet(ProcessorStatusFlag.Z) ? "Z" : "-");
            sb.Append(this.HasSet(ProcessorStatusFlag.C) ? "C" : "-");
            return sb.ToString();
        }
    }
}