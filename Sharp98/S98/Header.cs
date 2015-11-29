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

namespace Sharp98.S98
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
            if (timerInfo == 0)
                throw new ArgumentOutOfRangeException(nameof(timerInfo));

            if (timerInfo2 == 0)
                throw new ArgumentOutOfRangeException(nameof(timerInfo2));
            
            if (dump == null)
                throw new ArgumentNullException(nameof(dump));

            if (tag == null)
                throw new ArgumentNullException(nameof(tag));

            if (device == null)
                throw new ArgumentNullException(nameof(device));

            if (loopIndex < -1 || loopIndex >= dump.Count)
                throw new ArgumentNullException(nameof(loopIndex));

            this.TimerInfo = timerInfo;
            this.TimerInfo2 = timerInfo2;
            this.LoopPointDumpIndex = loopIndex;
            this.DumpData = dump;
            this.Tag = tag;
            this.Device = device;
        }

        #endregion

        #region -- Public Methods --
        
        public void Export(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (!stream.CanWrite)
                throw new InvalidOperationException("書き込みのできないストリームに書き込もうとしました.");

            if (!stream.CanSeek)
                throw new InvalidOperationException("シークのできないストリームに書き込もうとしました.");

            if (this.Device.Count > 64)
                throw new InvalidOperationException("デバイス数は 64 個以下である必要があります.");

            long basePosition = stream.Position;
            long loopPosition, tagPosition;

            this.ExportHeader(stream);
            this.ExportDevice(stream);
            this.ExportDump(stream, out loopPosition);

            if (this.Tag.Count == 0)
                tagPosition = 0L;
            else
            {
                tagPosition = stream.Position;
                ExportTag(stream);
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

        public static Header Import(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (!stream.CanRead)
                throw new InvalidOperationException("読み取りができないストリームが指定されました.");

            if (!stream.CanSeek)
                throw new InvalidOperationException("シークができないストリームが指定されました.");

            var buffer = new byte[32];
            byte[] tagBuffer;
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
            
            stream.Seek(dump, SeekOrigin.Begin);
            dumpData = ImportDump(stream, loop, out loop_index, buffer);
            
            stream.Seek(tag, SeekOrigin.Begin);
            tagBuffer = new byte[stream.Length - tag];
            stream.Read(tagBuffer, 0, tagBuffer.Length);
            tagCollection = new TagCollection(tagBuffer);

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
            (this.TimerInfo == 0 ? DefaultNumerator : this.TimerInfo).GetLEByte(buffer, 4);

            // Timer Info2
            (this.TimerInfo2 == 0 ? DefaultDenominator : this.TimerInfo2).GetLEByte(buffer, 8);

            // Compressing == 0
            // File Offset to Dump Data
            (this.Device.Count == 0 ? 32 : 32 + 16 * (uint)this.Device.Count).GetLEByte(buffer, 20);

            // Device Count
            (this.Device.Count == 0 ? 0 : (uint)this.Device.Count).GetLEByte(buffer, 28);
            stream.Write(buffer, 0, 32);
        }

        private static bool CheckMagicAndVersion(byte[] buffer, int index = 0)
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

        private static IReadOnlyList<DumpData> ImportDump(Stream stream, uint loop_offset, out int loop_index, byte[] buffer)
        {
            int buffer_int;
            var dumpData = new List<DumpData>();
            loop_index = 0;

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

        private void ExportDevice(Stream stream)
        {
            var buffer = new byte[16];

            foreach (var device in this.Device)
            {
                device.Export(buffer);
                stream.Write(buffer, 0, 16);
            }
        }

        private void ExportDump(Stream stream, out long loopPosition)
        {
            var buffer = new byte[6];
            var dumpCount = 0;

            // 0 means no loop
            loopPosition = 0L;

            foreach (var dump in this.DumpData)
            {
                if (this.LoopPointDumpIndex == dumpCount)
                    loopPosition = stream.Position;

                int dumpLength = dump.Export(buffer);
                stream.Write(buffer, 0, dumpLength);

                dumpCount++;
            }
        }

        private void ExportTag(Stream stream)
        {
            var buffer = this.Tag.Export(System.Text.Encoding.GetEncoding(932));
            stream.Write(buffer, 0, buffer.Length);
        }

        #endregion
    }
}
