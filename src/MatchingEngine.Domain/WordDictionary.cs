namespace MatchingEngine.Domain;

public sealed class WordDictionary
{
  private readonly SymSpell _symSpell;

  public WordDictionary(FileInfo dictionaryFilePath)
  {
    if (!dictionaryFilePath.Exists)
      throw new FileNotFoundException($"Dictionary file not found: {dictionaryFilePath.FullName}",
        dictionaryFilePath.FullName);
    //create object
    const int initialCapacity = 82765;
    const int maxEditDistanceDictionary = 2; //maximum edit distance per dictionary pre-calculation
    _symSpell = new SymSpell(initialCapacity, maxEditDistanceDictionary);

    //load dictionary
    const int termIndex = 0; //column of the term in the dictionary text file
    const int countIndex = 1; //column of the term frequency in the dictionary text file
    if (!_symSpell.LoadDictionary(dictionaryFilePath.FullName, termIndex, countIndex))
    {
      throw new Exception("Fail to load dictionary");
    }
  }

  public List<SymSpell.SuggestItem> TrySpellCheck(string recordToSearch)
  {
    //lookup suggestions for single-word input strings
    const int
      maxEditDistanceLookup = 2; //max edit distance per lookup (maxEditDistanceLookup<=maxEditDistanceDictionary)
    const SymSpell.Verbosity suggestionVerbosity = SymSpell.Verbosity.Closest; //Top, Closest, All
    return _symSpell.Lookup(recordToSearch, suggestionVerbosity, maxEditDistanceLookup);
  }
}
