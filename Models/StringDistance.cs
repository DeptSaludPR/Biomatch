using System;

namespace MatchingEngine.Models
{
    public static class StringDistance
    {
        /// <summary>
        /// Compute the distance between two strings.
        /// </summary>
        public static int LevenshteinDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }

            // Step 7
            return d[n, m];
        }

        public static int LowMemoryLevenshteinDist(string sRow, string sCol)
        {
            int rowLen = sRow.Length; // length of sRow
            int colLen = sCol.Length; // length of sCol
            int rowIdx; // iterates through sRow
            int colIdx; // iterates through sCol
            int cost; // cost


            // Step 1

            if (rowLen == 0)
            {
                return colLen;
            }

            if (colLen == 0)
            {
                return rowLen;
            }

            /// Create the two vectors
            int[] v0 = new int[rowLen + 1];
            int[] v1 = new int[rowLen + 1];
            int[] vTmp;


            /// Step 2
            /// Initialize the first vector
            for (rowIdx = 1; rowIdx <= rowLen; rowIdx++)
            {
                v0[rowIdx] = rowIdx;
            }

            // Step 3

            /// Fore each column
            for (colIdx = 1; colIdx <= colLen; colIdx++)
            {
                /// Set the 0'th element to the column number
                v1[0] = colIdx;

                var Col_j = sCol[colIdx - 1]; // jth character of sCol


                // Step 4

                /// Fore each row
                for (rowIdx = 1; rowIdx <= rowLen; rowIdx++)
                {
                    var Row_i = sRow[rowIdx - 1]; // ith character of sRow


                    // Step 5

                    if (Row_i == Col_j)
                    {
                        cost = 0;
                    }
                    else
                    {
                        cost = 1;
                    }

                    // Step 6

                    /// Find minimum
                    int m_min = v0[rowIdx] + 1;
                    int b = v1[rowIdx - 1] + 1;
                    int c = v0[rowIdx - 1] + cost;

                    if (b < m_min)
                    {
                        m_min = b;
                    }

                    if (c < m_min)
                    {
                        m_min = c;
                    }

                    v1[rowIdx] = m_min;
                }

                /// Swap the vectors
                vTmp = v0;
                v0 = v1;
                v1 = vTmp;
            }

            return (v0[rowLen]);
        }

        public static int GeneralDemographicFieldDistance(string a, string b)
        {
            //check for empty values
            if (a == "" & b == "")
            {
                return 0;
            }
            else if (a == "" | b == "")
            {
                return (-1);
            }
            else
            {
                return (StringDistance.LowMemoryLevenshteinDist(a, b));
            }
        }

        public static int MiddleNameFieldDistance(string a, string b)
        {
            if (a.Length == 1 | b.Length == 1)
            {
                if (a.Substring(0, 1) == b.Substring(0, 1))
                {
                    return (0);
                }
                else
                {
                    return StringDistance.LowMemoryLevenshteinDist(a, b);
                }
            }
            else
            {
                return StringDistance.LowMemoryLevenshteinDist(a, b);
            }
        }
    }
}