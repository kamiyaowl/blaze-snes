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

        public override string ToString() {
            var sb = new StringBuilder();
            sb.Append(Value.HasFlag(ProcessorStatusFlag.E) ? "E" : "-");
            sb.Append(Value.HasFlag(ProcessorStatusFlag.N) ? "N" : "-");
            sb.Append(Value.HasFlag(ProcessorStatusFlag.V) ? "V" : "-");
            sb.Append(Value.HasFlag(ProcessorStatusFlag.M) ? "M" : "-");
            sb.Append(Value.HasFlag(ProcessorStatusFlag.X) ? "X" : "-");
            sb.Append(Value.HasFlag(ProcessorStatusFlag.D) ? "D" : "-");
            sb.Append(Value.HasFlag(ProcessorStatusFlag.I) ? "I" : "-");
            sb.Append(Value.HasFlag(ProcessorStatusFlag.Z) ? "Z" : "-");
            sb.Append(Value.HasFlag(ProcessorStatusFlag.C) ? "C" : "-");
            return sb.ToString();
        }
    }
}