using System;
using System.IO;

using BlazeSnes.Core.Common;
using BlazeSnes.Core.External;

namespace BlazeSnes.Core.Bus {
    public class MemoryMap {
        public void Read(uint addr, byte[] data, bool isNondestructive = false) {
            throw new NotImplementedException();
        }

        public void Write(uint addr, byte[] data) {
            throw new NotImplementedException();
        }
    }
}