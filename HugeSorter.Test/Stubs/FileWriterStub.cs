namespace HugeSorter.Test.Stubs
{
    using System;
    using TextSorted.Interfaces;
    public class FileWriterStub : IFileWriter
    {
        private long writeCounter;
        public string WrittenData { get; set; }
        public long WriteCounter { get { return writeCounter; } }
        public void Write(string value)
        {
            this.writeCounter += value.Length;
            WrittenData = string.Concat(WrittenData, value, Environment.NewLine);
        }
        public void Dispose()
        {
        }
    }
}
