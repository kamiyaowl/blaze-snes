using System;
using System.Diagnostics;
using System.IO;

using BlazeSnes.Core.Common;
using BlazeSnes.Core.External;

namespace BlazeSnes.Core.Bus {
    public class ApuControlReg : IBusAccessible {
        public bool Read(uint addr, byte[] data, bool isNondestructive = false) {
            // TODO: 実装する
            throw new NotImplementedException();
        }

        public bool Write(uint addr, in byte[] data) {
            // TODO: 実装する
            throw new NotImplementedException();
        }
    }
}