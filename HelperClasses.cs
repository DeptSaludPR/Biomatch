namespace MatchingEngine;

public static class Helpers
{
    public static bool FirstCharactersAreEqual(string a, string b)
    {
        if (a == string.Empty || b == string.Empty)
            return true;
        return string.Equals(a[..1], b[..1], StringComparison.InvariantCultureIgnoreCase);
    }
}