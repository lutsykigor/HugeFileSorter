namespace HugeSorter.Test.Stubs
{
    using System.Collections.Generic;
    using TextSorted.Interfaces;
    public class FileAccessStub : IFileAccess
    {
        private FileReaderStub reader;
        private FileWriterStub writer;
        private List<string[]> readSets;
        private int size;

        public int WriterRequests { get; set; }
        public int ReaderRequests { get; set; }

        public FileAccessStub(FileReaderStub reader, FileWriterStub writer, int size)
        {
            this.writer = writer;
            this.reader = reader;
            this.size = size;
        }

        public FileAccessStub(List<string[]> readSets, FileWriterStub writer, int size)
        {
            this.writer = writer;
            this.readSets = readSets;
            this.size = size;
        }

        public long GetFileSize(string path)
        {
            return this.size;
        }

        public IFileReader GetReader(string path)
        {
            if (this.reader != null)
            {
                return this.reader;
            }
            else
            {
                if (this.readSets.Count == ReaderRequests)
                {
                    return new FileReaderStub(this.readSets[0]);
                }
                return new FileReaderStub(readSets[ReaderRequests++]);
            }
        }
        public IFileWriter GetWriter(string path, bool append)
        {
            WriterRequests++;
            return this.writer;
        }

        public void Delete(string path)
        {

        }
    }
}
