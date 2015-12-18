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
using System.IO.Compression;
using System.Text;

namespace Sharp98.VGM
{
    public class Header : IFormatHeader
    {
        #region -- Private Static Fields --

        private static readonly byte[] magicNumber = new byte[] { 0x56, 0x67, 0x6d, 0x20 };
        private static readonly byte[] gzipMagicNumber = new byte[] { 0x1f, 0x8b };

        private static readonly Version version100 = new Version(1, 0);
        private static readonly Version version101 = new Version(1, 1);
        private static readonly Version version110 = new Version(1, 10);
        private static readonly Version version150 = new Version(1, 50);
        private static readonly Version version151 = new Version(1, 51);
        private static readonly Version version160 = new Version(1, 60);
        private static readonly Version version161 = new Version(1, 61);
        private static readonly Version version170 = new Version(1, 70);
        private static readonly Version version171 = new Version(1, 71);

        #endregion

        #region -- Public Static Fields --

        public static readonly Version LatestVersion = version171;

        #endregion

        #region -- Private Fields --

        private readonly IReadOnlyList<DumpData> vgmDumpData;
        private readonly IReadOnlyList<DeviceInfo> vgmDevices;
        private readonly TagCollection vgmTagCollection;
        private readonly Version version;

        #endregion

        #region -- Public Properties --

        public FormatType FormatType => FormatType.VGM;

        public int SampleTimeNumerator => 1;

        public int SampleTimeDenominator => 44100;

        public double SampleTime => 1.0 / 44100.0;

        public int LoopPointIndex { get; private set; }

        public IReadOnlyList<DumpData> VGMDumpData => this.vgmDumpData;

        public TagCollection VGMTagCollection => this.vgmTagCollection;

        public IReadOnlyList<DeviceInfo> VGMDevices => this.vgmDevices;


        public IReadOnlyList<IDumpData> DumpData => new CastedReadOnlyList<DumpData, IDumpData>(this.vgmDumpData);

        public ITagCollection TagCollection => this.vgmTagCollection;

        public IReadOnlyList<IDeviceInfo> Devices => new CastedReadOnlyList<DeviceInfo, IDeviceInfo>(this.vgmDevices);

        public Version Version => this.version;

        #endregion

        #region -- Constructors --

        public Header(
            int loopIndex,
            IReadOnlyList<DumpData> dump,
            TagCollection tag,
            IReadOnlyList<DeviceInfo> devices,
            Version version)
        {
            if (dump == null)
                throw new ArgumentNullException(nameof(dump));

            if (tag == null)
                throw new ArgumentNullException(nameof(tag));

            if (devices == null)
                throw new ArgumentNullException(nameof(devices));

            if (loopIndex < -1 || loopIndex >= dump.Count)
                throw new ArgumentNullException(nameof(loopIndex));

            if (version == null)
                throw new ArgumentNullException(nameof(version));

            this.LoopPointIndex = loopIndex;
            this.vgmDumpData = dump;
            this.vgmTagCollection = tag;
            this.vgmDevices = devices;
            this.version = version;
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

            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));

            throw new NotImplementedException();
        }

        #endregion

        #region -- Public Static Methods --

        public static Header Import(Stream stream, Encoding encoding = null)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (!stream.CanRead)
                throw new InvalidOperationException("読み取りができないストリームが指定されました.");

            var buffer = new byte[256];
            long eofOffset, gd3Offset, loopOffset, vgmOffset, vgmEndOffset;
            Version version;
            uint totalSampleCount, loopSampleCount;
            int loopIndex;
            IReadOnlyList<DeviceInfo> deviceInfo;
            IReadOnlyList<DumpData> dumpData;
            TagCollection tag;

            if (stream.Read(buffer, 0, 36) != 36)
                throw new EndOfStreamException();

            if (IsGziped(buffer))
            {
                if (!stream.CanSeek)
                    throw new InvalidOperationException("シークができないストリームが指定されました.");

                stream.Seek(-36L, SeekOrigin.Current);
                using (GZipStream gzs = new GZipStream(stream, CompressionMode.Decompress, true))
                    return Import(gzs, encoding);
            }

            if (!IsVGM(buffer))
                throw new InvalidDataException("VGM フォーマットではないデータが指定されました.");

            version = GetVersion(buffer);
            CheckBasicHeader(buffer, out eofOffset, out gd3Offset, out loopOffset, out totalSampleCount, out loopSampleCount);
            ReadMoreHeader(stream, buffer, version);
            deviceInfo = GetDeviceInfo(buffer, version);
            vgmOffset = SeekToVGMOffset(stream, buffer, version);
            vgmEndOffset = (gd3Offset > 0 ? gd3Offset : eofOffset);
            dumpData = GetDumpData(stream, encoding, deviceInfo, vgmOffset, vgmEndOffset, loopOffset, out loopIndex);
            tag = GetTagCollection(stream, buffer, gd3Offset, vgmEndOffset, eofOffset);

            return new Header(loopIndex, dumpData, tag, deviceInfo, version);
        }

        #endregion

        #region -- Private Methods --

        private static bool IsGziped(byte[] buffer)
        {
            for (int i = 0; i < gzipMagicNumber.Length; i++)
                if (buffer[i] != gzipMagicNumber[i])
                    return false;

            return true;
        }

        private static bool IsVGM(byte[] buffer)
        {
            for (int i = 0; i < magicNumber.Length; i++)
                if (buffer[i] != magicNumber[i])
                    return false;

            return true;
        }

        private static void CheckBasicHeader(
            byte[] buffer,
            out long eofOffset,
            out long gd3Offset,
            out long loopOffset,
            out uint totalSampleCount,
            out uint loopSampleCount)
        {
            eofOffset = buffer.GetLEUInt32(0x04) + 0x04;
            gd3Offset = buffer.GetLEUInt32(0x14) + 0x14;
            loopOffset = buffer.GetLEUInt32(0x1c) + 0x1c;

            totalSampleCount = buffer.GetLEUInt32(0x18);
            loopSampleCount = buffer.GetLEUInt32(0x20);
        }

        private static Version GetVersion(byte[] buffer)
        {
            if (buffer[10] != 0 || buffer[11] != 0)
                throw new InvalidDataException("不明なバージョンの VGM データが読み込まれました.");

            int major, minor;

            if (!int.TryParse(buffer[9].ToString("x"), out major) ||
                !int.TryParse(buffer[8].ToString("x"), out minor))
                throw new InvalidDataException("不明なバージョンの VGM データが読み込まれました.");

            return new Version(major, minor);
        }

        private static void ReadMoreHeader(Stream stream, byte[] buffer, Version version)
        {
            const int startIndex = 36;

            if (version >= version161)
                stream.Read(buffer, startIndex, 0x100 - startIndex);
            else if (version >= version151)
                stream.Read(buffer, startIndex, 0x80 - startIndex);
            else
                stream.Read(buffer, startIndex, 0x40 - startIndex);
        }

        private static IReadOnlyList<DeviceInfo> GetDeviceInfo(byte[] buffer, Version version)
        {
            var list = new List<DeviceInfo>();

            TryAddDeviceInfo(buffer, list, 0x0c, DeviceType.SN76489);
            TryAddDeviceInfo(buffer, list, 0x10, DeviceType.YM2413);

            if (version >= version110)
            {
                TryAddDeviceInfo(buffer, list, 0x2c, DeviceType.YM2612);
                TryAddDeviceInfo(buffer, list, 0x30, DeviceType.YM2151);
            }

            if (version >= version151)
            {
                TryAddDeviceInfo(buffer, list, 0x38, DeviceType.SegaPCM);
                TryAddDeviceInfo(buffer, list, 0x40, DeviceType.RF5C68);
                TryAddDeviceInfo(buffer, list, 0x44, DeviceType.YM2203);
                TryAddDeviceInfo(buffer, list, 0x48, DeviceType.YM2608);
                TryAddDeviceInfo(buffer, list, 0x4c, DeviceType.YM2610B);
                TryAddDeviceInfo(buffer, list, 0x50, DeviceType.YM3812);
                TryAddDeviceInfo(buffer, list, 0x54, DeviceType.YM3526);
                TryAddDeviceInfo(buffer, list, 0x58, DeviceType.Y8950);
                TryAddDeviceInfo(buffer, list, 0x5c, DeviceType.YMF262);
                TryAddDeviceInfo(buffer, list, 0x60, DeviceType.YMF278B);
                TryAddDeviceInfo(buffer, list, 0x64, DeviceType.YMF271);
                TryAddDeviceInfo(buffer, list, 0x68, DeviceType.YMZ280B);
                TryAddDeviceInfo(buffer, list, 0x6c, DeviceType.RF5C164);
                TryAddDeviceInfo(buffer, list, 0x70, DeviceType.PWM);
                TryAddDeviceInfo(buffer, list, 0x74, DeviceType.AY8910);
            }

            if (version >= version161)
            {
                TryAddDeviceInfo(buffer, list, 0x80, DeviceType.GameBoyDMG);
                TryAddDeviceInfo(buffer, list, 0x84, DeviceType.NES_APU);
                TryAddDeviceInfo(buffer, list, 0x88, DeviceType.MultiPCM);
                TryAddDeviceInfo(buffer, list, 0x8c, DeviceType.uPD7759);
                TryAddDeviceInfo(buffer, list, 0x90, DeviceType.OKIM6258);
                TryAddDeviceInfo(buffer, list, 0x98, DeviceType.OKIM6295);
                TryAddDeviceInfo(buffer, list, 0x9c, DeviceType.K051649);
                TryAddDeviceInfo(buffer, list, 0xa0, DeviceType.K054539);
                TryAddDeviceInfo(buffer, list, 0xa4, DeviceType.HuC6280);
                TryAddDeviceInfo(buffer, list, 0xa8, DeviceType.C140);
                TryAddDeviceInfo(buffer, list, 0xac, DeviceType.K053260);
                TryAddDeviceInfo(buffer, list, 0xb0, DeviceType.Pokey);
                TryAddDeviceInfo(buffer, list, 0xb4, DeviceType.QSound);
            }

            if (version >= version171)
            {
                TryAddDeviceInfo(buffer, list, 0xb8, DeviceType.SCSP);
                TryAddDeviceInfo(buffer, list, 0xc0, DeviceType.WonderSwan);
                TryAddDeviceInfo(buffer, list, 0xc4, DeviceType.VSU);
                TryAddDeviceInfo(buffer, list, 0xc8, DeviceType.SAA1099);
                TryAddDeviceInfo(buffer, list, 0xcc, DeviceType.ES5503);
                TryAddDeviceInfo(buffer, list, 0xd0, DeviceType.ES5506);
                TryAddDeviceInfo(buffer, list, 0xd8, DeviceType.X1_010);
                TryAddDeviceInfo(buffer, list, 0xdc, DeviceType.C352);
                TryAddDeviceInfo(buffer, list, 0xe0, DeviceType.GA20);
            }

            return list;
        }

        private static long GetVGMDataOffset(byte[] buffer, Version version)
        {
            if (version >= version150)
                return 0x34 + buffer.GetLEUInt32(0x34);
            else
                return 0x40;
        }

        private static void TryAddDeviceInfo(byte[] buffer, List<DeviceInfo> list, int offset, DeviceType type)
        {
            uint clock = buffer.GetLEUInt32(offset);

            if (clock > 0)
            {
                list.Add(new DeviceInfo(type, (int)(clock & 0x7fffffff)));

                if ((clock & 0x80000000) != 0)
                    list.Add(new DeviceInfo(type, (int)(clock & 0x7fffffff)));
            }
        }

        private static long SeekTo(Stream stream, long currentPosition, long position)
        {
            if (currentPosition > position)
                throw new InvalidDataException();       // bug detector

            if (currentPosition == position)
                return position;

            long seekLength = position - currentPosition;
            var readCount = stream.Read(new byte[seekLength], 0, (int)seekLength);

            if (seekLength != readCount)
                throw new EndOfStreamException();

            return position;
        }

        private static long SeekToVGMOffset(Stream stream, byte[] buffer, Version version)
        {
            long currentPosition = (version >= version161 ? 0x100 : version >= version151 ? 0x80 : 0x40);
            long position = GetVGMDataOffset(buffer, version);

            return SeekTo(stream, currentPosition, position);
        }

        private static IReadOnlyList<DumpData> GetDumpData(
            Stream stream,
            Encoding encoding,
            IReadOnlyList<DeviceInfo> devices,
            long startPosition,
            long endPosition,
            long loopOffset,
            out int loopIndex)
        {
            long currentPosition = startPosition;
            var list = new List<DumpData>();
            loopIndex = -1;

            while (currentPosition < endPosition)
            {
                if (currentPosition == loopOffset)
                    loopIndex = list.Count;

                int readLength;
                list.Add(VGM.DumpData.Import(stream, encoding, devices, out readLength));
                currentPosition += readLength;
            }

            // error check
            if (currentPosition != endPosition)
                throw new InvalidDataException();

            return list;
        }

        private static TagCollection GetTagCollection(Stream stream, byte[] buffer, long gd3Offset, long vgmEndOffset, long eofOffset)
        {
            if (gd3Offset > 0)
            {
                SeekTo(stream, vgmEndOffset, gd3Offset);
                byte[] tagBuffer;
                if (gd3Offset == eofOffset)
                {
                    using (var ms = new MemoryStream())
                    {
                        while (true)
                        {
                            int readCount = stream.Read(buffer, 0, 256);

                            if (readCount <= 0)
                                break;

                            ms.Write(buffer, 0, readCount);
                        }

                        tagBuffer = ms.ToArray();
                    }
                }
                else
                {
                    tagBuffer = new byte[eofOffset - gd3Offset];
                    stream.Read(tagBuffer, 0, tagBuffer.Length);
                }

                return VGM.TagCollection.Import(tagBuffer, 0);
            }
            else
                return new TagCollection();
        }

        #endregion
    }
}
