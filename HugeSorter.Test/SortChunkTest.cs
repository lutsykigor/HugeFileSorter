namespace HugeSorter.Test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Stubs;
    using System;
    using TextSorted;

    [TestClass]
    public class SortChunkTest
    {
        private FileWriterStub writer;
        private FileReaderStub reader;
        [TestInitialize]
        public void Init()
        {
            string[] data = { string.Empty };
            writer = new FileWriterStub();
            reader = new FileReaderStub(data);
        }

        [TestMethod]
        public void ShouldFlushIfBufferIsFull()
        {
            var chunk = new SortChunk("a", "path", new FileAccessStub(reader, writer, 10), 3);
            chunk.Add("test 1");
            chunk.Add("test 2");

            Assert.IsTrue(writer.WriteCounter == 0);

            chunk.Add("test 3");

            Assert.IsTrue(writer.WriteCounter > 0);
        }

        [TestMethod]
        public void ShouldClearBufferAfterFlush()
        {
            var chunk = new SortChunk("a", "path", new FileAccessStub(reader, writer, 10), 2);
            chunk.Add("test 1");

            Assert.IsTrue(chunk.Buffered == 1);

            chunk.Add("test 2");

            Assert.IsTrue(chunk.Buffered == 0);
        }
    }
}
