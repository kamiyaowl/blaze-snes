using System;
using System.Diagnostics;
using System.IO;

using BlazeSnes.Core.Common;

namespace BlazeSnes.Core.Cpu {
    /// <summary>
    /// 65816 のレジスタセット
    /// </summary>
    public class CpuRegister : IResetable {
        /// <summary>
        /// Accumulator Register
        /// </summary>
        /// <value></value>
        public ushort A { get; set; }
        /// <summary>
        /// Index Register
        /// </summary>
        /// <value></value>
        public ushort X { get; set; }
        /// <summary>
        /// Index Register
        /// </summary>
        /// <value></value>
        public ushort Y { get; set; }
        /// <summary>
        /// Stack Pointer
        /// </summary>
        /// <value></value>
        public ushort SP { get; set; }
        /// <summary>
        /// Data Bank Register
        /// </summary>
        /// <value></value>
        public ushort DB { get; set; }
        /// <summary>
        /// Direct Page Register
        /// </summary>
        /// <value></value>
        public ushort DP { get; set; }
        /// <summary>
        /// Program Bank Register
        /// </summary>
        /// <value></value>
        public ushort PB { get; set; }
        /// <summary>
        /// Processor Status
        /// </summary>
        /// <value></value>
        public ProcessorStatus P { get; set; } = new ProcessorStatus();
        /// <summary>
        /// Program Counter
        /// </summary>
        /// <value></value>
        public ushort PC { get; set; }

        /// <summary>
        /// Memory/Acccumulatorの16bit Accessが有効ならtrueを返します
        /// </summary>
        /// <returns></returns>
        public bool Is16bitMemoryAccess => !P.HasFlag(ProcessorStatusFlag.E) && !P.HasFlag(ProcessorStatusFlag.M);
        /// <summary>
        /// Indexに16bit Accesが有効ならtrueを返します
        /// </summary>
        /// <returns></returns>
        public bool Is16bitIndexAccess => !P.HasFlag(ProcessorStatusFlag.E) && !P.HasFlag(ProcessorStatusFlag.X);
        /// <summary>
        /// データバンクの値をSystemAddrに変換します
        /// </summary>
        /// <returns></returns>
        public uint DataBankAddr => (uint)this.DB << 16;
        /// <summary>
        /// ページバンクの値をSystemAddrに変換します
        /// </summary>
        /// <returns></returns>
        public uint PageBankAddr => (uint)this.PB << 16;
        /// <summary>
        /// Memory Registerのビット幅を考慮して入出力します
        /// </summary>
        /// <value></value>
        public ushort AConsideringMemoryReg {
            get => Is16bitMemoryAccess ? (ushort)A : (ushort)(A & 0xff);
            set => A = Is16bitMemoryAccess ? value : (ushort)(value & 0xff);
        }
        /// <summary>
        /// Index Registerのビット幅を考慮して入出力します
        /// </summary>
        /// <value></value>
        public ushort XConsideringIndexReg {
            get => Is16bitIndexAccess ? (ushort)X : (ushort)(X & 0xff);
            set => X = Is16bitIndexAccess ? value : (ushort)(value & 0xff);
        }
        /// <summary>
        /// Index Registerのビット幅を考慮して入出力します
        /// </summary>
        /// <value></value>
        public ushort YConsideringIndexReg {
            get => Is16bitIndexAccess ? (ushort)Y : (ushort)(Y & 0xff);
            set => Y = Is16bitIndexAccess ? value : (ushort)(value & 0xff);
        }

        public void Reset() {
            A = 0;
            X = 0;
            Y = 0;
            SP = 0x1fFF; // TODO 値の精査
            DB = 0;
            DP = 0;
            PB = 0;
            P.Reset();
            PC = 0;
        }

        /// <summary>
        /// 指定された値をStackにPushする
        /// </summary>
        /// <param name="bus"></param>
        /// <param name="data"></param>
        public void PushToStack(IBusAccessible bus, byte data) {
            Debug.Assert(this.SP > 0);

            bus.Write8(this.SP, data);
            this.SP--;
        }

        /// <summary>
        /// Stackから値を取り出します
        /// </summary>
        /// <param name="bus"></param>
        /// <returns></returns>
        public byte PopFromStack(IBusAccessible bus) {
            // SPは常に次に書き込める位置を指しているので、先に戻す
            checked {
                this.SP++;
            }
            return bus.Read8(this.SP);
        }

        /// <summary>
        /// 割り込みを発生させます
        /// </summary>
        /// <param name="bus"></param>
        /// <param name="irq"></param>
        public void InvokeInterrupt(IBusAccessible bus, Interrupt irq) {
            // Native ModeのReset Vectorは使わない(起動時はEmulation modeになるはず)
            Debug.Assert((irq != Interrupt.Reset) || (irq == Interrupt.Reset && P.HasFlag(ProcessorStatusFlag.E)));
            // RESET/NMI以外は割り込み許可レジスタを見に行く
            if (P.HasFlag(ProcessorStatusFlag.I) && (irq != Interrupt.Reset || irq != Interrupt.NonMaskable)) {
                return;
            }
            // 割り込みの種類によっては必要な値をStackに退避する
            switch (irq) {
                case Interrupt.CoProcessor: {
                        // TODO: 一通り実装する
                        break;
                    }
                default:
                    throw new ArgumentException($"存在しない割り込み種類が使用されました {irq}");
            }
            // lower addrを取得する、
            // ROM上の配置はLoROM, HiROMで変わるがメモリマップ上はBANK0末端のはず
            ushort lowAddr = irq switch
            {
                Interrupt.CoProcessor => 0x0fe4,
                Interrupt.Abort => 0x0fe6,
                Interrupt.NonMaskable => 0x0fe8,
                Interrupt.Reset => 0x0fea,
                Interrupt.OtherIrq => 0x0fec,
                _ => throw new ArgumentException($"存在しないか定義されていない割り込み種類です {irq}"),
            };
            // 取得したPCに飛ばす
            var dstAddr = bus.Read16(lowAddr);
            this.PC = dstAddr;
        }
    }
}