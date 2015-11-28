using System;
using System.IO;
using NUnit.Framework;

namespace common.tests
{
    [TestFixture]
    public class StreamExtensionsTests
    {
        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void TestExceptionOnNonASCII()
        {
            var stream = new MemoryStream();
            stream.WriteASCII("String with non ascii character: \x058E");
        }

        [Test]
        public void TestWrite()
        {
            var stream = new MemoryStream();
            stream.WriteASCII("\x00\x01\x02\x03");

            Assert.That(stream.ToArray(), Is.EqualTo(new byte[] {0,1,2,3}));
        }
    }
}
