using System;
using System.IO;

namespace BlazeSnes.Core {
    /// <summary>
    /// .smcファイルフォーマットを解釈した内容です
    /// </summary>
    public class Cartridge {
        /// <summary>
        /// $00:FFB0 ~ $00FFFF までの情報を格納します
        /// </summary>
        protected byte[] romRegistrationData;

        /// <summary>
        /// $00:xFB0 2byte
        /// </summary>
        /// <value></value>
        public UInt16 MakerCode => (UInt16)(romRegistrationData[0x0] | (romRegistrationData[0x0 + 1] << 8));
        /// <summary>
        /// $00:xFB2 4byte
        /// </summary>
        /// <value></value>
        public UInt32 GameCode => (UInt32)(romRegistrationData[0x2] | (romRegistrationData[0x2 + 1] << 8)| (romRegistrationData[0x2 + 2] << 16) | (romRegistrationData[0x2 + 3] << 24));
        /// <summary>
        /// $00:xFBD 1byte
        /// </summary>
        /// <value></value>
        public byte ExpansionRamSize => romRegistrationData[0xd];
        /// <summary>
        /// $00:xFBE 1byte
        /// </summary>
        /// <value></value>
        public byte SpecialVersion => romRegistrationData[0xe];
        /// <summary>
        /// $00:xFBF 1byte
        /// </summary>
        /// <value></value>
        public byte CartridgeTypeSub => romRegistrationData[0xf];
        /// <summary>
        /// $00:xFFC0 21byte
        /// </summary>
        /// <value></value>
        public string GameTitle => System.Text.Encoding.ASCII.GetString(romRegistrationData[0x10..(0x10+21)]);
        /// <summary>
        /// $00:xFD5 1byte
        /// </summary>
        /// <value></value>
        public byte MapMode => romRegistrationData[0x25];
        /// <summary>
        /// $00:xFD6 1byte
        /// </summary>
        /// <value></value>
        public byte CartridgeType => romRegistrationData[0x26];
        /// <summary>
        /// $00:xFD7 1byte
        /// </summary>
        /// <value></value>
        public byte RomSize => romRegistrationData[0x27];
        /// <summary>
        /// $00:xFD8 1byte
        /// </summary>
        /// <value></value>
        public byte RamSize => romRegistrationData[0x28];
                /// <summary>
        /// $00:xFD9 1byte
        /// </summary>
        /// <value></value>
        public byte DestinationCode => romRegistrationData[0x29];
        /// <summary>
        /// $00:xFDB 1byte
        /// </summary>
        /// <value></value>
        public byte MaskRomVersion => romRegistrationData[0x2b];
        /// <summary>
        /// $00:xFDC 2byte
        /// </summary>
        /// <value></value>
        public ushort CheckSumComplement => (ushort)((romRegistrationData[0x2c]) | (romRegistrationData[0x2d] << 8));
        /// <summary>
        /// $00:xFDE 2byte
        /// </summary>
        /// <value></value>
        public ushort CheckSum => (ushort)((romRegistrationData[0x2e]) | (romRegistrationData[0x2f] << 8));

        /// <summary>
        /// readerに指定されたファイルを読み込み、Cartridgeの情報として展開します
        /// </summary>
        /// <param name="fs">対象ファイルのストリーム</param>
        public Cartridge (FileStream fs) {
            using (var br = new BinaryReader (fs)) {
                // SNES ROM Headerの検査
                // RomType {LoROM, HiROM] x SMC Header{Exist, None} で4パターン試す必要がある
                const uint LOROM_OFFSET = 0x7fb0;
                const uint HIROM_OFFSET = 0xffb0;
                const uint EXTRA_HEADER_SIZE = 512; // よくわからんが512byte先頭にinfoつけているbinaryがあるらしい
                const int HEADER_SIZE = 0x30;

                var tryOffsets = new []{
                    LOROM_OFFSET,
                    HIROM_OFFSET,
                    LOROM_OFFSET + EXTRA_HEADER_SIZE,
                    HIROM_OFFSET + EXTRA_HEADER_SIZE,
                };
                this.romRegistrationData = new byte[HEADER_SIZE];

                bool isSuccess = false;
                foreach(var offset in tryOffsets) {
                    var currentPos = br.BaseStream.Seek(offset, SeekOrigin.Begin);
                    // 規定位置までSeekできてなければファイルサイズが小さいので違う
                    if (currentPos != offset) continue;

                    var readSize = br.Read(this.romRegistrationData, 0, HEADER_SIZE);
                    // 規定量Readできてない
                    if (readSize != HEADER_SIZE) continue;

                    // CheckSum見とく
                    if (this.CheckSum != (ushort)(~this.CheckSumComplement)) continue;

                    // やったね
                    isSuccess = true;
                }
                // 全パターンでだめだったらしい
                if (!isSuccess) {
                    throw new FileLoadException("Format Error");
                }

                // TODO: 正常に読み込めたなら、残りのデータをローカルに展開
            }
        }
    }
}