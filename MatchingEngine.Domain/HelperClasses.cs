namespace MatchingEngine.Domain;

public static class Helpers
{
    public static bool FirstCharactersAreEqual(ReadOnlySpan<char> a, ReadOnlySpan<char> b)
    {
        return a.IsEmpty || b.IsEmpty || a[..1].StartsWith(b[..1]);
    }
}