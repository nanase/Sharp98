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
using System.IO;
using System.Text;
using Sharp98.DataParameter;

namespace Sharp98.S98
{
    public struct DumpData : IDumpData
    {
        #region -- Private Info --

        private readonly byte op;
        private readonly byte address;
        private readonly byte data;
        private readonly int sync_wait_time;

#pragma warning disable 414 

        // unused, but reserved
        private readonly byte reserved;

#pragma warning restore 414

        #endregion

        #region -- Public Properties --

        public DataType DataType
        {
            get
            {
                if (this.op < 64)
                    return DataType.AddressAndData;
                else if (this.op == 0xff || this.op == 0xfe)
                    return DataType.Wait;
                else if (this.op == 0xfd)
                    return DataType.EndMarker;
                else
                    return DataType.Unknown;
            }
        }

        public int TargetDevice
        {
            get
            {
                if (this.op < 64)
                    return this.op / 2;
                else
                    return -1;
            }
        }

        public object Parameter
        {
            get
            {
                if (this.op < 64)
                    return new AddressAndData(this.address, this.data);
                else if (this.op == 0xff)
                    return 1;
                else if (this.op == 0xfe)
                    return this.sync_wait_time;
                else
                    return null;
            }
        }

        public byte Operator => this.op;

        public byte Address => this.address;

        public byte Data => this.data;

        public int SyncWaitTime => this.sync_wait_time;

        #endregion

        #region -- Constructors --

        public DumpData(byte op, byte address, byte data, int syncWaitTime)
        {
            if (op == 0xfe && syncWaitTime < 2)
                throw new ArgumentOutOfRangeException(nameof(syncWaitTime));

            this.op = op;
            this.address = address;
            this.data = data;
            this.reserved = 0;
            this.sync_wait_time = syncWaitTime;
        }

        #endregion

        #region -- Public Methods --

        public int Export(byte[] buffer, int index = 0)
        {
            if (this.op < 0x80)
            {
                buffer[index++] = this.op;
                buffer[index++] = this.address;
                buffer[index] = this.data;
                return 3;
            }
            else if (this.op == 0xfd || this.op == 0xff)
            {
                buffer[index] = this.op;
                return 1;
            }
            else if (this.op == 0xfe)
            {
                buffer[index] = this.op;
                int vvlength = GetVVArray(this.sync_wait_time, buffer, index + 1);
                return vvlength + 1;
            }
            else
                throw new InvalidOperationException();
        }

        public void Export(Stream stream)
        {
            this.Export(stream, null);
        }

        public void Export(Stream stream, Encoding encoding)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (!stream.CanWrite)
                throw new InvalidOperationException("書き込みのできないストリームが指定されました.");

            stream.WriteByte(this.op);

            if (this.op < 0x80)
            {
                stream.WriteByte(this.address);
                stream.WriteByte(this.data);
            }
            else if (this.op == 0xfd || this.op == 0xff)
                return;
            else if (this.op == 0xfe)
            {
                var buffer = new byte[5];
                int vvlength = GetVVArray(this.sync_wait_time, buffer, 0);
                stream.Write(buffer, 0, 5);
            }
            else
                throw new InvalidOperationException();
        }

        #endregion

        #region -- Public Static Methods --

        public static DumpData Import(byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            if (buffer.Length <= 1)
                throw new ArgumentOutOfRangeException(nameof(buffer));

            if (buffer[0] < 0x80)
                return new DumpData(buffer[0], buffer[1], buffer[2], 0);
            else if (buffer[0] == 0xfd || buffer[0] == 0xff)
                return new DumpData(buffer[0], 0, 0, 0);
            else if (buffer[0] == 0xfe)
            {
                var array = new byte[5];
                Array.Copy(buffer, 1, array, 0, 5);
                return new DumpData(buffer[0], 0, 0, GetVVValue(array));
            }
            else
                throw new InvalidOperationException();
        }

        #endregion

        #region -- Private Static Methods --

        private static int GetVVValue(byte[] array, int index = 0)
        {
            // S98v3 掲載の算出コードから取得方法を類推
            // 最大値は Int32.MaxValue として取り扱う (2,147,483,645 - 2)
            // 返却値の範囲: 2 - 2,147,483,645
            
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (index < 0 || array.Length <= index)
                throw new ArgumentOutOfRangeException(nameof(index));

            int res, current;

            // 1st byte
            current = array[index++];
            res = current & 0x7f;
            if (current < 128)
                return res + 2;

            // 2nd byte
            current = array[index++];
            res += (current & 0x7f) << 7;
            if (current < 128)
                return res + 2;

            // 3rd byte
            current = array[index++];
            res += (current & 0x7f) << 14;
            if (current < 128)
                return res + 2;

            // 4th byte
            current = array[index++];
            res += (current & 0x7f) << 21;
            if (current < 128)
                return res + 2;

            // 5th byte
            current = array[index++];
            res += (current & 0x7f) << 28;
            if (current < 128)
            {
                if (res > 2147483645 || res < 0)
                    throw new OverflowException();
                else
                    return res + 2;
            }

            // 6th byte or more
            throw new OverflowException();
        }

        private static int GetVVArray(int value, byte[] buffer, int index = 0)
        {
            if (value < 2)
                throw new ArgumentOutOfRangeException(nameof(value));

            value -= 2;
            
            // 1st byte : 0 - 127
            buffer[index] = (byte)(value & 0x7f);
            if (value < 128)
                return 1;

            // 2nd byte : 128 - 16383
            buffer[index] += 128;
            buffer[++index] = (byte)((value >> 7) & 0x7f);
            if (value < 16384)
                return 2;

            // 3nd byte : 16384 - 2097150
            buffer[index] += 128;
            buffer[++index] = (byte)((value >> 14) & 0x7f);
            if (value < 2097152)
                return 3;

            // 4th byte : 2097151 - 268435456
            buffer[index] += 128;
            buffer[++index] = (byte)((value >> 21) & 0x7f);
            if (value < 268435456)
                return 4;

            // 5th byte : 2097151 - 268435456
            buffer[index] += 128;
            buffer[++index] = (byte)((value >> 28) & 0x07);
            return 5;
        }

        #endregion
    }
}
