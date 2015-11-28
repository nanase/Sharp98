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
            if (!Enum.IsDefined(typeof(DeviceType), type))
                throw new ArgumentOutOfRangeException(nameof(type));

            if (clock == 0)
                throw new ArgumentOutOfRangeException(nameof(clock));

            if (!Enum.IsDefined(typeof(PanFlag), pan))
                throw new ArgumentOutOfRangeException(nameof(pan));
            
            this.type = type;
            this.clock = clock;
            this.pan = pan;
        }

        #endregion

        #region -- Public Methods --

        public void Export(byte[] buffer, int index = 0)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            if (index < 0 || buffer.Length < index + 16)
                throw new ArgumentOutOfRangeException(nameof(index));

            ((uint)this.Type).GetLEByte(buffer, index);
            this.Clock.GetLEByte(buffer, index + 4);
            ((uint)this.Pan).GetLEByte(buffer, index + 8);
        }

        #endregion

        #region -- Public Static Methods --

        public static DeviceInfo Import(byte[] buffer, int index = 0)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            if (index < 0 || buffer.Length <= index + 16)
                throw new ArgumentOutOfRangeException(nameof(index));

            return new DeviceInfo(
                (DeviceType)buffer.GetLEUInt32(index + 0),
                buffer.GetLEUInt32(index + 4),
                (PanFlag)buffer.GetLEUInt32(index + 8));
        }

        #endregion
    }
}
