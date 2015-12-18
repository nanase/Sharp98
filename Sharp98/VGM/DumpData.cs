//
// DumpData.cs
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
using Sharp98.DataParameter;

namespace Sharp98.VGM
{
    public struct DumpData : IDumpData
    {
        #region -- Private Info --

        private readonly byte command;
        private readonly int targetDevice;
        private readonly object parameter;

        #endregion

        #region -- Public Properties --

        public DataType DataType => GetDataType(this.command);

        public int TargetDevice => this.targetDevice;

        public object Parameter => this.parameter;

        #endregion

        #region -- Constructors --

        public DumpData(byte command, int targetDevice, object parameter)
        {
            this.command = command;
            this.targetDevice = targetDevice;
            this.parameter = parameter;
        }

        #endregion

        #region -- Public Methods --

        public byte[] Export(Encoding encoding)
        {
            throw new NotImplementedException();
        }

        public int Export(byte[] buffer, int index, int length, Encoding encoding)
        {
            throw new NotImplementedException();
        }

        public void Export(Stream stream, Encoding encoding)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region -- Public Static Methods --

        public static DumpData Import(Stream stream, Encoding encoding, IReadOnlyList<DeviceInfo> devices, out int readLength)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (!stream.CanRead)
                throw new InvalidOperationException("読み取りのできないストリームが指定されました.");

            if (devices == null)
                throw new ArgumentNullException(nameof(devices));

            int command = stream.ReadByte();

            if (command == -1)
                throw new EndOfStreamException();

            var buffer = new byte[11];
            readLength = readByteLength[command];

            if (stream.Read(buffer, 0, readLength) != readLength)
                throw new EndOfStreamException();

            int deviceIndex = GetDeviceIndex(command, buffer[0], devices);
            int additionalReadLength;
            var obj = GetObject(command, stream, buffer, out additionalReadLength);

            readLength += additionalReadLength;
            readLength++;

            return new DumpData((byte)command, deviceIndex, obj);
        }

        #endregion

        #region -- Private Static Methods --

        private static readonly int[] readByteLength = new int[256]
            {
                0, 0, 0,  0, 0, 0, 0, 0,  0, 0, 0, 0, 0, 0, 0, 0, /* 0x00 - 0x0f */ // unused
                0, 0, 0,  0, 0, 0, 0, 0,  0, 0, 0, 0, 0, 0, 0, 0, /* 0x10 - 0x1f */ // unused
                0, 0, 0,  0, 0, 0, 0, 0,  0, 0, 0, 0, 0, 0, 0, 0, /* 0x20 - 0x2f */ // unused
                1, 1, 1,  1, 1, 1, 1, 1,  1, 1, 1, 1, 1, 1, 1, 1, /* 0x30 - 0x3f */
                2, 2, 2,  2, 2, 2, 2, 2,  2, 2, 2, 2, 2, 2, 2, 1, /* 0x40 - 0x4f */
                1, 2, 2,  2, 2, 2, 2, 2,  2, 2, 2, 2, 2, 2, 2, 2, /* 0x50 - 0x5f */
                0, 2, 0,  0, 0, 0, 0, 6, 11, 0, 0, 0, 0, 0, 0, 0, /* 0x60 - 0x6f */
                0, 0, 0,  0, 0, 0, 0, 0,  0, 0, 0, 0, 0, 0, 0, 0, /* 0x70 - 0x7f */
                0, 0, 0,  0, 0, 0, 0, 0,  0, 0, 0, 0, 0, 0, 0, 0, /* 0x80 - 0x8f */
                4, 4, 5, 10, 1, 4, 0, 0,  0, 3, 3, 3, 3, 3, 3, 3, /* 0x90 - 0x9f */
                2, 2, 2,  2, 2, 2, 2, 2,  2, 2, 2, 2, 2, 2, 2, 2, /* 0xa0 - 0xaf */
                2, 2, 2,  2, 2, 2, 2, 2,  2, 2, 2, 2, 2, 2, 2, 2, /* 0xb0 - 0xbf */
                3, 3, 3,  3, 3, 3, 3, 3,  0, 0, 0, 0, 0, 0, 0, 0, /* 0xc0 - 0xcf */
                3, 3, 3,  3, 3, 3, 3, 3,  3, 3, 3, 3, 3, 3, 3, 3, /* 0xd0 - 0xdf */
                4, 4, 4,  4, 4, 4, 4, 4,  4, 4, 4, 4, 4, 4, 4, 4, /* 0xe0 - 0xef */
                4, 4, 4,  4, 4, 4, 4, 4,  4, 4, 4, 4, 4, 4, 4, 4, /* 0xf0 - 0xff */
            };

        private static object GetObject(int command, Stream stream, byte[] buffer, out int additionalReadLength)
        {
            additionalReadLength = 0;

            switch (command)
            {
                /* 1 param */
                case 0x30:
                case 0x4f:
                case 0x50:
                    return (int)buffer[0];

                /* 2 params -- aa dd */
                case 0x51:
                case 0x52:
                case 0x54:
                case 0x56:
                case 0x58:
                case 0x5a:
                case 0x5b:
                case 0x5c:
                case 0x5d:
                case 0x5e:
                case 0xa0:
                case 0xa1:
                case 0xa2:
                case 0xa4:
                case 0xa6:
                case 0xa8:
                case 0xaa:
                case 0xab:
                case 0xac:
                case 0xad:
                case 0xae:
                case 0xb0:
                case 0xb2:
                case 0xb3:
                case 0xb4:
                case 0xb5:
                case 0xb6:
                case 0xb7:
                case 0xb8:
                case 0xb9:
                case 0xba:
                case 0xbb:
                case 0xbc:
                case 0xbd:
                case 0xbe:
                case 0xbf:
                    return new AddressAndData(buffer[0], buffer[1]);

                case 0x53:
                case 0x55:
                case 0x57:
                case 0x59:
                case 0x5f:
                case 0xa3:
                case 0xa5:
                case 0xa7:
                case 0xa9:
                case 0xaf:
                    return new AddressAndData(buffer[0] + 0x100, buffer[1]);

                /* 2 params -- bbaa dd */
                case 0xc0:
                case 0xc1:
                case 0xc2:
                    return new AddressAndData(buffer[0] | buffer[1] << 8, buffer[2]);

                /* 2 params -- cc bbaa */
                case 0xc3:
                    return new ChannelAndAddress(buffer[0], buffer[1] << 8 | buffer[2]);

                /* 2 params -- mmll rr (or mmll dd) */
                case 0xc4:
                case 0xc5:
                case 0xc6:
                case 0xc7:
                case 0xc8:
                    return new AddressAndData(buffer[0] << 8 | buffer[1], buffer[2]);

                /* 3 params -- pp aa dd */
                case 0xd0:
                case 0xd1:
                case 0xd2:
                case 0xd3:
                case 0xd4:
                case 0xd5:
                case 0xd6:
                    return new PortAddressAndData(buffer[0], buffer[1], buffer[2]);

                /* 1 params -- dddddddd */
                case 0xe0:
                    return buffer.GetLEUInt32(0);

                /* 2 params -- mmll aadd */
                case 0xe1:
                    return new ChannelAndAddress(buffer[0] << 8 | buffer[1], buffer[2] << 8 | buffer[3]);

                /* DataBlock */
                case 0x67:
                    return ImportVGMDataBlock(buffer, 0, stream, out additionalReadLength);

                /* PCM RAM Write */
                case 0x68:
                    throw new NotImplementedException();

                /* DAC Stream Control Write */
                case 0x90:
                case 0x91:
                case 0x92:
                case 0x93:
                case 0x94:
                case 0x95:
                    throw new NotImplementedException();

                /* wait */
                case 0x61:
                    return buffer[0] | buffer[1] << 8;

                case 0x62:
                    return 735;

                case 0x63:
                    return 882;

                case 0x70: return 1;
                case 0x71: return 2;
                case 0x72: return 3;
                case 0x73: return 4;
                case 0x74: return 5;
                case 0x75: return 6;
                case 0x76: return 7;
                case 0x77: return 8;
                case 0x78: return 9;
                case 0x79: return 10;
                case 0x7a: return 11;
                case 0x7b: return 12;
                case 0x7c: return 13;
                case 0x7d: return 14;
                case 0x7e: return 15;
                case 0x7f: return 16;

                case 0x80: return 0;
                case 0x81: return 1;
                case 0x82: return 2;
                case 0x83: return 3;
                case 0x84: return 4;
                case 0x85: return 5;
                case 0x86: return 6;
                case 0x87: return 7;
                case 0x88: return 8;
                case 0x89: return 9;
                case 0x8a: return 10;
                case 0x8b: return 11;
                case 0x8c: return 12;
                case 0x8d: return 13;
                case 0x8e: return 14;
                case 0x8f: return 15;

                default: return null;
            }
        }

        private static DataType GetDataType(int command)
        {
            switch (command)
            {
                /* 1 param -- dd */
                case 0x30:
                case 0x4f:
                case 0x50:
                    return DataType.DataOnly;

                case 0x51:
                case 0x52:
                case 0x53:
                case 0x54:
                case 0x55:
                case 0x56:
                case 0x57:
                case 0x58:
                case 0x59:
                case 0x5a:
                case 0x5b:
                case 0x5c:
                case 0x5d:
                case 0x5e:
                case 0x5f:
                case 0xa0:
                case 0xb0:
                case 0xb2:
                case 0xb3:
                case 0xb4:
                case 0xb5:
                case 0xb6:
                case 0xb7:
                case 0xb8:
                case 0xb9:
                case 0xba:
                case 0xbb:
                case 0xbc:
                case 0xbd:
                case 0xbe:
                case 0xbf:
                case 0xc0:
                case 0xc1:
                case 0xc2:
                case 0xc4:
                case 0xc5:
                case 0xc6:
                case 0xc7:
                case 0xc8:
                case 0xd0:
                case 0xd1:
                case 0xd2:
                case 0xd3:
                case 0xd4:
                case 0xd5:
                case 0xd6:
                case 0xe1:
                    return DataType.AddressAndData;

                /* 2 params -- cc bbaa */
                case 0xc3:
                /* 1 param -- dddddd */
                case 0xe0:
                    return DataType.AddressOnly;

                /* DataBlock */
                /* PCM RAM Write */
                /* DAC Stream Control Write */
                case 0x67:
                case 0x68:
                case 0x90:
                case 0x91:
                case 0x92:
                case 0x93:
                case 0x94:
                case 0x95:
                    return DataType.DataStreamControl;

                case 0x61:
                case 0x62:
                case 0x63:
                case 0x70:
                case 0x71:
                case 0x72:
                case 0x73:
                case 0x74:
                case 0x75:
                case 0x76:
                case 0x77:
                case 0x78:
                case 0x79:
                case 0x7a:
                case 0x7b:
                case 0x7c:
                case 0x7d:
                case 0x7e:
                case 0x7f:
                case 0x80:
                case 0x81:
                case 0x82:
                case 0x83:
                case 0x84:
                case 0x85:
                case 0x86:
                case 0x87:
                case 0x88:
                case 0x89:
                case 0x8a:
                case 0x8b:
                case 0x8c:
                case 0x8d:
                case 0x8e:
                case 0x8f:
                    return DataType.Wait;

                case 0x66:
                    return DataType.EndMarker;
                    
                default: return DataType.Unknown;
            }
        }

        private static int GetDeviceIndex(int command, byte secondByte, IReadOnlyList<DeviceInfo> devices)
        {
            DeviceType type;
            bool secondary = false;

            switch (command)
            {
                case 0x30: type = DeviceType.SN76489; secondary = true; break;

                case 0x4f:
                case 0x50: type = DeviceType.SN76489; break;
                case 0x51: type = DeviceType.YM2413; break;

                case 0x52:
                case 0x53: type = DeviceType.YM2612; break;
                case 0x54: type = DeviceType.YM2151; break;
                case 0x55: type = DeviceType.YM2203; break;

                case 0x56:
                case 0x57: type = DeviceType.YM2608; break;

                case 0x58:
                case 0x59: type = DeviceType.YM2610B; break;
                case 0x5a: type = DeviceType.YM3812; break;
                case 0x5b: type = DeviceType.YM3526; break;
                case 0x5c: type = DeviceType.Y8950; break;
                case 0x5d: type = DeviceType.YMZ280B; break;

                case 0x5e:
                case 0x5f: type = DeviceType.YMF262; break;

                case 0x80:
                case 0x81:
                case 0x82:
                case 0x83:
                case 0x84:
                case 0x85:
                case 0x86:
                case 0x87:
                case 0x88:
                case 0x89:
                case 0x8a:
                case 0x8b:
                case 0x8c:
                case 0x8d:
                case 0x8e:
                case 0x8f: type = DeviceType.YM2612; break;
                case 0xa0: type = DeviceType.AY8910; break;

                case 0xa1: type = DeviceType.YM2413; secondary = true; break;

                case 0xa2:
                case 0xa3: type = DeviceType.YM2612; secondary = true; break;
                case 0xa4: type = DeviceType.YM2151; secondary = true; break;
                case 0xa5: type = DeviceType.YM2203; secondary = true; break;

                case 0xa6:
                case 0xa7: type = DeviceType.YM2608; secondary = true; break;

                case 0xa8:
                case 0xa9: type = DeviceType.YM2610B; secondary = true; break;
                case 0xaa: type = DeviceType.YM3812; secondary = true; break;
                case 0xab: type = DeviceType.YM3526; secondary = true; break;
                case 0xac: type = DeviceType.Y8950; secondary = true; break;
                case 0xad: type = DeviceType.YMZ280B; secondary = true; break;

                case 0xae:
                case 0xaf: type = DeviceType.YMF262; break;

                case 0xb0: type = DeviceType.RF5C68; secondary = (secondByte > 127); break;
                case 0xb1: type = DeviceType.RF5C164; secondary = (secondByte > 127); break;
                case 0xb2: type = DeviceType.PWM; secondary = (secondByte > 127); break;
                case 0xb3: type = DeviceType.GameBoyDMG; secondary = (secondByte > 127); break;
                case 0xb4: type = DeviceType.NES_APU; secondary = (secondByte > 127); break;
                case 0xb5: type = DeviceType.MultiPCM; secondary = (secondByte > 127); break;
                case 0xb6: type = DeviceType.uPD7759; secondary = (secondByte > 127); break;
                case 0xb7: type = DeviceType.OKIM6258; secondary = (secondByte > 127); break;
                case 0xb8: type = DeviceType.OKIM6295; secondary = (secondByte > 127); break;
                case 0xb9: type = DeviceType.HuC6280; secondary = (secondByte > 127); break;
                case 0xba: type = DeviceType.K053260; secondary = (secondByte > 127); break;
                case 0xbb: type = DeviceType.Pokey; secondary = (secondByte > 127); break;
                case 0xbc: type = DeviceType.WonderSwan; secondary = (secondByte > 127); break;
                case 0xbd: type = DeviceType.SAA1099; secondary = (secondByte > 127); break;
                case 0xbe: type = DeviceType.ES5506; secondary = (secondByte > 127); break;
                case 0xbf: type = DeviceType.GA20; secondary = (secondByte > 127); break;

                case 0xc0: type = DeviceType.SegaPCM; secondary = (secondByte > 127); break;
                case 0xc1: type = DeviceType.RF5C68; secondary = (secondByte > 127); break;
                case 0xc2: type = DeviceType.RF5C164; secondary = (secondByte > 127); break;
                case 0xc3: type = DeviceType.MultiPCM; secondary = (secondByte > 127); break;
                case 0xc4: type = DeviceType.QSound; secondary = (secondByte > 127); break;
                case 0xc5: type = DeviceType.SCSP; secondary = (secondByte > 127); break;
                case 0xc6: type = DeviceType.WonderSwan; secondary = (secondByte > 127); break;
                case 0xc7: type = DeviceType.VSU; secondary = (secondByte > 127); break;
                case 0xc8: type = DeviceType.X1_010; secondary = (secondByte > 127); break;

                case 0xd0: type = DeviceType.YMF278B; break;
                case 0xd1: type = DeviceType.YMF271; break;
                case 0xd2: type = DeviceType.SCC1; secondary = (secondByte > 127); break;
                case 0xd3: type = DeviceType.K054539; secondary = (secondByte > 127); break;
                case 0xd4: type = DeviceType.K051649; secondary = (secondByte > 127); break;
                case 0xd5: type = DeviceType.ES5506; secondary = (secondByte > 127); break;
                case 0xd6: type = DeviceType.ES5506; secondary = (secondByte > 127); break;

                case 0xe1: type = DeviceType.C352; secondary = (secondByte > 127); break;

                default:
                    return -1;
            }

            for (int i = 0, length = devices.Count; i < length; i++)
            {
                if (devices[i].VGMDeviceType == type)
                {
                    if (secondary)
                        secondary = false;
                    else
                        return i;
                }
            }

            return -1;
        }

        private static DataBlock ImportVGMDataBlock(byte[] headerBuffer, int index, Stream stream, out int readLength)
        {
            if (headerBuffer == null)
                throw new ArgumentNullException(nameof(headerBuffer));

            if (index < 0 || index >= headerBuffer.Length)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (!stream.CanRead)
                throw new InvalidOperationException("読み取りのできないストリームが指定されました.");

            readLength = (int)headerBuffer.GetLEUInt32(index + 2);
            var buffer = new byte[readLength];
            stream.Read(buffer, 0, readLength);

            return new DataBlock(headerBuffer[index + 1], buffer);
        }

        #endregion
    }
}
