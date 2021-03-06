﻿//
// TagCollection.cs
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
using System.Linq;
using System.Text;

namespace Sharp98.S98
{
    public class TagCollection : Dictionary<string, string>, ITagCollection
    {
        #region -- Private Fields --

        private static readonly byte[] marker = new byte[] { 0x5b, 0x53, 0x39, 0x38, 0x5d };
        private static readonly byte[] preamble = new byte[] { 0xef, 0xbb, 0xbf };

        #endregion

        #region -- Public Indexer --

        public new string this[string key]
        {
            get
            {
                return base[key];
            }
            set
            {
                CheckKeyValue(key, value);
                base[key.ToLower()] = value;
            }
        }

        #endregion

        #region -- Constructors --

        public TagCollection()
            : base()
        {
        }

        public TagCollection(byte[] import)
            : base()
        {
            CheckMarker(import);
            var isUTF8 = IsEncodedByUTF8(import);
            var encoding = (isUTF8 ? Encoding.UTF8 : Encoding.Default);
            this.Import(import, encoding, marker.Length + (isUTF8 ? preamble.Length : 0));
        }

        public TagCollection(byte[] import, Encoding encoding)
            : base()
        {
            CheckMarker(import);
            var isUTF8 = (encoding == Encoding.UTF8);
            this.Import(import, encoding, marker.Length + (isUTF8 ? preamble.Length : 0));
        }

        public TagCollection(IDictionary<string, string> dictionary)
            : base(dictionary.Count)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));

            foreach (var item in dictionary)
                this.Add(item.Key, item.Value);
        }

        #endregion

        #region -- Public Methods --

        public new void Add(string key, string value)
        {
            CheckKeyValue(key, value);
            base.Add(key.ToLower(), value);
        }

        public byte[] Export(Encoding encoding)
        {
            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));
            
            int requireLength = this.GetBufferOutputSize(encoding);
            var buffer = new byte[requireLength];
            this.ExportBuffer(buffer, 0, encoding);
            return buffer;
        }
        
        public int Export(byte[] buffer, int index, int length, Encoding encoding)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));

            int requireLength = this.GetBufferOutputSize(encoding);

            if (index < 0 || buffer.Length < index + requireLength)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (length < requireLength)
                throw new ArgumentOutOfRangeException(nameof(length), $"バッファの長さが足りません。少なくとも {requireLength} の長さが必要です。");
            
            this.ExportBuffer(buffer, index, encoding);

            return requireLength;
        }
        
        public void Export(Stream stream, Encoding encoding)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));

            if (!stream.CanWrite)
                throw new InvalidOperationException("書き込みできないストリームが指定されました.");

            stream.Write(marker, 0, marker.Length);

            if (encoding == Encoding.UTF8)
                stream.Write(preamble, 0, preamble.Length);
            
            foreach (var item in this)
            {
                var buf = encoding.GetBytes(string.Format("{0}={1}\n\0", item.Key.ToLower(), item.Value));
                stream.Write(buf, 0, buf.Length);
            }
        }

        #endregion

        #region -- Private Methods --

        private int GetBufferOutputSize(Encoding encoding)
        {
            int count = this.Sum(p => encoding.GetByteCount(p.Key) + encoding.GetByteCount(p.Value) + 3);

            if (encoding == Encoding.UTF8)
                count += preamble.Length;

            return count + marker.Length;
        }

        private void ExportBuffer(byte[] buffer, int index, Encoding encoding)
        {
            Array.Copy(marker, 0, buffer, index, marker.Length);
            index += marker.Length;

            if (encoding == Encoding.UTF8)
            {
                Array.Copy(preamble, 0, buffer, index, preamble.Length);
                index += preamble.Length;
            }

            foreach (var item in this)
            {
                var str = string.Format("{0}={1}\n\0", item.Key.ToLower(), item.Value);
                var strLength = encoding.GetBytes(str, 0, str.Length, buffer, index);
                index += strLength;
            }
        }

        private void Import(byte[] import, Encoding encoding, int index)
        {
            int currentIndex = index;

            while (currentIndex < import.Length)
            {
                string key = null;
                string value = null;

                // key
                for (int i = currentIndex; i < import.Length; i++)
                {
                    if (import[i] == 0x3d)
                    {
                        key = encoding.GetString(import, currentIndex, i - currentIndex);
                        i++;
                        currentIndex = i;
                        break;
                    }
                }

                // value
                for (int i = currentIndex; i < import.Length; i++)
                {
                    if (import[i] == 0x0a)
                    {
                        value = encoding.GetString(import, currentIndex, i - currentIndex);
                        i++;
                        currentIndex = i;
                        break;
                    }
                }

                if (currentIndex < import.Length && import[currentIndex] == 0x00)
                    currentIndex++;

                if (key == null || value == null)
                    throw new InvalidDataException();

                this.Add(key, value);
            }
        }

        #endregion

        #region -- Private Static Methods --

        private static void CheckKeyValue(string key, string value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (key.Length == 0)
                throw new ArgumentException($"パラメータ '{nameof(key)}' に空の文字列を指定することはできません.");

            if (key.Contains("\n"))
                throw new ArgumentException($"パラメータ '{nameof(key)}' に改行文字 0x0A を含むことはできません.");

            if (key.Contains("="))
                throw new ArgumentException($"パラメータ '{nameof(key)}' に文字 '=' を含むことはできません.");

            if (value.Contains("\n"))
                throw new ArgumentException($"パラメータ '{nameof(value)}' に改行文字 0x0A を含むことはできません.");
        }

        private static void CheckMarker(byte[] import)
        {
            if (import == null)
                throw new ArgumentNullException(nameof(import));

            if (import.Length < marker.Length)
                throw new ArgumentException("タグ識別子が存在しません.", nameof(import));

            for (int i = 0; i < marker.Length; i++)
                if (import[i] != marker[i])
                    throw new ArgumentException("タグ識別子が存在しません.", nameof(import));
        }

        private static bool IsEncodedByUTF8(byte[] import)
        {
            // this condition is always evaluated as "false"
            // if (import.Length < marker.Length + preamble.Length)
            //    return false;

            for (int i = marker.Length, j = 0; j < preamble.Length; i++, j++)
                if (import[i] != preamble[j])
                    return false;

            return true;
        }

        #endregion
    }
}
