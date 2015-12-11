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

namespace Sharp98.VGM
{
    public enum DeviceType
    {
        None,

        // --- ---

        AY8910,
        C140,
        C352,
        ES5503,
        ES5506,
        GameBoyDMG,
        GA20,
        HuC6280,
        K051649,
        K053260,
        K054539,
        NES_APU,
        MultiPCM,
        OKIM6258,
        OKIM6295,
        Pokey,
        PWM,
        QSound,
        RF5C68,
        RF5C164,
        SAA1099,
        SegaPCM,
        SCSP,
        SN76489,
        uPD7759,
        VSU,
        WonderSwan,
        X1_010,
        Y8950,
        YM2149,
        YM2151,
        YM2203,
        YM2413,
        YM2608,
        YM2610B,
        YM2612,
        YM3526,
        YM3812,
        YMF262,
        YMF271,
        YMF278B,
        YMZ280B,

        // --- Aliases ---

        AY_3_8910 = AY8910,
        DCSG = SN76489,
        MSX_AUDIO = Y8950,
        OPN = YM2203,
        OPN2 = YM2612,
        OPNA = YM2608,
        OPNB = YM2610B,
        OPM = YM2151,
        OPLL = YM2413,
        OPL = YM3526,
        OPL2 = YM3812,
        OPL3 = YMF262,
        SCC1 = K051649,
    }
}
