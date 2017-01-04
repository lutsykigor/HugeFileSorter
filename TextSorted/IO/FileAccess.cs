namespace TextSorted.IO
{
    using System;
    using System.IO;
    using Interfaces;

    /// <summary>
    /// IO fasade
    /// </summary>
    public class FileAccess : IFileAccess
    {
        /// <summary>
        /// Deleted the specified file
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <returns></returns>
        public void Delete(string path)
        {
            File.Delete(path);
        }

        /// <summary>
        /// Returns file size
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <returns>File size in bytes</returns>
        public long GetFileSize(string path)
        {
            return new FileInfo(path).Length;
        }

        /// <summary>
        /// Returns a read access to the specific file
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <returns>IFileReader object</returns>
        public IFileReader GetReader(string path)
        {
            return new FileReader(File.OpenText(path));
        }

        /// <summary>
        /// Returns a write access to the specific file
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <returns>IFileWriter object</returns>
        public IFileWriter GetWriter(string path, bool append)
        {
            return new FileWriter(new StreamWriter(path, append));
        }
    }
}
