using System.CommandLine;
using MatchingEngine.CLI.Commands;

var findDuplicatesCommand = MatchingCommand.GetFindDuplicatesCommand();
var preprocessCommand = MatchingCommand.GetPreprocessCommand();

var rootCommand = new RootCommand
{
    findDuplicatesCommand,
    preprocessCommand
};

await rootCommand.InvokeAsync(args);