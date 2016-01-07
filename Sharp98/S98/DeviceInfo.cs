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
using System.IO;
using System.Text;

namespace Sharp98.S98
{
    public struct DeviceInfo : IDeviceInfo
    {
        #region -- Public Fields --

        public static readonly DeviceInfo DefaultDevice = new DeviceInfo(S98.DeviceType.OPNA, 7987200, PanFlag.Stereo);

        #endregion

        #region -- Private Info --

        private readonly DeviceType type;
        private readonly int clock;
        private readonly PanFlag pan;

        #endregion

        #region -- Public Properties --

        public DeviceType S98DeviceType => this.type;

        public Sharp98.DeviceType DeviceType => this.type.FromS98Device();

        public int Clock => this.clock;

        public PanFlag Pan => this.pan;

        #endregion

        #region -- Constructors --

        public DeviceInfo(DeviceType type, uint clock, PanFlag pan)
        {
            if (!Enum.IsDefined(typeof(DeviceType), type))
                throw new ArgumentOutOfRangeException(nameof(type));

            if (clock == 0 || clock > int.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(clock));
            
            if (!Enum.IsDefined(typeof(PanFlag), pan))
                throw new ArgumentOutOfRangeException(nameof(pan));

            this.type = type;
            this.clock = (int)clock;
            this.pan = pan;
        }

        #endregion

        #region -- Public Methods --

        public byte[] Export(Encoding encoding)
        {
            var buffer = new byte[16];
            this.ExportBuffer(buffer, 0);
            return buffer;
        }

        public int Export(byte[] buffer, int index, int length, Encoding encoding)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            if (index < 0 || buffer.Length < index + 16)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (length < 16)
                throw new ArgumentOutOfRangeException(nameof(length), "バッファの長さが足りません。少なくとも 16 の長さが必要です。");

            this.ExportBuffer(buffer, index);
            return 16;
        }

        public void Export(Stream stream, Encoding encoding)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (!stream.CanWrite)
                throw new InvalidOperationException("書き込みのできないストリームが指定されました.");

            var buffer = new byte[16];
            this.Export(buffer, 0, 16, null);
            stream.Write(buffer, 0, 16);
        }

        #endregion

        #region -- Private Methods --

        private void ExportBuffer(byte[] buffer, int index)
        {
            ((uint)this.S98DeviceType).GetLEByte(buffer, index);
            ((uint)this.Clock).GetLEByte(buffer, index + 4);
            ((uint)this.Pan).GetLEByte(buffer, index + 8);
            Array.Clear(buffer, index + 12, 4);
        }

        #endregion

        #region -- Public Static Methods --

        public static DeviceInfo Import(byte[] buffer, int index = 0)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            if (index < 0 || buffer.Length < index + 16)
                throw new ArgumentOutOfRangeException(nameof(index));

            return new DeviceInfo(
                (DeviceType)buffer.GetLEUInt32(index + 0),
                buffer.GetLEUInt32(index + 4),
                (PanFlag)buffer.GetLEUInt32(index + 8));
        }

        #endregion
    }
}
