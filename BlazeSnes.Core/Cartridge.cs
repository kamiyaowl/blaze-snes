using System;
using System.IO;

namespace BlazeSnes.Core {
    /// <summary>
    /// .smcファイルフォーマットを解釈した内容です
    /// </summary>
    public class Cartridge {
        public static readonly uint LOROM_OFFSET = 0x7fb0;
        public static readonly uint HIROM_OFFSET = 0xffb0;
        public static readonly uint EXTRA_HEADER_SIZE = 512;
        public static readonly int HEADER_SIZE = 0x30;

        /// <summary>
        /// $00:xFB0 ~ $00:FFDF までの情報を格納します
        /// </summary>
        protected byte[] romRegistrationData;

        public ushort MakerCode => (ushort)(romRegistrationData[0x0] | (romRegistrationData[0x0 + 1] << 8));
        public UInt32 GameCode => (UInt32)(romRegistrationData[0x2] | (romRegistrationData[0x2 + 1] << 8) | (romRegistrationData[0x2 + 2] << 16) | (romRegistrationData[0x2 + 3] << 24));
        public byte ExpansionRamSize => romRegistrationData[0xd];
        public byte SpecialVersion => romRegistrationData[0xe];
        public byte CartridgeTypeSub => romRegistrationData[0xf];
        public string GameTitle => System.Text.Encoding.ASCII.GetString(romRegistrationData[0x10..(0x10 + 21)]);
        public byte MapMode => romRegistrationData[0x25];
        public byte CartridgeType => romRegistrationData[0x26];
        public byte RomSize => romRegistrationData[0x27];
        public byte RamSize => romRegistrationData[0x28];
        public byte DestinationCode => romRegistrationData[0x29];
        public byte MaskRomVersion => romRegistrationData[0x2b];
        public ushort CheckSumComplement => (ushort)((romRegistrationData[0x2c]) | (romRegistrationData[0x2d] << 8));
        public ushort CheckSum => (ushort)((romRegistrationData[0x2e]) | (romRegistrationData[0x2f] << 8));

        /// <summary>
        /// $00:xFE0 ~ $00:xFFF までの情報を格納します
        /// </summary>
        protected byte[] interruptVectorData;

        public ushort CopAddrInNative => (ushort)((interruptVectorData[0x4]) | (interruptVectorData[0x5] << 8));
        public ushort BreakAddrInNative => (ushort)((interruptVectorData[0x6]) | (interruptVectorData[0x7] << 8));
        public ushort AbortAddrInNative => (ushort)((interruptVectorData[0x8]) | (interruptVectorData[0x9] << 8));
        public ushort NmiAddrInNative => (ushort)((interruptVectorData[0xa]) | (interruptVectorData[0xb] << 8));
        public ushort ResetAddrInNative => (ushort)((interruptVectorData[0xc]) | (interruptVectorData[0xd] << 8));
        public ushort IrqAddrInNative => (ushort)((interruptVectorData[0xe]) | (interruptVectorData[0xf] << 8));
        public ushort CopAddrInEmulation => (ushort)((interruptVectorData[0x14]) | (interruptVectorData[0x15] << 8));
        public ushort BreakAddrInEmulation => (ushort)((interruptVectorData[0x16]) | (interruptVectorData[0x17] << 8));
        public ushort AbortAddrInEmulation => (ushort)((interruptVectorData[0x18]) | (interruptVectorData[0x19] << 8));
        public ushort NmiAddrInEmulation => (ushort)((interruptVectorData[0x1a]) | (interruptVectorData[0x1b] << 8));
        public ushort ResetAddrInEmulation => (ushort)((interruptVectorData[0x1c]) | (interruptVectorData[0x1d] << 8));
        public ushort IrqAddrInEmulation => (ushort)((interruptVectorData[0x1e]) | (interruptVectorData[0x1f] << 8));

        /// <summary>
        /// ROMの種類がLoRomならtrue
        /// </summary>
        public bool IsLoRom { get; internal set; } = false;
        /// <summary>
        /// ROMバイナリの先頭512byteにHeaderを持っている場合はtrue
        /// </summary>
        public bool HasHeaderOffset { get; set; } = false;

        /// <summary>
        /// readerに指定されたファイルを読み込み、Cartridgeの情報として展開します
        /// </summary>
        /// <param name="fs">対象ファイルのストリーム</param>
        public Cartridge(FileStream fs) {
            using (var br = new BinaryReader(fs)) {
                // 後で使う情報
                var isSuccess = parseRomRegistrationHeader(br, out var isLoRom, out var hasHeaderOffset);
                // 全パターンでだめだったらしい
                if (!isSuccess) {
                    throw new FileLoadException("Format Error");
                }
                this.IsLoRom = isLoRom;
                this.HasHeaderOffset = hasHeaderOffset;
                // 正常に読み込めたなら、残りのデータをローカルに展開
                var baseOffset = (HasHeaderOffset ? EXTRA_HEADER_SIZE : 0);
                // 割り込みベクタ
                this.interruptVectorData = new byte[0x20];
                var vectorTableBaseAddr = baseOffset + 0x30 + (IsLoRom ? LOROM_OFFSET : HIROM_OFFSET);
                if (br.BaseStream.Seek(vectorTableBaseAddr, SeekOrigin.Begin) != vectorTableBaseAddr) {
                    throw new FileLoadException("Interrupt Vectors Seek Error");
                }
                if (br.Read(this.interruptVectorData, 0, this.interruptVectorData.Length) != this.interruptVectorData.Length) {
                    throw new FileLoadException("Interrupt Vectors Read Error");
                }

                // TODO: 残りのデータ
            }
        }

        /// <summary>
        /// ROM情報を読み取って展開します
        /// </summary>
        /// <param name="br">読み取り対象のストリーム</param>
        /// <param name="isLoRom">LoROMならtrue</param>
        /// <param name="hasHeaderOffset">先頭にヘッダが付与されていればtrue</param>
        /// <returns>読み取り成功ならtrue</returns>
        protected bool parseRomRegistrationHeader(BinaryReader br, out bool isLoRom, out bool hasHeaderOffset) {
            // SNES ROM Headerの検査
            this.romRegistrationData = new byte[HEADER_SIZE];

            // RomType {LoROM, HiROM] x SMC Header{Exist, None} で4パターン試す必要がある
            var tryOffsetConfigs = new[] {
                    new { IsLoRom = false, HasHeaderOffset = false, },
                    new { IsLoRom = true, HasHeaderOffset = false, },
                    new { IsLoRom = false, HasHeaderOffset = true, },
                    new { IsLoRom = true, HasHeaderOffset = true, },
                };

            foreach (var offsetConfig in tryOffsetConfigs) {
                // 設定値通りにオフセットを導出
                var offset = (offsetConfig.IsLoRom ? LOROM_OFFSET : HIROM_OFFSET) +
                    (offsetConfig.HasHeaderOffset ? EXTRA_HEADER_SIZE : 0);

                // 読みたい位置までシークする
                if (br.BaseStream.Seek(offset, SeekOrigin.Begin) != offset) {
                    // 規定位置までSeekできてなければファイルサイズが小さいので違う
                    continue;
                }
                // 読み出す
                if (br.Read(this.romRegistrationData, 0, this.romRegistrationData.Length) != HEADER_SIZE) {
                    // 規定量Readできてない
                    continue;
                }
                // CheckSum見とく
                if (this.CheckSum != (ushort)(~this.CheckSumComplement)) {
                    continue;
                }
                // やったね
                isLoRom = offsetConfig.IsLoRom;
                hasHeaderOffset = offsetConfig.HasHeaderOffset;
                return true;
            }

            // 全部ダメだった
            isLoRom = false;
            hasHeaderOffset = false;
            return false;
        }
    }
}