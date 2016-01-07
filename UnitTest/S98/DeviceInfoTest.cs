using System;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sharp98.S98;

namespace UnitTest.S98
{
    [TestClass]
    public class DeviceInfoTest
    {
        const Sharp98.DeviceType type = Sharp98.DeviceType.OPNA;
        const Sharp98.S98.DeviceType s98type = Sharp98.S98.DeviceType.OPNA;
        const int clock = 8000000;
        const PanFlag pan = PanFlag.Stereo;

        [TestMethod]
        public void ConstructorTest1()
        {
            var device = new DeviceInfo();
            Assert.AreEqual(Sharp98.DeviceType.None, device.DeviceType);
            Assert.AreEqual(0, device.Clock);
            Assert.AreEqual(PanFlag.Stereo, device.Pan);
            Assert.AreEqual(Sharp98.S98.DeviceType.None, device.S98DeviceType);
        }

        [TestMethod]
        public void ConstructorTest2()
        {
            var device = new DeviceInfo(s98type, clock, pan);
            Assert.AreEqual(type, device.DeviceType);
            Assert.AreEqual(clock, device.Clock);
            Assert.AreEqual(pan, device.Pan);
            Assert.AreEqual(s98type, device.S98DeviceType);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ConstructorError1()
        {
            var device = new DeviceInfo(unchecked((Sharp98.S98.DeviceType)(-1)), clock, pan);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ConstructorError2()
        {
            var device = new DeviceInfo(s98type, 0, pan);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ConstructorError3()
        {
            var device = new DeviceInfo(s98type, int.MaxValue + 1u, pan);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ConstructorError4()
        {
            var device = new DeviceInfo(s98type, clock, unchecked((PanFlag)(-1)));
        }

        [TestMethod]
        public void Export1Test()
        {
            var device = new DeviceInfo(s98type, clock, pan);

            var output1 = device.Export(Encoding.UTF8);
            var output2 = device.Export(null);

            CollectionAssert.AreEqual(output1, output2);
            Assert.IsNotNull(output1);
            Assert.AreEqual(16, output1.Length);
        }

        [TestMethod]
        public void Export2Test()
        {
            var device = new DeviceInfo(s98type, clock, pan);
            var buffer = new byte[16];
            var size = device.Export(buffer, 0, 16, null);

            Assert.AreEqual(16, size);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Export2Error1()
        {
            var device = new DeviceInfo(s98type, clock, pan);
            var size = device.Export(null, 0, 16, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Export2Error2()
        {
            var device = new DeviceInfo(s98type, clock, pan);
            var buffer = new byte[16];
            var size = device.Export(buffer, -1, 16, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Export2Error3()
        {
            var device = new DeviceInfo(s98type, clock, pan);
            var buffer = new byte[16];
            var size = device.Export(buffer, 16, 16, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Export2Error4()
        {
            var device = new DeviceInfo(s98type, clock, pan);
            var buffer = new byte[16];
            var size = device.Export(buffer, 0, 8, null);
        }

        [TestMethod]
        public void Export3Test()
        {
            var device = new DeviceInfo(s98type, clock, pan);
            byte[] buffer;

            using (var ms = new MemoryStream())
            {
                device.Export(ms, null);
                ms.Flush();
                buffer = ms.ToArray();
            }

            Assert.AreEqual(16, buffer.Length);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Export3Error1()
        {
            var device = new DeviceInfo(s98type, clock, pan);
            device.Export(null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Export3Error2()
        {
            var device = new DeviceInfo(s98type, clock, pan);
            byte[] buffer = new byte[16];

            using (var ms = new MemoryStream(buffer, false))
            {
                device.Export(ms, null);
            }
        }

        [TestMethod]
        public void Import1Test()
        {
            var device = new DeviceInfo(s98type, clock, pan);
            byte[] buffer = device.Export(null);
            var new_device = DeviceInfo.Import(buffer);

            Assert.AreEqual(type, new_device.DeviceType);
            Assert.AreEqual(clock, new_device.Clock);
            Assert.AreEqual(pan, new_device.Pan);
            Assert.AreEqual(s98type, new_device.S98DeviceType);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Import1Error1()
        {
            var new_device = DeviceInfo.Import(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Import1Error2()
        {
            var device = new DeviceInfo(s98type, clock, pan);
            byte[] buffer = device.Export(null);
            var new_device = DeviceInfo.Import(buffer, -1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Import1Error3()
        {
            var device = new DeviceInfo(s98type, clock, pan);
            byte[] buffer = device.Export(null);
            var new_device = DeviceInfo.Import(buffer, 16);
        }
    }
}
