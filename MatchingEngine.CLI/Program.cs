using System.CommandLine;
using MatchingEngine.CLI.Commands;

var duplicatesCommand = MatchingCommand.GetDuplicatesCommand();
var preprocessCommand = MatchingCommand.GetPreprocessCommand();
var templateCommand = MatchingCommand.GetTemplateCommand();

var rootCommand = new RootCommand("Contains utilities for matching data")
{
    preprocessCommand,
    duplicatesCommand,
    templateCommand
};

await rootCommand.InvokeAsync(args);