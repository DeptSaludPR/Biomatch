
namespace MatchingEngine;

public static class Helpers
{
    public static bool FirstCharactersAreEqual(string a, string b)
    {
        try
        {
            return a[..1] == b[..1];
        }
        catch
        {
            return true;
        }
    }
}