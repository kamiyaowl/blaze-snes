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
        /// <param name="reader">対象ファイルのストリーム</param>
        /// <param name="dst">読み出せた場合は、読みだしたカートリッジのインスタンス</param>
        /// <returns>正常に読み出せた場合はtrue</returns>
        public static bool TryParse(StreamReader reader, out Cartridge dst) {
            dst = null;
            // TODO: implement here
            return false;
        }
    }
}