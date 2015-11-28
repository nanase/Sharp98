using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sharp98.S98;

namespace UnitTest
{
    [TestClass]
    public class TagCollectionTest
    {
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

            var tag_buffer = tag_base.Export();
            var new_tag = new TagCollection(tag_buffer);
            Assert.AreEqual(2, new_tag.Count);
            Assert.AreEqual("foo", new_tag["title"]);
            Assert.AreEqual("bar", new_tag["name"]);

            var sjis = Encoding.GetEncoding("Shift_JIS");
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
    }
}
