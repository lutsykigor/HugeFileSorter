namespace TextSorted
{
    using Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// Extra large files sorter, distribute source file lines to small sorted chunks
    /// then sorts these chunks and merge results into single destination file
    /// </summary>
    public class FileSorter
    {
        // magic number, helps to dial with not equal distrubution of lines
        // increase it to have less splits, which impacts performance,
        // decreate - to have less file chunks
        private const int STOCHASTIC_MULTIPLIER = 2;

        private readonly long maxChunkSize;
        private readonly char[] sortedCharSet;
        private readonly char lowestCharacter;
        private readonly IFileAccess fileAccess;
        private readonly IComparer<string> comparer;
        private readonly Func<string, int> lineStartIndexFunc;

        private string sourcePath;
        private string destinationPath;
        private int maxSaltLength;
        private int chunkBufferSize;
        private SortedDictionary<string, SortChunk> chunkMap =
            new SortedDictionary<string, SortChunk>(StringComparer.InvariantCultureIgnoreCase);

        /// <param name="fileAccess">IO entry point</param>
        /// <param name="comparer">Lines comparer</param>
        /// <param name="maxChunkSize">Maximum size of chunk in bytes</param>
        /// <param name="sortedCharSet">Allowed characters</param>
        /// <param name="lineStartIndexFunc">Gets text part index from specific line, optional</param>
        public FileSorter(
            IFileAccess fileAccess,
            IComparer<string> comparer,
            long maxChunkSize,
            char[] sortedCharSet,
            Func<string, int> lineStartIndexFunc = null)
        {
            Contract.Requires<NullReferenceException>(fileAccess != null);
            Contract.Requires<NullReferenceException>(comparer != null);
            Contract.Requires<InvalidDataException>(maxChunkSize > 0);
            Contract.Requires<InvalidDataException>(sortedCharSet.Length > 0);

            this.fileAccess = fileAccess;
            this.comparer = comparer;
            this.sortedCharSet = sortedCharSet;
            this.maxChunkSize = maxChunkSize;
            this.lowestCharacter = this.sortedCharSet.First();
            this.lineStartIndexFunc = lineStartIndexFunc;
        }

        private void Initialize(string source, string destination)
        {
            Contract.Ensures(this.chunkBufferSize < maxChunkSize);
            Contract.Ensures(this.chunkMap.Keys.Count > 0);

            var fileSize = this.fileAccess.GetFileSize(source);

            // estimation of the chunks amount needed for this file,
            // STOCHASTIC_MULTIPLIER - is a magic constant,
            // allows to deal with not equal (stochastic) distrubution of lines in file
            var estimatedChunks = fileSize / maxChunkSize * STOCHASTIC_MULTIPLIER;

            this.sourcePath = source;
            this.destinationPath = destination;

            // average chunk buffer size,
            // chunk buffer depends on entire chunks count
            this.chunkBufferSize = (int)(maxChunkSize / estimatedChunks / 1000);

            if (this.chunkBufferSize == 0)
            {
                this.chunkBufferSize = 1;
            }

            // calculate max chunk sorted name length depends on allowed characters set and estimated chunks amount
            this.maxSaltLength = (int)Math.Ceiling(Math.Log(estimatedChunks, this.sortedCharSet.Length));

            this.chunkMap = ChunkCartesian(this.maxSaltLength);
        }

        /// <summary>
        /// Performs text sort, capable to sort extra large files
        /// </summary>
        /// <param name="source">Source file path</param>
        /// <param name="destination">Destination file path</param>
        public void Sort(string source, string destination)
        {
            Initialize(source, destination);
            using (var reader = this.fileAccess.GetReader(sourcePath))
            {
                while (!reader.EndOfStream)
                {
                    ProcessLine(reader.ReadLine(), this.chunkMap);
                }
            }

            var filledChunks = this.chunkMap.Values.Where(
                c => c.Size > 0 || c.Buffered > 0);

            foreach (var chunk in filledChunks)
            {
                chunk.Flush();
                SortChunk(chunk);
            }
        }

        private void ProcessLine(string line, SortedDictionary<string, SortChunk> chunkMap)
        {
            var splitIndex = 0;
            if (lineStartIndexFunc != null)
            {
                splitIndex = lineStartIndexFunc(line);
                if (splitIndex == -1)
                {
                    return;
                }

            }

            // parse line
            var textLength = line.Length - splitIndex;
            var subLength = textLength < this.maxSaltLength ? textLength : this.maxSaltLength;
            var lineSalt = line.Substring(splitIndex, subLength);

            // deal with situation when actual text part length is lower than chunk salt length
            if (lineSalt.Length < this.maxSaltLength)
            {
                lineSalt = string.Concat(lineSalt,
                    new string(this.lowestCharacter,
                    this.maxSaltLength - lineSalt.Length));
            }

            GetChunk(lineSalt, line, chunkMap).Add(line);
        }

        private void SortChunk(SortChunk chunk)
        {
            using (var reader = this.fileAccess.GetReader(chunk.FullPath))
            {
                var lines = reader.ReadToEnd().Split(
                    Environment.NewLine.ToCharArray(),
                    StringSplitOptions.RemoveEmptyEntries);

                Array.Sort(lines, this.comparer);

                using (var writer = this.fileAccess.GetWriter(this.destinationPath, true))
                {
                    writer.Write(string.Join(Environment.NewLine, lines));
                }
            }

            this.fileAccess.Delete(chunk.FullPath);
        }

        private SortedDictionary<string, SortChunk> ChunkCartesian(
            int length, params string[] startCharacters)
        {
            var characters = this.sortedCharSet.Select(c => c.ToString());
            var cartesianList = new List<string>(
                startCharacters.Length == 0 ? characters : startCharacters);

            // generate all possible chunk name combinations
            while (length > 1)
            {
                cartesianList = cartesianList.SelectMany((left) => characters,
                    (left, right) => string.Concat(left, right)).ToList();
                length--;
            }

            var cartesianChunks = cartesianList.Select(
                salt => new SortChunk(salt, this.sourcePath,
                this.fileAccess, this.chunkBufferSize));

            return new SortedDictionary<string, SortChunk>(
                cartesianChunks.ToDictionary(chunk => chunk.Salt), StringComparer.OrdinalIgnoreCase);
        }

        private SortChunk GetChunk(string salt,
            string line, SortedDictionary<string, SortChunk> chunkMap)
        {
            if (!chunkMap.ContainsKey(salt))
            {
                if (string.IsNullOrEmpty(salt))
                {
                    throw new Exception(string.Format("Unknown character: {0}", salt));
                }

                // recursively calls itself, due to chunk splits
                return GetChunk(salt.Substring(0, salt.Length - 1), line, chunkMap);
            }

            var chunk = chunkMap[salt];
            if (chunk.Size >= this.maxChunkSize)
            {
                // split chunk if its size is too big, and return new chunk
                return chunkMap[SplitChunk(chunk, line, chunkMap)];
            }

            return chunk;
        }

        private string SplitChunk(SortChunk chunk,
            string line, SortedDictionary<string, SortChunk> chunkMap)
        {
            string[] chunkLines;

            chunk.Flush();

            using (var reader = this.fileAccess.GetReader(chunk.FullPath))
            {
                chunkLines = reader.ReadToEnd().Split(
                    Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            }

            this.maxSaltLength++;

            // split chunk into a new set with length of the current charset
            var splitedChunks = ChunkCartesian(2, chunk.Salt);

            foreach (var splitedChunk in splitedChunks)
            {
                chunkMap.Add(splitedChunk.Key, splitedChunk.Value);
            }

            // process all chunk lines into newly created sub chunks
            foreach (var chunkLine in chunkLines)
            {
                ProcessLine(chunkLine, splitedChunks);
            }

            // remove splited chunk
            chunkMap.Remove(chunk.Salt);
            this.fileAccess.Delete(chunk.FullPath);

            // return chunk key for the current line
            return line.Substring(line.IndexOf(chunk.Salt, StringComparison.OrdinalIgnoreCase), this.maxSaltLength);
        }
    }
}
