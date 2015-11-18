//
// Header.cs
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
using System.Collections.Generic;
using System.IO;

namespace Sharp98
{
    public class Header
    {
        #region -- Private Static Fields --

        private static readonly byte[] magicNumber = new byte[] { 0x53, 0x39, 0x38 };

        #endregion

        #region -- Public Static Fields --

        public const int DefaultNumerator = 10;

        public const int DefaultDenominator = 1000;

        #endregion

        #region -- Public Properties --

        public uint TimerInfo { get; private set; }

        public uint TimerInfo2 { get; private set; }

        public double SyncTime
        {
            get
            {
                return (double)(this.TimerInfo == 0 ? DefaultNumerator : this.TimerInfo) /
                    (double)(this.TimerInfo2 == 0 ? DefaultDenominator : this.TimerInfo2);
            }
        }

        public int LoopPointDumpIndex { get; private set; }

        public IReadOnlyList<DumpData> DumpData { get; private set; }

        public TagCollection Tag { get; private set; }

        public IReadOnlyList<DeviceInfo> Device { get; private set; }

        #endregion

        #region -- Constructors --

        public Header(
            uint timerInfo,
            uint timerInfo2,
            int loopIndex,
            IReadOnlyList<DumpData> dump,
            TagCollection tag,
            IReadOnlyList<DeviceInfo> device)
        {
            this.TimerInfo = timerInfo;
            this.TimerInfo2 = timerInfo2;
            this.LoopPointDumpIndex = loopIndex;
            this.DumpData = dump;
            this.Tag = tag;
            this.Device = device;
        }

        #endregion

        #region -- Public Static Methods --

        public static Header Import(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (!stream.CanRead)
                throw new InvalidOperationException();

            if (!stream.CanSeek)
                throw new InvalidOperationException();

            var buffer = new byte[32];
            byte[] tagBuffer;
            int buffer_int, loop_index = 0;
            DeviceInfo[] di;
            TagCollection tagCollection;
            List<DumpData> dumpData = new List<DumpData>();
            uint timerInfo, timerInfo2;
            uint tag, dump, loop, device_count;

            stream.Read(buffer, 0, 32);

            // Magic / Version
            {
                for (int i = 0; i < magicNumber.Length; i++)
                    if (buffer[i] != magicNumber[i])
                        throw new InvalidDataException();

                if (buffer[3] != 0x33)
                    throw new InvalidDataException();
            }

            timerInfo = buffer.GetLEUInt32(4);
            timerInfo2 = buffer.GetLEUInt32(8);
            tag = buffer.GetLEUInt32(16);
            dump = buffer.GetLEUInt32(20);
            loop = buffer.GetLEUInt32(24);
            device_count = buffer.GetLEUInt32(28);

            if (tag > stream.Length)
                throw new InvalidDataException();

            if (dump > stream.Length)
                throw new InvalidDataException();

            if (loop > stream.Length)
                throw new InvalidDataException();

            if (loop != 0 && loop < dump)
                throw new InvalidDataException();

            if (tag < dump)
                throw new InvalidDataException();

            if (device_count > 64)
                throw new InvalidDataException();

            if (device_count == 0)
                di = new DeviceInfo[] { DeviceInfo.DefaultDevice };
            else
            {
                di = new DeviceInfo[device_count];
                stream.Read(buffer, 0, 16);

                for (int i = 0; i < device_count; i++)
                    di[i] = DeviceInfo.Import(buffer);
            }

            stream.Seek(dump, SeekOrigin.Begin);
            while (true)
            {
                if (stream.Position == loop)
                    loop_index = dumpData.Count;

                buffer_int = stream.ReadByte();
                stream.Seek(-1L, SeekOrigin.Current);

                if (buffer_int >= 0 && buffer_int < 128)
                    stream.Read(buffer, 0, 3);
                else if (buffer_int == 0xff)
                    stream.Read(buffer, 0, 1);
                else if (buffer_int == 0xfd)
                {
                    stream.Read(buffer, 0, 1);
                    dumpData.Add(Sharp98.DumpData.Import(buffer));
                    break;
                }
                else if (buffer_int == 0xfe)
                {
                    stream.Read(buffer, 0, 6);
                    int arrayLength = Array.FindIndex(buffer, 1, 5, e => e < 128);

                    if (arrayLength == -1)
                        throw new InvalidDataException();
                    else
                        stream.Seek(arrayLength - 5L, SeekOrigin.Current);
                }
                else
                    throw new InvalidDataException();

                dumpData.Add(Sharp98.DumpData.Import(buffer));
            }

            stream.Seek(tag, SeekOrigin.Begin);
            tagBuffer = new byte[stream.Length - tag];
            stream.Read(tagBuffer, 0, tagBuffer.Length);
            tagCollection = new TagCollection(tagBuffer);

            return new Header(timerInfo, timerInfo2, loop_index, dumpData, tagCollection, di);
        }

        #endregion
    }
}
