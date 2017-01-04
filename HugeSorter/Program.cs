namespace HugeSorter
{
    using System;
    using System.Collections.Generic;
    using TextSorted;
    using TextSorted.IO;

    /// <summary>
    /// Main app class.</summary>
    public class Program
    {
        /// <summary>
        /// Application entry point.</summary>
        static void Main(string[] args)
        {
            // specify alphabet used in source file
            var allowedChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ ";
            var start = DateTime.Now;

            Func<string, int> LineIndexFunc = line => line.IndexOf('.') + 1;

            // passed chunk size is good for files up to 7.2GB
            // with current alphanumeric set we will have 36 chunks up to 200MB of data,
            // of course it is estimation, and real lines distribution varies, so there may be more chunks
            // you need to change chunk size for different source file sizes
            var sorter = new FileSorter(
                new FileAccess(),
                new LinesComparer(StringComparer.OrdinalIgnoreCase, LineIndexFunc),
                209715200, allowedChars.ToCharArray(),
                LineIndexFunc);

            sorter.Sort("<insert-path-to-source-file-here>", "<insert-path-to-destination-here>");

            var end = DateTime.Now - start;

            Console.WriteLine(string.Format("Completed. Elapsed {0} seconds", end.TotalSeconds));
            Console.ReadLine();
        }
    }
}
