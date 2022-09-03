namespace MatchingEngine.Models;

public static class StringDistance
{
    private static int LowMemoryLevenshteinDist(string sRow, string sCol)
    {
        var rowLen = sRow.Length; // length of sRow
        var colLen = sCol.Length; // length of sCol
        int rowIdx; // iterates through sRow
        int colIdx; // iterates through sCol


        // Step 1

        if (rowLen == 0)
        {
            return colLen;
        }

        if (colLen == 0)
        {
            return rowLen;
        }

        // Create the two vectors
        var v0 = new int[rowLen + 1];
        var v1 = new int[rowLen + 1];


        // Step 2
        // Initialize the first vector
        for (rowIdx = 1; rowIdx <= rowLen; rowIdx++)
        {
            v0[rowIdx] = rowIdx;
        }

        // Step 3

        // Fore each column
        for (colIdx = 1; colIdx <= colLen; colIdx++)
        {
            // Set the 0'th element to the column number
            v1[0] = colIdx;

            var colJ = sCol[colIdx - 1]; // jth character of sCol


            // Step 4

            // For each row
            for (rowIdx = 1; rowIdx <= rowLen; rowIdx++)
            {
                var rowI = sRow[rowIdx - 1]; // ith character of sRow


                // Step 5

                var cost = rowI == colJ ? 0 : 1;

                // Step 6

                // Find minimum
                var mMin = v0[rowIdx] + 1;
                var b = v1[rowIdx - 1] + 1;
                var c = v0[rowIdx - 1] + cost;

                if (b < mMin)
                {
                    mMin = b;
                }

                if (c < mMin)
                {
                    mMin = c;
                }

                v1[rowIdx] = mMin;
            }

            // Swap the vectors
            (v0, v1) = (v1, v0);
        }

        return v0[rowLen];
    }

    public static int GeneralDemographicFieldDistance(string a, string b)
    {
        int distance;
        //check for empty values
        if (a == string.Empty || b == string.Empty)
        {
            distance = -1;
        }
        else if (a == string.Empty && b == string.Empty)
        {
            distance = 0;
        }
        else
        {
            distance = LowMemoryLevenshteinDist(a, b);
        }

        return distance;
    }
}