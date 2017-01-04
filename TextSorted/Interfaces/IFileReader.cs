namespace TextSorted.Interfaces
{
    using System;

    /// <summary>
    /// File read access abstraction
    /// </summary>
    public interface IFileReader : IDisposable
    {
        bool EndOfStream { get; }
        string ReadLine();
        string ReadToEnd();
    }
}
