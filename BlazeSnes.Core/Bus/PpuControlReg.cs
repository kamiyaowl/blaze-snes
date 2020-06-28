using System;
using System.Diagnostics;
using System.IO;

using BlazeSnes.Core.Common;
using BlazeSnes.Core.External;

namespace BlazeSnes.Core.Bus {
    /// <summary>
    /// PPU制御レジスタ
    /// 
    /// Write Only
    ///  2100h - INIDISP - Display Control 1                                  8xh
    ///  2101h - OBSEL   - Object Size and Object Base                        (?)
    ///  2102h - OAMADDL - OAM Address (lower 8bit)                           (?)
    ///  2103h - OAMADDH - OAM Address (upper 1bit) and Priority Rotation     (?)
    ///  2104h - OAMDATA - OAM Data Write (write-twice)                       (?)
    ///  2105h - BGMODE  - BG Mode and BG Character Size                      (xFh)
    ///  2106h - MOSAIC  - Mosaic Size and Mosaic Enable                      (?)
    ///  2107h - BG1SC   - BG1 Screen Base and Screen Size                    (?)
    ///  2108h - BG2SC   - BG2 Screen Base and Screen Size                    (?)
    ///  2109h - BG3SC   - BG3 Screen Base and Screen Size                    (?)
    ///  210Ah - BG4SC   - BG4 Screen Base and Screen Size                    (?)
    ///  210Bh - BG12NBA - BG Character Data Area Designation                 (?)
    ///  210Ch - BG34NBA - BG Character Data Area Designation                 (?)
    ///  210Dh - BG1HOFS - BG1 Horizontal Scroll (X) (write-twice) / M7HOFS   (?,?)
    ///  210Eh - BG1VOFS - BG1 Vertical Scroll (Y)   (write-twice) / M7VOFS   (?,?)
    ///  210Fh - BG2HOFS - BG2 Horizontal Scroll (X) (write-twice)            (?,?)
    ///  2110h - BG2VOFS - BG2 Vertical Scroll (Y)   (write-twice)            (?,?)
    ///  2111h - BG3HOFS - BG3 Horizontal Scroll (X) (write-twice)            (?,?)
    ///  2112h - BG3VOFS - BG3 Vertical Scroll (Y)   (write-twice)            (?,?)
    ///  2113h - BG4HOFS - BG4 Horizontal Scroll (X) (write-twice)            (?,?)
    ///  2114h - BG4VOFS - BG4 Vertical Scroll (Y)   (write-twice)            (?,?)
    ///  2115h - VMAIN   - VRAM Address Increment Mode                        (?Fh)
    ///  2116h - VMADDL  - VRAM Address (lower 8bit)                          (?)
    ///  2117h - VMADDH  - VRAM Address (upper 8bit)                          (?)
    ///  2118h - VMDATAL - VRAM Data Write (lower 8bit)                       (?)
    ///  2119h - VMDATAH - VRAM Data Write (upper 8bit)                       (?)
    ///  211Ah - M7SEL   - Rotation/Scaling Mode Settings                     (?)
    ///  211Bh - M7A     - Rotation/Scaling Parameter A & Maths 16bit operand(FFh)(w2)
    ///  211Ch - M7B     - Rotation/Scaling Parameter B & Maths 8bit operand (FFh)(w2)
    ///  211Dh - M7C     - Rotation/Scaling Parameter C         (write-twice) (?)
    ///  211Eh - M7D     - Rotation/Scaling Parameter D         (write-twice) (?)
    ///  211Fh - M7X     - Rotation/Scaling Center Coordinate X (write-twice) (?)
    ///  2120h - M7Y     - Rotation/Scaling Center Coordinate Y (write-twice) (?)
    ///  2121h - CGADD   - Palette CGRAM Address                              (?)
    ///  2122h - CGDATA  - Palette CGRAM Data Write             (write-twice) (?)
    ///  2123h - W12SEL  - Window BG1/BG2 Mask Settings                       (?)
    ///  2124h - W34SEL  - Window BG3/BG4 Mask Settings                       (?)
    ///  2125h - WOBJSEL - Window OBJ/MATH Mask Settings                      (?)
    ///  2126h - WH0     - Window 1 Left Position (X1)                        (?)
    ///  2127h - WH1     - Window 1 Right Position (X2)                       (?)
    ///  2128h - WH2     - Window 2 Left Position (X1)                        (?)
    ///  2129h - WH3     - Window 2 Right Position (X2)                       (?)
    ///  212Ah - WBGLOG  - Window 1/2 Mask Logic (BG1-BG4)                    (?)
    ///  212Bh - WOBJLOG - Window 1/2 Mask Logic (OBJ/MATH)                   (?)
    ///  212Ch - TM      - Main Screen Designation                            (?)
    ///  212Dh - TS      - Sub Screen Designation                             (?)
    ///  212Eh - TMW     - Window Area Main Screen Disable                    (?)
    ///  212Fh - TSW     - Window Area Sub Screen Disable                     (?)
    ///  2130h - CGWSEL  - Color Math Control Register A                      (?)
    ///  2131h - CGADSUB - Color Math Control Register B                      (?)
    ///  2132h - COLDATA - Color Math Sub Screen Backdrop Color               (?)
    ///  2133h - SETINI  - Display Control 2                                  00h?
    /// 
    /// Read Only
    ///  2134h - MPYL    - PPU1 Signed Multiply Result   (lower 8bit)         (01h)
    ///  2135h - MPYM    - PPU1 Signed Multiply Result   (middle 8bit)        (00h)
    ///  2136h - MPYH    - PPU1 Signed Multiply Result   (upper 8bit)         (00h)
    ///  2137h - SLHV    - PPU1 Latch H/V-Counter by Software (Read=Strobe)
    ///  2138h - RDOAM   - PPU1 OAM Data Read            (read-twice)
    ///  2139h - RDVRAML - PPU1 VRAM Data Read           (lower 8bits)
    ///  213Ah - RDVRAMH - PPU1 VRAM Data Read           (upper 8bits)
    ///  213Bh - RDCGRAM - PPU2 CGRAM Data Read (Palette)(read-twice)
    ///  213Ch - OPHCT   - PPU2 Horizontal Counter Latch (read-twice)         (01FFh)
    ///  213Dh - OPVCT   - PPU2 Vertical Counter Latch   (read-twice)         (01FFh)
    ///  213Eh - STAT77  - PPU1 Status and PPU1 Version Number
    ///  213Fh - STAT78  - PPU2 Status and PPU2 Version Number                Bit7=0
    /// </summary>
    public class PpuControlReg : IBusAccessible {
        public void Read(BusAccess access, uint addr, byte[] data, bool isNondestructive = false) {
            // TODO: 実装する
            throw new NotImplementedException();
        }

        public void Write(BusAccess access, uint addr, byte[] data) {
            // TODO: 実装する
            throw new NotImplementedException();
        }
    }
}