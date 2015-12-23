using System;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sharp98.S98;

namespace UnitTest.S98
{
    [TestClass]
    public class TagCollectionTest
    {
        private static readonly Encoding sjis = Encoding.GetEncoding("Shift_JIS");

        [TestMethod]
        public void ConstructorTest1()
        {
            var tag = new TagCollection();
            Assert.AreEqual(0, tag.Count);
        }

        [TestMethod]
        public void ConstructorTest2()
        {
            var tag_base = new TagCollection();
            tag_base.Add("title", "foo");
            tag_base.Add("name", "bar");

            var tag_buffer = tag_base.Export(Encoding.UTF8);
            var new_tag = new TagCollection(tag_buffer);
            Assert.AreEqual(2, new_tag.Count);
            Assert.AreEqual("foo", new_tag["title"]);
            Assert.AreEqual("bar", new_tag["name"]);

            var tag_buffer_sjis = tag_base.Export(sjis);
            var new_tag_sjis = new TagCollection(tag_buffer, sjis);
            Assert.AreEqual(2, new_tag.Count);
            Assert.AreEqual("foo", new_tag["title"]);
            Assert.AreEqual("bar", new_tag["name"]);
        }

        [TestMethod]
        public void ConstructorTest3()
        {
            var tag_base = new TagCollection();
            tag_base.Add("title", "foo");
            tag_base.Add("name", "bar");

            var new_tag = new TagCollection(tag_base);
            Assert.AreEqual(2, new_tag.Count);
            Assert.AreEqual("foo", new_tag["title"]);
            Assert.AreEqual("bar", new_tag["name"]);
        }

        [TestMethod]
        public void ConstructorTest4()
        {
            var tag_base = new TagCollection();
            tag_base.Add("title", "foo");
            tag_base.Add("name", "bar");
            var buffer = tag_base.Export(sjis);

            var new_tag = new TagCollection(buffer);
            Assert.AreEqual(2, new_tag.Count);
            Assert.AreEqual("foo", new_tag["title"]);
            Assert.AreEqual("bar", new_tag["name"]);
        }

        [TestMethod]
        public void ConstructorTest5()
        {
            var tag_base = new TagCollection();
            tag_base.Add("title", "foo");
            tag_base.Add("name", "bar");
            var buffer = tag_base.Export(sjis);

            var new_tag = new TagCollection(buffer, sjis);
            Assert.AreEqual(2, new_tag.Count);
            Assert.AreEqual("foo", new_tag["title"]);
            Assert.AreEqual("bar", new_tag["name"]);
        }

        [TestMethod]
        public void ConstructorTest6()
        {
            var tag_base = new TagCollection();
            tag_base.Add("title", "foo");
            tag_base.Add("name", "bar");
            var buffer = tag_base.Export(Encoding.UTF8);

            var new_tag = new TagCollection(buffer);
            Assert.AreEqual(2, new_tag.Count);
            Assert.AreEqual("foo", new_tag["title"]);
            Assert.AreEqual("bar", new_tag["name"]);
        }

        [TestMethod]
        public void ConstructorTest7()
        {
            var tag_base = new TagCollection();
            tag_base.Add("title", "foo");
            tag_base.Add("name", "bar");
            var buffer = tag_base.Export(Encoding.UTF8);

            var new_tag = new TagCollection(buffer, Encoding.UTF8);
            Assert.AreEqual(2, new_tag.Count);
            Assert.AreEqual("foo", new_tag["title"]);
            Assert.AreEqual("bar", new_tag["name"]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorError1()
        {
            var tag_base = new TagCollection((byte[])null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConstructorError2()
        {
            var tag_base = new TagCollection(new byte[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConstructorError3()
        {
            var tag_base = new TagCollection(new byte[10]);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void ConstructorError4()
        {
            const string testString = "[S98]\n\0";
            var tag_base = new TagCollection(Encoding.ASCII.GetBytes(testString));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void ConstructorError5()
        {
            const string testString = "[S98]aaa=";
            var tag_base = new TagCollection(Encoding.ASCII.GetBytes(testString));
        }

        [TestMethod]
        public void IndexerTest()
        {
            var tag_base = new TagCollection();
            tag_base["title"] = "foo";
            tag_base["name"] = "bar";

            Assert.AreEqual(2, tag_base.Count);
            Assert.AreEqual("foo", tag_base["title"]);
            Assert.AreEqual("bar", tag_base["name"]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddError1()
        {
            var tag_base = new TagCollection();
            tag_base.Add(null, "foo");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddError2()
        {
            var tag_base = new TagCollection();
            tag_base.Add("title", null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddError3()
        {
            var tag_base = new TagCollection();
            tag_base.Add(string.Empty, "foo");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddError4()
        {
            var tag_base = new TagCollection();
            tag_base.Add("title\n", "foo");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddError5()
        {
            var tag_base = new TagCollection();
            tag_base.Add("title=", "foo");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddError6()
        {
            var tag_base = new TagCollection();
            tag_base.Add("title", "foo\n");
        }

        [TestMethod]
        public void Export1Test()
        {
            var tag_base = new TagCollection();
            tag_base.Add("title", "foo");
            tag_base.Add("name", "bar");
            var buffer = tag_base.Export(Encoding.UTF8);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Export1Error1()
        {
            var tag_base = new TagCollection();
            tag_base.Export(null);
        }

        [TestMethod]
        public void Export2Test()
        {
            var buffer = new byte[100];
            var tag_base = new TagCollection();
            tag_base.Add("title", "foo");
            tag_base.Add("name", "bar");
            tag_base.Export(buffer, 0, 100, Encoding.UTF8);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Export2Error1()
        {
            var tag_base = new TagCollection();
            tag_base.Export(null, 0, 0, Encoding.UTF8);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Export2Error2()
        {
            var buffer = new byte[0];
            var tag_base = new TagCollection();
            tag_base.Export(buffer, 0, 0, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Export2Error3()
        {
            var buffer = new byte[0];
            var tag_base = new TagCollection();
            tag_base.Export(buffer, -1, 0, Encoding.UTF8);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Export2Error4()
        {
            var buffer = new byte[0];
            var tag_base = new TagCollection();
            tag_base.Export(buffer, 0, 42, Encoding.UTF8);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Export2Error5()
        {
            var buffer = new byte[8];
            var tag_base = new TagCollection();
            tag_base.Add("title", "foo");
            tag_base.Add("name", "bar");
            tag_base.Export(buffer, 0, 8, Encoding.UTF8);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Export2Error6()
        {
            var buffer = new byte[100];
            var tag_base = new TagCollection();
            tag_base.Add("title", "foo");
            tag_base.Add("name", "bar");
            tag_base.Export(buffer, 0, 8, Encoding.UTF8);
        }

        [TestMethod]
        public void Export3Test()
        {
            using (var ms = new MemoryStream())
            {
                var tag_base = new TagCollection();
                tag_base.Add("title", "foo");
                tag_base.Add("name", "bar");
                tag_base.Export(ms, Encoding.UTF8);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Export3Error1()
        {
            var tag_base = new TagCollection();
            tag_base.Add("title", "foo");
            tag_base.Add("name", "bar");
            tag_base.Export(null, Encoding.UTF8);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Export3Error2()
        {
            using (var ms = new MemoryStream())
            {
                var tag_base = new TagCollection();
                tag_base.Add("title", "foo");
                tag_base.Add("name", "bar");
                tag_base.Export(ms, null);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Export3Error3()
        {
            using (var ms = new MemoryStream(new byte[256], false))
            {
                var tag_base = new TagCollection();
                tag_base.Add("title", "foo");
                tag_base.Add("name", "bar");
                tag_base.Export(ms, Encoding.UTF8);
            }
        }
    }
}
