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
using System.Text;

namespace Sharp98.S98
{
    public class Header : IFormatHeader
    {
        #region -- Private Static Fields --

        private static readonly byte[] magicNumber = new byte[] { 0x53, 0x39, 0x38 };

        #endregion

        #region -- Public Static Fields --

        public const int DefaultNumerator = 10;

        public const int DefaultDenominator = 1000;

        #endregion

        #region -- Private Fields --

        private readonly IReadOnlyList<DumpData> s98DumpData;
        private readonly IReadOnlyList<DeviceInfo> s98Devices;
        private readonly TagCollection s98TagCollection;

        #endregion

        #region -- Public Properties --

        public FormatType FormatType => FormatType.S98;

        public int SampleTimeNumerator { get; private set; }

        public int SampleTimeDenominator { get; private set; }

        public double SampleTime
        {
            get
            {
                return (double)(this.SampleTimeNumerator == 0 ? DefaultNumerator : this.SampleTimeNumerator) /
                    (double)(this.SampleTimeDenominator == 0 ? DefaultDenominator : this.SampleTimeDenominator);
            }
        }

        public int LoopPointIndex { get; private set; }

        public IReadOnlyList<DumpData> S98DumpData => this.s98DumpData;

        public IReadOnlyList<DeviceInfo> S98Devices => this.s98Devices;

        public TagCollection S98TagCollection => this.s98TagCollection;

        public IReadOnlyList<IDumpData> DumpData => new CastedReadOnlyList<DumpData, IDumpData>(this.s98DumpData);

        public ITagCollection TagCollection => this.s98TagCollection;

        public IReadOnlyList<IDeviceInfo> Devices => new CastedReadOnlyList<DeviceInfo, IDeviceInfo>(this.s98Devices);

        public Version Version => new Version(3, 0);

        #endregion

        #region -- Constructors --

        public Header(
            uint timerInfo,
            uint timerInfo2,
            int loopIndex,
            IReadOnlyList<DumpData> dump,
            TagCollection tag,
            IReadOnlyList<DeviceInfo> devices)
        {
            if (timerInfo == 0 || timerInfo > int.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(timerInfo));

            if (timerInfo2 == 0 || timerInfo2 > int.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(timerInfo2));

            if (dump == null)
                throw new ArgumentNullException(nameof(dump));

            if (tag == null)
                throw new ArgumentNullException(nameof(tag));

            if (devices == null)
                throw new ArgumentNullException(nameof(devices));

            if (loopIndex < -1 || loopIndex >= dump.Count)
                throw new ArgumentNullException(nameof(loopIndex));

            this.SampleTimeNumerator = (int)timerInfo;
            this.SampleTimeDenominator = (int)timerInfo2;
            this.LoopPointIndex = loopIndex;
            this.s98DumpData = dump;
            this.s98TagCollection = tag;
            this.s98Devices = devices;
        }

        #endregion

        #region -- Public Methods --

        public void Export(string filepath, Encoding encoding)
        {
            if (filepath == null)
                throw new ArgumentNullException(nameof(filepath));

            using (var fs = new FileStream(filepath, FileMode.Create))
                this.Export(fs, encoding);
        }

        public void Export(Stream stream, Encoding encoding)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (!stream.CanWrite)
                throw new InvalidOperationException("書き込みのできないストリームに書き込もうとしました.");

            if (!stream.CanSeek)
                throw new InvalidOperationException("シークのできないストリームに書き込もうとしました.");

            if (this.s98Devices.Count > 64)
                throw new InvalidOperationException("デバイス数は 64 個以下である必要があります.");

            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));

            long basePosition = stream.Position;
            long loopPosition, tagPosition;

            this.ExportHeader(stream);
            this.ExportDevice(stream);
            this.ExportDump(stream, out loopPosition);

            if (this.s98TagCollection.Count == 0)
                tagPosition = 0L;
            else
            {
                tagPosition = stream.Position;
                ExportTag(stream, encoding);
            }

            long endPosition = stream.Position;
            var buffer = new byte[8];
            ((uint)tagPosition).GetLEByte(buffer, 0);
            ((uint)loopPosition).GetLEByte(buffer, 4);

            stream.Seek(basePosition + 0x10L, SeekOrigin.Begin);
            stream.Write(buffer, 0, 4);
            stream.Seek(basePosition + 0x18L, SeekOrigin.Begin);
            stream.Write(buffer, 4, 4);

            stream.Seek(endPosition, SeekOrigin.Begin);
        }

        #endregion

        #region -- Public Static Methods --

        public static Header Import(Stream stream, Encoding encoding = null)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (!stream.CanRead)
                throw new InvalidOperationException("読み取りができないストリームが指定されました.");

            if (!stream.CanSeek)
                throw new InvalidOperationException("シークができないストリームが指定されました.");

            var buffer = new byte[32];
            int loop_index = 0;
            DeviceInfo[] di;
            TagCollection tagCollection;
            IReadOnlyList<DumpData> dumpData;
            uint timerInfo, timerInfo2;
            uint tag, dump, loop, device_count;

            stream.Read(buffer, 0, 32);
            timerInfo = buffer.GetLEUInt32(4);
            timerInfo2 = buffer.GetLEUInt32(8);
            tag = buffer.GetLEUInt32(16);
            dump = buffer.GetLEUInt32(20);
            loop = buffer.GetLEUInt32(24);
            device_count = buffer.GetLEUInt32(28);

            if (!CheckMagicAndVersion(buffer, 0))
                throw new InvalidDataException("不明なファイル形式、またはバージョンです.");

            if ((tag < dump) || (tag >= stream.Length))
                throw new InvalidDataException("タグ情報のオフセットが範囲外です.");

            if (dump > stream.Length)
                throw new InvalidDataException("ダンプ情報のオフセットが範囲外です.");

            if ((loop > stream.Length) || (loop != 0 && loop < dump))
                throw new InvalidDataException("ループ位置のオフセットが範囲外です.");

            if (device_count > 64)
                throw new InvalidDataException("デバイス数が 64 個を超えています.");

            di = ImportDevice(device_count, stream, buffer);
            dumpData = ImportDump(stream, dump, loop, out loop_index, buffer);
            tagCollection = ImportTag(stream, tag, encoding);

            return new Header(timerInfo, timerInfo2, loop_index, dumpData, tagCollection, di);
        }

        #endregion

        #region -- Private Methods --

        private void ExportHeader(Stream stream)
        {
            var buffer = new byte[32];

            // Magic Number
            Array.Copy(magicNumber, 0, buffer, 0, 3);

            // Format Version
            buffer[3] = (byte)'3';

            // Timer Info
            ((uint)(this.SampleTimeNumerator == 0 ? DefaultNumerator : this.SampleTimeNumerator)).GetLEByte(buffer, 4);

            // Timer Info2
            ((uint)(this.SampleTimeDenominator == 0 ? DefaultDenominator : this.SampleTimeDenominator)).GetLEByte(buffer, 8);

            // Compressing == 0
            // File Offset to Dump Data
            (this.s98Devices.Count == 0 ? 32 : 32 + 16 * (uint)this.s98Devices.Count).GetLEByte(buffer, 20);

            // Device Count
            (this.s98Devices.Count == 0 ? 0 : (uint)this.s98Devices.Count).GetLEByte(buffer, 28);
            stream.Write(buffer, 0, 32);
        }

        private static bool CheckMagicAndVersion(byte[] buffer, int index)
        {
            // magic
            for (int i = 0; i < magicNumber.Length; i++)
                if (buffer[i] != magicNumber[i])
                    return false;

            // version
            if (buffer[3] != 0x33)
                return false;

            return true;
        }

        private static DeviceInfo[] ImportDevice(uint deviceCount, Stream stream, byte[] buffer)
        {
            if (deviceCount == 0)
                return new DeviceInfo[] { DeviceInfo.DefaultDevice };
            else
            {
                var di = new DeviceInfo[deviceCount];
                stream.Read(buffer, 0, 16);

                for (int i = 0; i < deviceCount; i++)
                    di[i] = DeviceInfo.Import(buffer);

                return di;
            }
        }

        private static IReadOnlyList<DumpData> ImportDump(Stream stream, uint dump_offset, uint loop_offset, out int loop_index, byte[] buffer)
        {
            int buffer_int;
            var dumpData = new List<DumpData>();
            loop_index = -1;

            stream.Seek(dump_offset, SeekOrigin.Begin);

            while (true)
            {
                if (stream.Position == loop_offset)
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
                    dumpData.Add(S98.DumpData.Import(buffer));
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

                dumpData.Add(S98.DumpData.Import(buffer));
            }

            return dumpData;
        }

        private static TagCollection ImportTag(Stream stream, uint tag_offset, Encoding encoding)
        {
            if (tag_offset == 0)
            {
                return new TagCollection();
            }
            else
            {
                stream.Seek(tag_offset, SeekOrigin.Begin);
                var tagBuffer = new byte[stream.Length - tag_offset];
                stream.Read(tagBuffer, 0, tagBuffer.Length);

                if (encoding == null)
                    return new TagCollection(tagBuffer);
                else
                    return new TagCollection(tagBuffer, encoding);
            }
        }

        private void ExportDevice(Stream stream)
        {
            var buffer = new byte[16];

            foreach (var device in this.s98Devices)
            {
                device.Export(buffer, 0, 16, null);
                stream.Write(buffer, 0, 16);
            }
        }

        private void ExportDump(Stream stream, out long loopPosition)
        {
            var buffer = new byte[6];
            var dumpCount = 0;

            // 0 means no loop
            loopPosition = 0L;

            foreach (var dump in this.s98DumpData)
            {
                if (this.LoopPointIndex == dumpCount)
                    loopPosition = stream.Position;

                int dumpLength = dump.Export(buffer, 0, 16, null);
                stream.Write(buffer, 0, dumpLength);

                dumpCount++;
            }
        }

        private void ExportTag(Stream stream, Encoding encoding)
        {
            this.s98TagCollection.Export(stream, encoding);
        }

        #endregion
    }
}
