//
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

namespace Sharp98.VGM
{
    public class TagCollection : SortedDictionary<string, string>, ITagCollection
    {
        #region -- Private Fields --

        private static readonly byte[] marker = new byte[] { 0x47, 0x64, 0x33, 0x20, 0x00, 0x01, 0x00, 0x00 };

        private const string key_trackNameEnglish = "00_TrackName_English";
        private const string key_trackNameJapanese = "01_TrackName_Japanese";
        private const string key_gameNameEnglish = "02_GameName_English";
        private const string key_gameNameJapanese = "03_GameName_Japanese";
        private const string key_systemNameEnglish = "04_SystemName_English";
        private const string key_systemNameJapanese = "05_SystemName_Japanese";
        private const string key_originalTrackAuthorEnglish = "06_OriginalTrackAuthor_English";
        private const string key_originalTrackAuthorJapanese = "07_OriginalTrackAuthor_Japanese";
        private const string key_releaseDate = "08_ReleaseDate";
        private const string key_personWhoConverted = "09_PersonWhoConverted";
        private const string key_notes = "10_Notes";

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
                base[key] = value;
            }
        }

        public string TrackNameEnglish
        {
            get { return this[key_trackNameEnglish]; }
            set { this[key_trackNameEnglish] = value; }
        }

        public string TrackNameJapanese
        {
            get { return this[key_trackNameJapanese]; }
            set { this[key_trackNameJapanese] = value; }
        }

        public string GameNameEnglish
        {
            get { return this[key_gameNameEnglish]; }
            set { this[key_gameNameEnglish] = value; }
        }

        public string GameNameJapanese
        {
            get { return this[key_gameNameJapanese]; }
            set { this[key_gameNameJapanese] = value; }
        }

        public string SystemNameEnglish
        {
            get { return this[key_systemNameEnglish]; }
            set { this[key_systemNameEnglish] = value; }
        }

        public string SystemNameJapanese
        {
            get { return this[key_systemNameJapanese]; }
            set { this[key_systemNameJapanese] = value; }
        }

        public string OriginalTrackAuthorEnglish
        {
            get { return this[key_originalTrackAuthorEnglish]; }
            set { this[key_originalTrackAuthorEnglish] = value; }
        }

        public string OriginalTrackAuthorJapanese
        {
            get { return this[key_originalTrackAuthorJapanese]; }
            set { this[key_originalTrackAuthorJapanese] = value; }
        }

        public string ReleaseDate
        {
            get { return this[key_releaseDate]; }
            set { this[key_releaseDate] = value; }
        }

        public string PersonWhoConverted
        {
            get { return this[key_personWhoConverted]; }
            set { this[key_personWhoConverted] = value; }
        }

        public string Notes
        {
            get { return this[key_notes]; }
            set { this[key_notes] = value; }
        }

        #endregion

        #region -- Constructors --

        public TagCollection()
            : base()
        {
            this.AddDefaultKeys();
        }

        public TagCollection(IDictionary<string, string> dictionary)
            : this()
        {
            foreach (var item in dictionary)
                this[item.Key] = item.Value;
        }

        #endregion

        #region -- Public Methods --

        public new void Add(string key, string value)
        {
            throw new NotSupportedException();
        }
        
        public void Export(Stream stream, Encoding encoding)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (!stream.CanWrite)
                throw new InvalidOperationException("書き込みできないストリームが指定されました.");
            
            int count = this.Values.Sum(v => Encoding.Unicode.GetByteCount(v) + 2);
            var buffer = new byte[count + 4];
            int index = 4;

            ((uint)count).GetLEByte(buffer, 0);

            foreach (var value in this.Values)
            {
                int length = Encoding.Unicode.GetBytes(value, 0, value.Length, buffer, index);
                index += length + 2;
            }

            stream.Write(marker, 0, marker.Length);
            stream.Write(buffer, 0, count + 4);
        }

        public byte[] Export(Encoding encoding)
        {
            throw new NotImplementedException();
        }

        public int Export(byte[] buffer, int index, int length, Encoding encoding)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region -- Public Static Methods --

        public static TagCollection Import(byte[] array, int index)
        {
            var tag = new TagCollection();

            CheckMarker(array);
            tag.ImportFromArray(array, index + marker.Length + 4);

            return tag;
        }

        #endregion

        #region -- Private Methods --

        private void AddDefaultKeys()
        {
            base.Add(key_trackNameEnglish, string.Empty);
            base.Add(key_trackNameJapanese, string.Empty);
            base.Add(key_gameNameEnglish, string.Empty);
            base.Add(key_gameNameJapanese, string.Empty);
            base.Add(key_systemNameEnglish, string.Empty);
            base.Add(key_systemNameJapanese, string.Empty);
            base.Add(key_originalTrackAuthorEnglish, string.Empty);
            base.Add(key_originalTrackAuthorJapanese, string.Empty);
            base.Add(key_releaseDate, string.Empty);
            base.Add(key_personWhoConverted, string.Empty);
            base.Add(key_notes, string.Empty);
        }

        private void ImportFromArray(byte[] import, int index)
        {
            string[] strs = new string[11];

            for (int i = 0; i < 11; i++)
            {
                int currentIndex = index;
                int length = 0;

                while (currentIndex + length < import.Length - 1)
                {
                    if (import[currentIndex + length] == 0 &&
                        import[currentIndex + length + 1] == 0)
                    {
                        strs[i] = Encoding.Unicode.GetString(import, currentIndex, length);
                        index = currentIndex + length + 2;
                        break;
                    }

                    length += 2;
                }
            }

            this[key_trackNameEnglish] = strs[0];
            this[key_trackNameJapanese] = strs[1];
            this[key_gameNameEnglish] = strs[2];
            this[key_gameNameJapanese] = strs[3];
            this[key_systemNameEnglish] = strs[4];
            this[key_systemNameJapanese] = strs[5];
            this[key_originalTrackAuthorEnglish] = strs[6];
            this[key_originalTrackAuthorJapanese] = strs[7];
            this[key_releaseDate] = strs[8];
            this[key_personWhoConverted] = strs[9];
            this[key_notes] = strs[10];
        }

        private void CheckKeyValue(string key, string value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (key.Length == 0)
                throw new ArgumentException($"パラメータ '{nameof(key)}' に空の文字列を指定することはできません.");

            if (!this.ContainsKey(key))
                throw new ArgumentOutOfRangeException(nameof(key));

            if (value.Contains("\0"))
                throw new ArgumentException($"パラメータ '{nameof(value)}' に文字 '\0' を含むことはできません.");
        }

        #endregion

        #region -- Private Static Methods --

        private static void CheckMarker(byte[] import)
        {
            if (import == null)
                throw new ArgumentNullException(nameof(import));

            if (import.Length < marker.Length)
                throw new ArgumentException("タグ識別子が存在しません.", nameof(import));

            for (int i = 0; i < marker.Length; i++)
                if (import[i] != marker[i])
                    throw new ArgumentException("タグ識別子が存在しません.", nameof(import));

            uint length = import.GetLEUInt32(marker.Length);

            if (import.Length < marker.Length + 4 + length)
                throw new ArgumentException("長さが不正です.", nameof(import));
        }

        #endregion
    }
}
