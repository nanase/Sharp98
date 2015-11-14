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

namespace Sharp98
{
    public struct DeviceInfo
    {
        #region -- Public Fields --

        public static readonly DeviceInfo DefaultDevice = new DeviceInfo(DeviceType.OPNA, 7987200, PanFlag.Stereo);

        #endregion

        #region -- Private Info --

        private readonly DeviceType type;
        private readonly uint clock;
        private readonly PanFlag pan;

        #endregion

        #region -- Public Properties --

        public DeviceType Type { get { return this.type; } }

        public uint Clock { get { return this.clock; } }

        public PanFlag Pan { get { return this.pan; } }

        #endregion

        #region -- Constructors --

        public DeviceInfo(DeviceType type, uint clock, PanFlag pan)
        {
            this.type = type;
            this.clock = clock;
            this.pan = pan;
        }

        #endregion

        #region -- Public Methods --

        public byte[] Export()
        {
            var output = new byte[16];

            Array.Copy(((uint)this.Type).GetLEByte(), 0, output, 0, 4);
            Array.Copy(this.Clock.GetLEByte(), 0, output, 4, 4);
            Array.Copy(((uint)this.Pan).GetLEByte(), 0, output, 8, 4);

            return output;
        }

        #endregion

        #region -- Public Static Methods --

        public static DeviceInfo Import(byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            if (buffer.Length != 16)
                throw new ArgumentOutOfRangeException(nameof(buffer));

            return new DeviceInfo(
                (DeviceType)buffer.GetLEUInt32(0),
                buffer.GetLEUInt32(4),
                (PanFlag)buffer.GetLEUInt32(8));
        }

        #endregion
    }
}
