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

namespace Sharp98
{
    public enum DeviceType : uint
    {
        None = 0,

        //

        YM2149 = 1,

        YM2203 = 2,

        YM2612 = 3,

        YM2608 = 4,

        YM2151 = 5,

        YM2413 = 6,

        YM3526 = 7,

        YM3812 = 8,

        YMF262 = 9,

        AY_3_8910 = 15,

        SN76489 = 16,

        //
        
        OPN = YM2203,

        OPN2 = YM2612,

        OPNA = YM2608,

        OPM = YM2151,

        OPLL = YM2413,

        OPL = YM3526,

        OPL2 = YM3812,

        OPL3 = YMF262,

        DCSG = SN76489,
    }
}
