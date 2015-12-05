//
// DeviceInfo.cs
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

using System;

namespace Sharp98.VGM
{
    public static class DeviceTypeConverter
    {
        public static Sharp98.DeviceType FromVGMDevice(this DeviceType type)
        {
            switch (type)
            {
                case DeviceType.None: return Sharp98.DeviceType.None;
                case DeviceType.AY8910: return Sharp98.DeviceType.AY8910;
                case DeviceType.C140: return Sharp98.DeviceType.C140;
                case DeviceType.C352: return Sharp98.DeviceType.C352;
                case DeviceType.ES5503: return Sharp98.DeviceType.ES5503;
                case DeviceType.ES5506: return Sharp98.DeviceType.ES5506;
                case DeviceType.GameBoyDMG: return Sharp98.DeviceType.GameBoyDMG;
                case DeviceType.GA20: return Sharp98.DeviceType.GA20;
                case DeviceType.HuC6280: return Sharp98.DeviceType.HuC6280;
                case DeviceType.K051649: return Sharp98.DeviceType.K051649;
                case DeviceType.K053260: return Sharp98.DeviceType.K053260;
                case DeviceType.K054539: return Sharp98.DeviceType.K054539;
                case DeviceType.NES_APU: return Sharp98.DeviceType.NES_APU;
                case DeviceType.MultiPCM: return Sharp98.DeviceType.MultiPCM;
                case DeviceType.OKIM6258: return Sharp98.DeviceType.OKIM6258;
                case DeviceType.OKIM6295: return Sharp98.DeviceType.OKIM6295;
                case DeviceType.Pokey: return Sharp98.DeviceType.Pokey;
                case DeviceType.PWM: return Sharp98.DeviceType.PWM;
                case DeviceType.QSound: return Sharp98.DeviceType.QSound;
                case DeviceType.RF5C68: return Sharp98.DeviceType.RF5C68;
                case DeviceType.RF5C164: return Sharp98.DeviceType.RF5C164;
                case DeviceType.SAA1099: return Sharp98.DeviceType.SAA1099;
                case DeviceType.SegaPCM: return Sharp98.DeviceType.SegaPCM;
                case DeviceType.SCSP: return Sharp98.DeviceType.SCSP;
                case DeviceType.SN76489: return Sharp98.DeviceType.SN76489;
                case DeviceType.uPD7759: return Sharp98.DeviceType.uPD7759;
                case DeviceType.VSU: return Sharp98.DeviceType.VSU;
                case DeviceType.WonderSwan: return Sharp98.DeviceType.WonderSwan;
                case DeviceType.X1_010: return Sharp98.DeviceType.X1_010;
                case DeviceType.Y8950: return Sharp98.DeviceType.Y8950;
                case DeviceType.YM2149: return Sharp98.DeviceType.YM2149;
                case DeviceType.YM2151: return Sharp98.DeviceType.YM2151;
                case DeviceType.YM2203: return Sharp98.DeviceType.YM2203;
                case DeviceType.YM2413: return Sharp98.DeviceType.YM2413;
                case DeviceType.YM2608: return Sharp98.DeviceType.YM2608;
                case DeviceType.YM2610B: return Sharp98.DeviceType.YM2610B;
                case DeviceType.YM2612: return Sharp98.DeviceType.YM2612;
                case DeviceType.YM3526: return Sharp98.DeviceType.YM3526;
                case DeviceType.YM3812: return Sharp98.DeviceType.YM3812;
                case DeviceType.YMF262: return Sharp98.DeviceType.YMF262;
                case DeviceType.YMF271: return Sharp98.DeviceType.YMF271;
                case DeviceType.YMF278B: return Sharp98.DeviceType.YMF278B;
                case DeviceType.YMZ280B: return Sharp98.DeviceType.YMZ280B;

                default: throw new ArgumentOutOfRangeException(nameof(type));
            }
        }

        public static DeviceType ToVGMDevice(this Sharp98.DeviceType type)
        {
            switch (type)
            {
                case Sharp98.DeviceType.None: return DeviceType.None;
                case Sharp98.DeviceType.AY8910: return DeviceType.AY8910;
                case Sharp98.DeviceType.C140: return DeviceType.C140;
                case Sharp98.DeviceType.C352: return DeviceType.C352;
                case Sharp98.DeviceType.ES5503: return DeviceType.ES5503;
                case Sharp98.DeviceType.ES5506: return DeviceType.ES5506;
                case Sharp98.DeviceType.GameBoyDMG: return DeviceType.GameBoyDMG;
                case Sharp98.DeviceType.GA20: return DeviceType.GA20;
                case Sharp98.DeviceType.HuC6280: return DeviceType.HuC6280;
                case Sharp98.DeviceType.K051649: return DeviceType.K051649;
                case Sharp98.DeviceType.K053260: return DeviceType.K053260;
                case Sharp98.DeviceType.K054539: return DeviceType.K054539;
                case Sharp98.DeviceType.NES_APU: return DeviceType.NES_APU;
                case Sharp98.DeviceType.MultiPCM: return DeviceType.MultiPCM;
                case Sharp98.DeviceType.OKIM6258: return DeviceType.OKIM6258;
                case Sharp98.DeviceType.OKIM6295: return DeviceType.OKIM6295;
                case Sharp98.DeviceType.Pokey: return DeviceType.Pokey;
                case Sharp98.DeviceType.PWM: return DeviceType.PWM;
                case Sharp98.DeviceType.QSound: return DeviceType.QSound;
                case Sharp98.DeviceType.RF5C68: return DeviceType.RF5C68;
                case Sharp98.DeviceType.RF5C164: return DeviceType.RF5C164;
                case Sharp98.DeviceType.SAA1099: return DeviceType.SAA1099;
                case Sharp98.DeviceType.SegaPCM: return DeviceType.SegaPCM;
                case Sharp98.DeviceType.SCSP: return DeviceType.SCSP;
                case Sharp98.DeviceType.SN76489: return DeviceType.SN76489;
                case Sharp98.DeviceType.uPD7759: return DeviceType.uPD7759;
                case Sharp98.DeviceType.VSU: return DeviceType.VSU;
                case Sharp98.DeviceType.WonderSwan: return DeviceType.WonderSwan;
                case Sharp98.DeviceType.X1_010: return DeviceType.X1_010;
                case Sharp98.DeviceType.Y8950: return DeviceType.Y8950;
                case Sharp98.DeviceType.YM2149: return DeviceType.YM2149;
                case Sharp98.DeviceType.YM2151: return DeviceType.YM2151;
                case Sharp98.DeviceType.YM2203: return DeviceType.YM2203;
                case Sharp98.DeviceType.YM2413: return DeviceType.YM2413;
                case Sharp98.DeviceType.YM2608: return DeviceType.YM2608;
                case Sharp98.DeviceType.YM2610B: return DeviceType.YM2610B;
                case Sharp98.DeviceType.YM2612: return DeviceType.YM2612;
                case Sharp98.DeviceType.YM3526: return DeviceType.YM3526;
                case Sharp98.DeviceType.YM3812: return DeviceType.YM3812;
                case Sharp98.DeviceType.YMF262: return DeviceType.YMF262;
                case Sharp98.DeviceType.YMF271: return DeviceType.YMF271;
                case Sharp98.DeviceType.YMF278B: return DeviceType.YMF278B;
                case Sharp98.DeviceType.YMZ280B: return DeviceType.YMZ280B;

                default: throw new ArgumentOutOfRangeException(nameof(type));
            }
        }
    }
}
