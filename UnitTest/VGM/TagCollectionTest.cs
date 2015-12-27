using System;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sharp98.VGM;

namespace UnitTest.VGM
{
    [TestClass]
    public class TagCollectionTest
    {
        private static readonly Encoding sjis = Encoding.GetEncoding("Shift_JIS");

        [TestMethod]
        public void ConstructorTest1()
        {
            var tag = new TagCollection();
            Assert.AreEqual(11, tag.Count);
        }

        [TestMethod]
        public void ConstructorTest2()
        {
            var tag_base = new TagCollection();
            tag_base.TrackNameEnglish = "foo";
            tag_base.GameNameEnglish = "bar";

            var new_tag = new TagCollection(tag_base);
            Assert.AreEqual(11, new_tag.Count);
            Assert.AreEqual("foo", new_tag.TrackNameEnglish);
            Assert.AreEqual("bar", new_tag.GameNameEnglish);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorError1()
        {
            var tag_base = new TagCollection(null);
        }
        
        [TestMethod]
        public void IndexerTest()
        {
            var tag_base = new TagCollection();
            tag_base["00_TrackName_English"] = "00";
            tag_base["01_TrackName_Japanese"] = "01";
            tag_base["02_GameName_English"] = "02";
            tag_base["03_GameName_Japanese"] = "03";
            tag_base["04_SystemName_English"] = "04";
            tag_base["05_SystemName_Japanese"] = "05";
            tag_base["06_OriginalTrackAuthor_English"] = "06";
            tag_base["07_OriginalTrackAuthor_Japanese"] = "07";
            tag_base["08_ReleaseDate"] = "08";
            tag_base["09_PersonWhoConverted"] = "09";
            tag_base["10_Notes"] = "10";

            Assert.AreEqual("00", tag_base.TrackNameEnglish);
            Assert.AreEqual("01", tag_base.TrackNameJapanese);
            Assert.AreEqual("02", tag_base.GameNameEnglish);
            Assert.AreEqual("03", tag_base.GameNameJapanese);
            Assert.AreEqual("04", tag_base.SystemNameEnglish);
            Assert.AreEqual("05", tag_base.SystemNameJapanese);
            Assert.AreEqual("06", tag_base.OriginalTrackAuthorEnglish);
            Assert.AreEqual("07", tag_base.OriginalTrackAuthorJapanese);
            Assert.AreEqual("08", tag_base.ReleaseDate);
            Assert.AreEqual("09", tag_base.PersonWhoConverted);
            Assert.AreEqual("10", tag_base.Notes);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void AddError1()
        {
            var tag_base = new TagCollection();
            tag_base.Add("foo", "bar");
        }

        //[TestMethod]
        //[ExpectedException(typeof(ArgumentNullException))]
        //public void Export1Error1()
        //{
        //    var tag_base = new TagCollection();
        //    tag_base.Export(null);
        //}

        //[TestMethod]
        //public void Export2Test()
        //{
        //    var buffer = new byte[100];
        //    var tag_base = new TagCollection();
        //    tag_base.TrackNameEnglish = "foo";
        //    tag_base.GameNameEnglish = "bar";
        //    tag_base.Export(buffer, 0, 100, Encoding.UTF8);
        //}

        //[TestMethod]
        //[ExpectedException(typeof(ArgumentNullException))]
        //public void Export2Error1()
        //{
        //    var tag_base = new TagCollection();
        //    tag_base.Export(null, 0, 0, Encoding.UTF8);
        //}

        //[TestMethod]
        //[ExpectedException(typeof(ArgumentNullException))]
        //public void Export2Error2()
        //{
        //    var buffer = new byte[0];
        //    var tag_base = new TagCollection();
        //    tag_base.Export(buffer, 0, 0, null);
        //}

        //[TestMethod]
        //[ExpectedException(typeof(ArgumentOutOfRangeException))]
        //public void Export2Error3()
        //{
        //    var buffer = new byte[0];
        //    var tag_base = new TagCollection();
        //    tag_base.Export(buffer, -1, 0, Encoding.UTF8);
        //}

        //[TestMethod]
        //[ExpectedException(typeof(ArgumentOutOfRangeException))]
        //public void Export2Error4()
        //{
        //    var buffer = new byte[0];
        //    var tag_base = new TagCollection();
        //    tag_base.Export(buffer, 0, 42, Encoding.UTF8);
        //}

        //[TestMethod]
        //[ExpectedException(typeof(ArgumentOutOfRangeException))]
        //public void Export2Error5()
        //{
        //    var buffer = new byte[8];
        //    var tag_base = new TagCollection();
        //    tag_base.TrackNameEnglish = "foo";
        //    tag_base.GameNameEnglish = "bar";
        //    tag_base.Export(buffer, 0, 8, Encoding.UTF8);
        //}

        //[TestMethod]
        //[ExpectedException(typeof(ArgumentOutOfRangeException))]
        //public void Export2Error6()
        //{
        //    var buffer = new byte[100];
        //    var tag_base = new TagCollection();
        //    tag_base.TrackNameEnglish = "foo";
        //    tag_base.GameNameEnglish = "bar";
        //    tag_base.Export(buffer, 0, 8, Encoding.UTF8);
        //}

        [TestMethod]
        public void Export3Test()
        {
            using (var ms = new MemoryStream())
            {
                var tag_base = new TagCollection();
                tag_base.TrackNameEnglish = "foo";
                tag_base.GameNameEnglish = "bar";
                tag_base.Export(ms, Encoding.UTF8);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Export3Error1()
        {
            var tag_base = new TagCollection();
            tag_base.TrackNameEnglish = "foo";
            tag_base.GameNameEnglish = "bar";
            tag_base.Export(null, Encoding.UTF8);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Export3Error3()
        {
            using (var ms = new MemoryStream(new byte[256], false))
            {
                var tag_base = new TagCollection();
                tag_base.TrackNameEnglish = "foo";
                tag_base.GameNameEnglish = "bar";
                tag_base.Export(ms, Encoding.UTF8);
            }
        }
    }
}
