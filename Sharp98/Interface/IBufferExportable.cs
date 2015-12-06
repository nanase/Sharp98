//
// IBufferExportable.cs
//
// Author:
//       Tomona Nanase <nanase@users.noreply.github.com>
//
// The MIT License (MIT)
//
// Copyright (c) 2015 Tomona Nanase
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System.Text;

namespace Sharp98
{
    /// <summary>
    /// オブジェクトを、バッファを表す <see cref="byte"/> 型の配列にエクスポートするためのインタフェースです。
    /// </summary>
    public interface IBufferExportable
    {
        #region -- Methods --
        
        /// <summary>
        /// オブジェクトをバッファにエクスポートし、そのバッファを返します。。
        /// </summary>
        /// <param name="encoding">文字列のエンコード。実装クラスによっては使用されません。</param>
        /// <returns>書き込まれたバッファ。</returns>
        byte[] Export(Encoding encoding);

        /// <summary>
        /// オブジェクトを指定されたバッファにエクスポートします。
        /// </summary>
        /// <param name="buffer">エクスポート先のバッファ。</param>
        /// <param name="index">書き込みが開始されるバッファのインデクス。</param>
        /// <param name="length">書き込みが許容される最大の長さ。</param>
        /// <param name="encoding">文字列のエンコード。実装クラスによっては使用されません。</param>
        /// <returns>書き込まれた配列長。</returns>
        int Export(byte[] buffer, int index, int length, Encoding encoding);

        #endregion
    }
}
