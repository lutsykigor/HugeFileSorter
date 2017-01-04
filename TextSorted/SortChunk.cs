namespace TextSorted
{
    using Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;

    /// <summary>
    /// Sorted lines subset, represents sorted part of a big text file
    /// </summary>
    public class SortChunk
    {
        /// <summary>
        /// Chunk sorted name, used as a key
        /// </summary>
        public string Salt { get; set; }

        /// <summary>
        /// Full file path to the chunk
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// Current size without a buffer
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// Amount of lines in a buffer
        /// </summary>
        public int Buffered { get { return buffer.Count; } }

        private int bufferSize;
        private IFileAccess fileAccess;
        private List<string> buffer = new List<string>();

        /// <param name="salt">Chunk name and key</param>
        /// <param name="fullPath">File path to chunk file</param>
        /// <param name="fileAccess">IO fasade</param>
        /// <param name="bufferSize">Buffer size limit</param>
        public SortChunk(
            string salt,
            string fullPath,
            IFileAccess fileAccess,
            int bufferSize)
        {
            Contract.Requires<InvalidDataException>(!string.IsNullOrEmpty(salt));
            Contract.Requires<NullReferenceException>(fileAccess != null);
            Contract.Requires<InvalidDataException>(bufferSize > 0);

            this.fileAccess = fileAccess;
            this.bufferSize = bufferSize;

            FullPath = Path.Combine(Path.GetDirectoryName(fullPath),
                string.Concat(Guid.NewGuid(), Path.GetFileName(fullPath)));
            Salt = salt;
        }

        /// <summary>
        /// Adds line to chunk, flushes a buffer when it's full
        /// </summary>
        /// <param name="value">Text line</param>
        public void Add(string value)
        {
            buffer.Add(value);
            if (buffer.Count >= this.bufferSize)
            {
                Flush();
            }
        }

        /// <summary>
        /// Flushes buffer to a file
        /// </summary>
        public void Flush()
        {
            if (buffer.Count > 0)
            {
                using (var writer = this.fileAccess.GetWriter(FullPath, true))
                {
                    var flushText = string.Join(Environment.NewLine, buffer);
                    writer.Write(flushText);
                    Size += flushText.Length;
                }
                buffer.Clear();
            }
        }
    }
}
