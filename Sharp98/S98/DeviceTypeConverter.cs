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

namespace Sharp98.S98
{
    public static class DeviceTypeConverter
    {
        public static Sharp98.DeviceType FromS98Device(this DeviceType type)
        {
            switch (type)
            {
                case DeviceType.None: return Sharp98.DeviceType.None;
                case DeviceType.YM2149: return Sharp98.DeviceType.YM2149;
                case DeviceType.YM2203: return Sharp98.DeviceType.YM2203;
                case DeviceType.YM2612: return Sharp98.DeviceType.YM2612;
                case DeviceType.YM2608: return Sharp98.DeviceType.YM2608;
                case DeviceType.YM2151: return Sharp98.DeviceType.YM2151;
                case DeviceType.YM2413: return Sharp98.DeviceType.YM2413;
                case DeviceType.YM3526: return Sharp98.DeviceType.YM3526;
                case DeviceType.YM3812: return Sharp98.DeviceType.YM3812;
                case DeviceType.YMF262: return Sharp98.DeviceType.YMF262;
                case DeviceType.AY_3_8910: return Sharp98.DeviceType.AY8910;
                case DeviceType.SN76489: return Sharp98.DeviceType.SN76489;

                default: throw new ArgumentOutOfRangeException(nameof(type));
            }
        }

        public static DeviceType ToS98Device(this Sharp98.DeviceType type)
        {
            switch (type)
            {
                case Sharp98.DeviceType.None: return DeviceType.None;
                case Sharp98.DeviceType.AY8910: return DeviceType.AY_3_8910;
                case Sharp98.DeviceType.SN76489: return DeviceType.SN76489;
                case Sharp98.DeviceType.YM2149: return DeviceType.YM2149;
                case Sharp98.DeviceType.YM2151: return DeviceType.YM2151;
                case Sharp98.DeviceType.YM2203: return DeviceType.YM2203;
                case Sharp98.DeviceType.YM2413: return DeviceType.YM2413;
                case Sharp98.DeviceType.YM2608: return DeviceType.YM2608;
                case Sharp98.DeviceType.YM2612: return DeviceType.YM2612;
                case Sharp98.DeviceType.YM3526: return DeviceType.YM3526;
                case Sharp98.DeviceType.YM3812: return DeviceType.YM3812;
                case Sharp98.DeviceType.YMF262: return DeviceType.YMF262;

                default: throw new ArgumentOutOfRangeException(nameof(type));
            }
        }
    }
}
