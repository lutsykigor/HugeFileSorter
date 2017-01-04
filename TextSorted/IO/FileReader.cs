namespace TextSorted.IO
{
    using Interfaces;
    using System.IO;

    /// <summary>
    /// File input proxy
    /// </summary>
    public class FileReader : IFileReader
    {
        private StreamReader reader;
        public FileReader(StreamReader reader)
        {
            this.reader = reader;
        }

        /// <summary>
        /// Determines whether current read position is at the end of the file
        /// </summary>
        public bool EndOfStream
        {
            get
            {
                return reader.EndOfStream;
            }
        }

        /// <summary>
        /// Disposes access to the file
        /// </summary>
        public void Dispose()
        {
            this.reader.Dispose();
        }

        /// <summary>
        /// Reads a line from the file
        /// </summary>
        /// <returns>String line</returns>
        public string ReadLine()
        {
            return this.reader.ReadLine();
        }

        /// <summary>
        /// Reads all text to the end of file
        /// </summary>
        /// <returns>String data</returns>
        public string ReadToEnd()
        {
            return this.reader.ReadToEnd();
        }
    }
}
