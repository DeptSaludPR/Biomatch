
namespace MatchingEngine
{
    public class Helpers
    {
        // getColumnNames: gets the column names of a csv file and returns it as a string array

        public static bool FirstCharactersAreEqual(string A, string B)
        {
            try
            {
                return A.Substring(0, 1) == B.Substring(0, 1);
            }
            catch
            {
                return true;
            }
        }
    }
}