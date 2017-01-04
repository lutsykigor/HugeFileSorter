namespace HugeSorter.Test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Stubs;
    using System;
    using System.Collections.Generic;
    using TextSorted;
    [TestClass]
    public class FileSorterTest
    {
        private FileWriterStub writer;
        private FileReaderStub reader;
        private FileSorter sorter;

        [TestMethod]
        public void ShouldSortTextFile()
        {
            string[] sortData = {
                "bester",
                "abc",
                "come phase here",
                "almost end",
                "bast phase" };

            writer = new FileWriterStub();
            reader = new FileReaderStub(sortData);
            sorter = new FileSorter(new FileAccessStub(reader, writer, 40),
                StringComparer.Ordinal, 10, "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ ".ToCharArray());

            sorter.Sort("c:\\test.txt", "c:\\testDest.txt");

            string[] expectedResult = {
                "abc",
                "almost end",
                "bast phase",
                "bester",
                "come phase here"
                };

            Assert.IsTrue(writer.WrittenData.IndexOf(string.Join(Environment.NewLine, expectedResult)) > 0);
        }

        [TestMethod]
        public void ShouldSplitChunksIfFull()
        {
            string[] sortData = {
                "bester",
                "abc",
                "come phase here",
                "almost end",
                "bast phase",
                "best phase",
                "bist phase",
                "bost phase"};

            string[] chunkRead = {
                "bester",
                "bast phase"
            };

            List<string[]> readData = new List<string[]> { sortData, chunkRead };
            writer = new FileWriterStub();
            reader = new FileReaderStub(sortData);
            var fileAccess = new FileAccessStub(readData, writer, 40);
            sorter = new FileSorter(fileAccess, StringComparer.Ordinal,
                20, "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ ".ToCharArray());

            sorter.Sort("c:\\test.txt", "c:\\testDest.txt");

            Assert.IsTrue(fileAccess.WriterRequests == 16);
        }
    }
}
