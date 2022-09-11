using System.CommandLine;
using MatchingEngine.CLI.Commands;

var duplicatesCommand = MatchingCommand.GetDuplicatesCommand();
var dictionaryCommand = MatchingCommand.GetDictionaryCommand();
var templateCommand = MatchingCommand.GetTemplateCommand();

var rootCommand = new RootCommand("Contains utilities for matching data")
{
    duplicatesCommand,
    dictionaryCommand,
    templateCommand,
};

await rootCommand.InvokeAsync(args);