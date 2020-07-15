using System;
using System.Diagnostics;
using System.IO;

namespace BlazeSnes.Core.Common {
    /// <summary>
    /// 電源Off/Onを用いた完全なリセットする機能を実装します(!= RESET割り込み)
    /// </summary>
    public interface IResetable {
        /// <summary>
        /// 内部変数を初期化します
        /// </summary>
        void Reset();
    }
}