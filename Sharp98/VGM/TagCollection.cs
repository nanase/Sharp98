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
        #region -- Public Fields --

        public readonly string KeyTrackNameEnglish = "00_TrackName_English";
        public readonly string KeyTrackNameJapanese = "01_TrackName_Japanese";
        public readonly string KeyGameNameEnglish = "02_GameName_English";
        public readonly string KeyGameNameJapanese = "03_GameName_Japanese";
        public readonly string KeySystemNameEnglish = "04_SystemName_English";
        public readonly string KeySystemNameJapanese = "05_SystemName_Japanese";
        public readonly string KeyOriginalTrackAuthorEnglish = "06_OriginalTrackAuthor_English";
        public readonly string KeyOriginalTrackAuthorJapanese = "07_OriginalTrackAuthor_Japanese";
        public readonly string KeyReleaseDate = "08_ReleaseDate";
        public readonly string KeyPersonWhoConverted = "09_PersonWhoConverted";
        public readonly string KeyNotes = "10_Notes";

        #endregion

        #region -- Private Fields --

        private static readonly byte[] marker = new byte[] { 0x47, 0x64, 0x33, 0x20, 0x00, 0x01, 0x00, 0x00 };

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
            get { return this[KeyTrackNameEnglish]; }
            set { this[KeyTrackNameEnglish] = value; }
        }

        public string TrackNameJapanese
        {
            get { return this[KeyTrackNameJapanese]; }
            set { this[KeyTrackNameJapanese] = value; }
        }

        public string GameNameEnglish
        {
            get { return this[KeyGameNameEnglish]; }
            set { this[KeyGameNameEnglish] = value; }
        }

        public string GameNameJapanese
        {
            get { return this[KeyGameNameJapanese]; }
            set { this[KeyGameNameJapanese] = value; }
        }

        public string SystemNameEnglish
        {
            get { return this[KeySystemNameEnglish]; }
            set { this[KeySystemNameEnglish] = value; }
        }

        public string SystemNameJapanese
        {
            get { return this[KeySystemNameJapanese]; }
            set { this[KeySystemNameJapanese] = value; }
        }

        public string OriginalTrackAuthorEnglish
        {
            get { return this[KeyOriginalTrackAuthorEnglish]; }
            set { this[KeyOriginalTrackAuthorEnglish] = value; }
        }

        public string OriginalTrackAuthorJapanese
        {
            get { return this[KeyOriginalTrackAuthorJapanese]; }
            set { this[KeyOriginalTrackAuthorJapanese] = value; }
        }

        public string ReleaseDate
        {
            get { return this[KeyReleaseDate]; }
            set { this[KeyReleaseDate] = value; }
        }

        public string PersonWhoConverted
        {
            get { return this[KeyPersonWhoConverted]; }
            set { this[KeyPersonWhoConverted] = value; }
        }

        public string Notes
        {
            get { return this[KeyNotes]; }
            set { this[KeyNotes] = value; }
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
            base.Add(KeyTrackNameEnglish, string.Empty);
            base.Add(KeyTrackNameJapanese, string.Empty);
            base.Add(KeyGameNameEnglish, string.Empty);
            base.Add(KeyGameNameJapanese, string.Empty);
            base.Add(KeySystemNameEnglish, string.Empty);
            base.Add(KeySystemNameJapanese, string.Empty);
            base.Add(KeyOriginalTrackAuthorEnglish, string.Empty);
            base.Add(KeyOriginalTrackAuthorJapanese, string.Empty);
            base.Add(KeyReleaseDate, string.Empty);
            base.Add(KeyPersonWhoConverted, string.Empty);
            base.Add(KeyNotes, string.Empty);
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

            this[KeyTrackNameEnglish] = strs[0];
            this[KeyTrackNameJapanese] = strs[1];
            this[KeyGameNameEnglish] = strs[2];
            this[KeyGameNameJapanese] = strs[3];
            this[KeySystemNameEnglish] = strs[4];
            this[KeySystemNameJapanese] = strs[5];
            this[KeyOriginalTrackAuthorEnglish] = strs[6];
            this[KeyOriginalTrackAuthorJapanese] = strs[7];
            this[KeyReleaseDate] = strs[8];
            this[KeyPersonWhoConverted] = strs[9];
            this[KeyNotes] = strs[10];
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
