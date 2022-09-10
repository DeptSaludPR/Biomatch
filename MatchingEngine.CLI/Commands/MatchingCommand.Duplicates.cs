using System.CommandLine;
using System.Globalization;
using CsvHelper;
using MatchingEngine.CLI.Services;
using MatchingEngine.Domain.Models;

namespace MatchingEngine.CLI.Commands;

public static partial class MatchingCommand
{
    public static Command GetFindDuplicatesCommand()
    {
        var filePath1Argument = new Argument<FileInfo>
            (name: "filePath1", description: "The path to the first file to be compared");

        var filePath2Argument = new Argument<FileInfo>
            (name: "filePath2", description: "The path to the second file to be compared");

        var outputOption = new Option<FileInfo>
        (name: "--output", description: "Output file path",
            getDefaultValue: () => new FileInfo("Duplicates.csv"));
        outputOption.AddAlias("-o");

        var scoreOption = new Option<double>
        (name: "--score",
            description: "Score for matching",
            getDefaultValue: () => 0.8);

        var logPathOption = new Option<FileInfo?>
        (name: "--log",
            description: "Log file path",
            getDefaultValue: () => null);
        logPathOption.AddAlias("-l");

        var command = new Command("find-duplicates", "Find duplicates records in two files")
        {
            filePath1Argument,
            filePath2Argument,
            outputOption,
            scoreOption,
            logPathOption
        };

        command.SetHandler(
            async (filePath1ArgumentValue, filePath2ArgumentValue, outputOptionValue, scoreOptionValue, logPathValue) =>
            {
                using var readerFile1 = new StreamReader(filePath1ArgumentValue.FullName);
                using var csvRecords1 = new CsvReader(readerFile1, CultureInfo.InvariantCulture);
                var records1FromCsv = csvRecords1.GetRecords<PatientRecord>();

                using var reader = new StreamReader(filePath2ArgumentValue.FullName);
                using var csvRecords2 = new CsvReader(reader, CultureInfo.InvariantCulture);
                var records2FromCsv = csvRecords2.GetRecords<PatientRecord>();

                await DuplicateService.RunFileComparisons(records1FromCsv, records2FromCsv,
                    outputOptionValue, true, 1, 100, true, 1, 100, false, scoreOptionValue, logPathValue);
            },
            filePath1Argument, filePath2Argument, outputOption, scoreOption, logPathOption);

        return command;
    }
}