using System;
using System.IO;

namespace BlazeSnes.Core.Common {
    /// <summary>
    /// 特定のレジスタを示します
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Register<T> where T : struct {
        /// <summary>
        /// 保持している生の値、継承ご追加するメソッドはこのデータに付随した操作を実装すること
        /// </summary>
        protected T value;
        /// <summary>
        /// accessorは上書き可
        /// </summary>
        /// <value></value>
        public virtual T Value {
            get => this.value;
            set => this.value = value;
        }

        public override string ToString() => $"Reg(${value:04x})";
    }
}