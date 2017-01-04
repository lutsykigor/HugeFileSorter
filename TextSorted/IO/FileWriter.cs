namespace TextSorted.IO
{
    using Interfaces;
    using System.IO;

    /// <summary>
    /// File output proxy
    /// </summary>
    public class FileWriter : IFileWriter
    {
        private StreamWriter writer;
        public FileWriter(StreamWriter writer)
        {
            this.writer = writer;
        }

        /// <summary>
        /// Disposes access to the file
        /// </summary>
        public void Dispose()
        {
            this.writer.Dispose();
        }

        /// <summary>
        /// Writes a string value to the file
        /// </summary>
        /// <param name="value">Payload to write</param>
        public void Write(string value)
        {
            this.writer.Write(value);
        }
    }
}
