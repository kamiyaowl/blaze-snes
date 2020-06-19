using System;
using System.IO;

namespace BlazeSnes.Core {

    /// <summary>
    /// CartridgeのROMの種類を示します
    /// </summary>
    public enum RomType {
        ROM, RAM, SRAM, DSP1, FX
    }

    /// <summary>
    /// 
    /// </summary>
    public class Cartridge {
        public string GameTitle { get; internal set; }
        public byte MakeupByte { get; internal set; }
        public RomType RomType { get; internal set; }
        public byte RomSize { get; internal set; }
        public byte SramSize { get; internal set; }
        public byte CreatorLicenseId { get; internal set; }
        public byte Version { get; internal set; }
        public ushort CheckSumComplement { get; internal set; }
        public ushort CheckSum { get; internal set; }

        /// <summary>
        /// readerに指定されたファイルを読み込み、Cartridgeの情報として展開します
        /// </summary>
        /// <param name="fs">対象ファイルのストリーム</param>
        public Cartridge(FileStream fs) {
            using(var br = new BinaryReader(fs)) {

            }
        }
    }
}