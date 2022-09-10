using System.CommandLine;
using System.Globalization;
using CsvHelper;
using MatchingEngine.Domain.Models;

namespace MatchingEngine.CLI.Commands;

public static partial class MatchingCommand
{
    public static Command GetTemplateCommand()
    {
        var templateGenerateCommand = GetTemplateGenerateCommand();
        var templateValidateCommand = GetTemplateValidateCommand();

        var command = new Command("template", "Template operations")
        {
            templateGenerateCommand,
            templateValidateCommand
        };

        return command;
    }

    private static Command GetTemplateGenerateCommand()
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

        return command;
    }

    private static Command GetTemplateValidateCommand()
    {
        var templateFilePathArgument = new Argument<FileInfo>
            (name: "template file path", description: "The path of the file to be validated");

        var command = new Command("validate", "Validates template format")
        {
            templateFilePathArgument,
        };

        command.SetHandler(
            (templateFilePathArgumentValue) =>
            {
                try
                {
                    using var readerFile1 = new StreamReader(templateFilePathArgumentValue.FullName);
                    using var csvRecords1 = new CsvReader(readerFile1, CultureInfo.InvariantCulture);
                    var records = csvRecords1.GetRecords<PatientRecord>();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Template file is valid, and contains {records.Count():N0} records.");
                }
                catch (Exception)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Template file is invalid, use the template generate command to create a valid template file.");
                }
            }, templateFilePathArgument);

        return command;
    }
}