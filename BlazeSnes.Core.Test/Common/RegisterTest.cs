using System;
using System.IO;
using System.Linq;

using BlazeSnes.Core.Common;
using BlazeSnes.Core.Cpu;

using Xunit;
using Xunit.Sdk;

namespace BlazeSnes.Core.Test.Common {
    public class RegisterTest {
        class SampleRegister : Register<byte> { }

        /// <summary>
        /// 書いた値がそのまま読み出せるか確認
        /// </summary>
        /// <param name="data"></param>
        [Theory, InlineData(0x00), InlineData(0xff), InlineData(0xa5)]
        public void WriteRead(byte data) {
            var target = new SampleRegister();
            target.Value = data;
            Assert.Equal(data, target.Value);
        }
    }
}