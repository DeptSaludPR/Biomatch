using System.CommandLine;
using MatchingEngine.CLI.Commands;

var findCommand = MatchingCommand.GetFindCommand();
var deduplicateCommand = MatchingCommand.GetDeduplicateCommand();
var dictionaryCommand = MatchingCommand.GetDictionaryCommand();
var templateCommand = MatchingCommand.GetTemplateCommand();

var rootCommand = new RootCommand("Contains utilities for matching data")
{
  findCommand,
  deduplicateCommand,
  dictionaryCommand,
  templateCommand,
};

await rootCommand.InvokeAsync(args);
