namespace MatchingEngine.Domain.Helpers;

public static class StringHelpers
{
    public static bool FirstCharactersAreEqual(ReadOnlySpan<char> a, ReadOnlySpan<char> b)
    {
        return a.IsEmpty || b.IsEmpty || a[..1].StartsWith(b[..1]);
    }
}