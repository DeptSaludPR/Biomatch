using System.CommandLine;
using System.Globalization;
using CsvHelper;
using MatchingEngine.Domain.Models;

namespace MatchingEngine.CLI.Commands;

public static partial class MatchingCommand
{
    public static Command GetTemplateCommand()
    {
        var outputOption = new Option<FileInfo>
        (name: "--output", description: "Output file path",
            getDefaultValue: () => new FileInfo("MatchingTemplate.csv"));
        outputOption.AddAlias("-o");

        var command = new Command("generate", "Creates an empty template file")
        {
            outputOption,
        };

        command.SetHandler(
            async (outputOptionValue) =>
            {
                await using var writer = new StreamWriter(outputOptionValue.FullName);
                await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
                csv.WriteHeader<PatientRecord>();
            }, outputOption);
        
        var rootCommand = new Command("template", "Template operations")
        {
            command
        };

        return rootCommand;
    }
}