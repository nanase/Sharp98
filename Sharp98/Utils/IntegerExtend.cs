﻿//
// IntegerExtend.cs
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
    static class IntegerExtend
    {
        public static byte[] GetLEByte(this uint value)
        {
            var array = BitConverter.GetBytes(value);

            if (!BitConverter.IsLittleEndian)
                Array.Reverse(array);

            return array;
        }

        public static void GetLEByte(this uint value, byte[] array, int index = 0)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (index < 0 || array.Length < index + 4)
                throw new ArgumentOutOfRangeException(nameof(array));

            unsafe
            {
                byte* vptr = (byte*)&value;

                if (BitConverter.IsLittleEndian)
                {
                    array[index++] = *(vptr++);
                    array[index++] = *(vptr++);
                    array[index++] = *(vptr++);
                }
                else
                {
                    index += 3;
                    array[index--] = *(vptr++);
                    array[index--] = *(vptr++);
                    array[index--] = *(vptr++);
                }

                array[index] = *vptr;
            }
        }

        public static uint GetLEUInt32(this byte[] array, int index = 0)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (index < 0 || array.Length < index + 4)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (!BitConverter.IsLittleEndian)
                Array.Reverse(array, index, 4);

            return BitConverter.ToUInt32(array, index);
        }
    }
}
