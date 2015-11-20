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

namespace Sharp98
{
    public struct DumpData
    {
        #region -- Private Info --

        private readonly byte op;
        private readonly byte address;
        private readonly byte data;
        private readonly byte reserved;
        private readonly int sync_wait_time;

        #endregion

        #region -- Public Properties --

        public byte Operator { get { return this.op; } }

        public byte Address { get { return this.address; } }

        public byte Data { get { return this.data; } }

        public int SyncWaitTime { get { return this.sync_wait_time; } }

        #endregion

        #region -- Constructors --

        public DumpData(byte op, byte address, byte data, int syncWaitTime)
        {
            if (syncWaitTime < 2)
                throw new ArgumentOutOfRangeException(nameof(syncWaitTime));

            this.op = op;
            this.address = address;
            this.data = data;
            this.reserved = 0;
            this.sync_wait_time = syncWaitTime;
        }

        #endregion

        #region -- Public Methods --

        public byte[] Export()
        {
            if (this.op < 0x80)
                return new[] { this.op, this.address, this.data };
            else if (this.op == 0xfd || this.op == 0xff)
                return new[] { this.op };
            else if (this.op == 0xfe)
            {
                var vv = GetVVArray(this.sync_wait_time);
                var array = new byte[vv.Length + 1];
                vv[0] = this.op;
                Array.Copy(vv, 0, array, 1, vv.Length);
                return array;
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
            if (current >= 128)
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

        private static byte[] GetVVArray(int value)
        {
            if (value < 2)
                throw new ArgumentOutOfRangeException(nameof(value));

            value -= 2;

            if (value < 128)
                return new[] { (byte)(value & 0x7f) };
            else if (value < 16384)
                return new[] { (byte)(value & 0x7f + 128), (byte)((value >> 7) & 0x7f) };
            else if (value < 2097151)
                return new[] { (byte)(value & 0x7f + 128), (byte)((value >> 7) & 0x7f + 128), (byte)((value >> 14) & 0x7f) };
            else if (value < 268435456)
                return new[] { (byte)(value & 0x7f + 128), (byte)((value >> 7) & 0x7f + 128), (byte)((value >> 14) & 0x7f + 128), (byte)((value >> 21) & 0x7f) };
            else
                return new[] { (byte)(value & 0x7f + 128), (byte)((value >> 7) & 0x7f + 128), (byte)((value >> 14) & 0x7f + 128), (byte)((value >> 21) & 0x7f + 128), (byte)((value >> 28) & 0x07) };
        }

        #endregion
    }
}
