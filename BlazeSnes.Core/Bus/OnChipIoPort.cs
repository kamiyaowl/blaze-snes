using System;
using System.Diagnostics;
using System.IO;

using BlazeSnes.Core.Common;
using BlazeSnes.Core.External;

namespace BlazeSnes.Core.Bus {
    public class OnChipIoPort : IBusAccessible {
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