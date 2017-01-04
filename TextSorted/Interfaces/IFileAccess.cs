namespace TextSorted.Interfaces
{
    /// <summary>
    /// IO fasade abstraction
    /// </summary>
    public interface IFileAccess
    {
        IFileReader GetReader(string path);
        IFileWriter GetWriter(string path, bool append);
        long GetFileSize(string path);
        void Delete(string path);
    }
}
