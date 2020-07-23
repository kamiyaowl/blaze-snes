using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace BlazeSnes.Core.Common {
    /// <summary>
    /// 便利メソッド群です。乱用禁止
    /// </summary>
    public static class Extension {
        /// <summary>
        /// 一旦BinaryStreamに起こしてから復元します
        /// </summary>
        /// <param name="src"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T DeepClone<T>(this T src) {
            using (var ms = new MemoryStream()) {
                // serialize
                var b = new BinaryFormatter();
                b.Serialize(ms, src);
                // deserialize
                ms.Seek(0, SeekOrigin.Begin);
                 return (T)b.Deserialize(ms);
            }
        }
    }
}