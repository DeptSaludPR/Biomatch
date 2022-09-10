using System.CommandLine;
using MatchingEngine.CLI.Commands;

var findDuplicatesCommand = MatchingCommand.GetFindDuplicatesCommand();
var preprocessCommand = MatchingCommand.GetPreprocessCommand();
var templateCommand = MatchingCommand.GetTemplateCommand();

var rootCommand = new RootCommand
{
    preprocessCommand,
    findDuplicatesCommand,
    templateCommand
};

await rootCommand.InvokeAsync(args);