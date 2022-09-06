using System.CommandLine;
using System.Globalization;
using CsvHelper;
using MatchingEngine.Domain;
using MatchingEngine.Domain.Models;

namespace MatchingEngine.CLI.Commands;

public static partial class MatchingCommand
{
    public static Command GetPreprocessCommand()
    {
        var filePath1Argument = new Argument<string>
            (name: "filePath", description: "The path of the file to be preprocessed");

        var outputOption = new Option<string>
            (name: "--output", description: "Output file path",
                getDefaultValue: () => "./ProcessedRecords.csv");
        outputOption.AddAlias("-o");

        var command = new Command("preprocess", "Prepare a file for matching")
        {
            filePath1Argument,
            outputOption,
        };

        command.SetHandler(
            async (filePath1ArgumentValue, outputOptionValue) =>
            {
                using var readerFile1 = new StreamReader(filePath1ArgumentValue);
                using var csvRecords1 = new CsvReader(readerFile1, CultureInfo.InvariantCulture);
                var records1FromCsv = csvRecords1.GetRecords<PatientRecord>();
                var records1 = records1FromCsv.ToArray();

                var processedRecords = Preprocess.PreprocessData(records1);
                
                await using var writer = new StreamWriter(outputOptionValue);
                await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
                await csv.WriteRecordsAsync(processedRecords);
            },
            filePath1Argument, outputOption);

        return command;
    }
}