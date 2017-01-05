namespace HugeSorter
{
    using System;
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

            Console.WriteLine("Enter source file path:");
            var sourcePath = ReadFilePath(true);

            Console.WriteLine("Enter destination file path:");
            var destinationPath = ReadFilePath(false);

            Console.WriteLine("Processing...");
            var start = DateTime.Now;

            Func<string, int> LineIndexFunc = line => line.IndexOf('.') + 1;

            // passed chunk size is good for files of big sizes, from 1GB
            // you need to change chunk size for small source file sizes for better performance
            var sorter = new FileSorter(
                new FileAccess(),
                new LinesComparer(StringComparer.OrdinalIgnoreCase, LineIndexFunc),
                209715200, allowedChars.ToCharArray(),
                LineIndexFunc);

            sorter.Sort(sourcePath, destinationPath);

            var end = DateTime.Now - start;

            Console.WriteLine(string.Format("Completed. Elapsed {0} seconds", end.TotalSeconds));
            Console.ReadKey();
        }

        private static string ReadFilePath(bool existing)
        {
            var path = Console.ReadLine();
            if (!CheckFilePath(path, existing))
            {
                Console.WriteLine("Invalid path or no permissions, please enter again.");
                return ReadFilePath(existing);
            }
            return path;
        }

        private static bool CheckFilePath(string path, bool existing)
        {
            if (existing)
            {
                return System.IO.File.Exists(path);
            }
            else
            {
                // TODO: find more elegant solution
                try
                {
                    var writer = System.IO.File.OpenWrite(path);
                    writer.Dispose();
                }
                catch
                {
                    return false;
                }
                return true;
            }
        }
    }
}
