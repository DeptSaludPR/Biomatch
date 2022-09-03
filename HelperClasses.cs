using System;

namespace MatchingEngine;

public static class Helpers
{
    public static bool FirstCharactersAreEqual(string a, string b)
    {
        return string.Equals(a[..1], b[..1], StringComparison.InvariantCultureIgnoreCase);
    }
}