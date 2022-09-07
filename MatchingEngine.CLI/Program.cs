using System.CommandLine;
using MatchingEngine.CLI.Commands;

var findDuplicatesCommand = MatchingCommand.GetFindDuplicatesCommand();
var preprocessCommand = MatchingCommand.GetPreprocessCommand();

var rootCommand = new RootCommand
{
    preprocessCommand,
    findDuplicatesCommand,
};

await rootCommand.InvokeAsync(args);