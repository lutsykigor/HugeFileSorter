namespace TextSorted.Interfaces
{
    using System;

    /// <summary>
    /// File write access abstraction
    /// </summary>
    public interface IFileWriter : IDisposable
    {
        void Write(string value);
    }
}
