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

            // passed chunk size is good for files of big sizes, from 1GB
            // you need to change chunk size for small source file sizes for better performance
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
