using System.CommandLine;
using MatchingEngine.CLI.Commands;

var findDuplicatesCommand = MatchingCommand.GetFindDuplicatesCommand();
var preprocessCommand = MatchingCommand.GetPreprocessCommand();
var templateCommand = MatchingCommand.GetTemplateCommand();

var rootCommand = new RootCommand("Contains utilities for matching data")
{
    preprocessCommand,
    findDuplicatesCommand,
    templateCommand
};

await rootCommand.InvokeAsync(args);