namespace HugeSorter
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Custom lines comparer.</summary>
    public class LinesComparer : IComparer<string>
    {
        private readonly IComparer<string> baseComparer;
        private readonly Func<string, int> textStartIndexFunc;
        public LinesComparer(IComparer<string> baseComparer, Func<string, int> textStartIndexFunc)
        {
            this.baseComparer = baseComparer;
            this.textStartIndexFunc = textStartIndexFunc;
        }

        /// <summary>
        /// Compare two lines
        /// </summary>
        /// <returns>Comparison result</returns>
        public int Compare(string x, string y)
        {
            var splitX = textStartIndexFunc(x);
            if (splitX == 0)
            {
                return 1;
            }

            var splitY = textStartIndexFunc(y);
            if (splitY == 0)
            {
                return -1;
            }

            // get text parts
            var textX = x.Substring(splitX, x.Length - splitX);
            var textY = y.Substring(splitY, y.Length - splitY);

            // compare text parts only
            var baseRes = baseComparer.Compare(textX, textY);

            if (baseRes == 0)
            {
                // compare num parts
                var numStrX = x.Substring(0, splitX);
                var numStrY = y.Substring(0, splitY);
                int numX, numY;
                if (int.TryParse(numStrX, out numX) && int.TryParse(numStrY, out numY))
                {
                    return numX.CompareTo(numY);
                }
            }

            return baseRes;
        }
    }
}
