//
// DeviceType.cs
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

namespace Sharp98.S98
{
    /// <summary>
    /// 対象デバイスを識別するための列挙体です。
    /// </summary>
    /// <remarks>
    /// <see cref="DeviceType"/> 列挙体は使用されるデバイスを指定する際に、デバイスのタイプを識別するために使われます。
    /// 原典: http://www.vesta.dti.ne.jp/~tsato/soft_s98v3.html
    /// </remarks>
    public enum DeviceType : uint
    {
        /// <summary>
        /// 未使用。対象データは無視されます。
        /// </summary>
        None = 0,

        // --- ---

        /// <summary>
        /// YM2149 (PSG)。
        /// </summary>
        YM2149 = 1,

        /// <summary>
        /// YM2203 (OPN)。
        /// </summary>
        YM2203 = 2,

        /// <summary>
        /// YM2612 (OPN2)。
        /// </summary>
        YM2612 = 3,

        /// <summary>
        /// YM2608 (OPNA)。
        /// </summary>
        YM2608 = 4,

        /// <summary>
        /// YM2151 (OPM)。
        /// </summary>
        YM2151 = 5,

        /// <summary>
        /// YM2413 (OPLL)。
        /// </summary>
        YM2413 = 6,

        /// <summary>
        /// YM3526 (OPL)。
        /// </summary>
        YM3526 = 7,

        /// <summary>
        /// YM3812 (OPL2)。
        /// </summary>
        YM3812 = 8,

        /// <summary>
        /// YMF262 (OPL3)。
        /// </summary>
        YMF262 = 9,

        /// <summary>
        /// AY-3-8910 (PSG)。
        /// </summary>
        AY_3_8910 = 15,

        /// <summary>
        /// SN76489 (DCSG)。
        /// </summary>
        SN76489 = 16,

        // --- ---
        
        /// <summary>
        /// OPN (YM2203)。
        /// </summary>
        OPN = YM2203,

        /// <summary>
        /// OPN2 (YM2612)。
        /// </summary>
        OPN2 = YM2612,

        /// <summary>
        /// OPNA (YM2608)。
        /// </summary>
        OPNA = YM2608,

        /// <summary>
        /// OPM (YM2151)。
        /// </summary>
        OPM = YM2151,

        /// <summary>
        /// OPLL (YM2413)。
        /// </summary>
        OPLL = YM2413,

        /// <summary>
        /// OPL (YM3526)。
        /// </summary>
        OPL = YM3526,

        /// <summary>
        /// OPL2 (YM3812)。
        /// </summary>
        OPL2 = YM3812,

        /// <summary>
        /// OPL3 (YMF262)。
        /// </summary>
        OPL3 = YMF262,

        /// <summary>
        /// DCSG (SN76489)。
        /// </summary>
        DCSG = SN76489,
    }
}
