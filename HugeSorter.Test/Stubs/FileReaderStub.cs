namespace HugeSorter.Test.Stubs
{
    using System;
    using TextSorted.Interfaces;
    public class FileReaderStub : IFileReader
    {
        private string[] data;
        private int currentPosition;
        public FileReaderStub(string[] data)
        {
            this.data = data;
            this.currentPosition = 0;
        }
        public bool EndOfStream
        {
            get
            {
                return currentPosition == data.Length;
            }
        }

        public void Dispose()
        {
            
        }

        public string ReadLine()
        {
            return data[this.currentPosition++];
        }

        public string ReadToEnd()
        {
            this.currentPosition = data.Length;
            return string.Join(Environment.NewLine, this.data);
        }
    }
}
