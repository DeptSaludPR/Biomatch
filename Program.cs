using System.CommandLine;
using System.Globalization;
using CsvHelper;
using MatchingEngine.Models;

namespace MatchingEngine;

public static class Program
{
    private static async Task Main(string[] args)
    {
        var filePath1Argument = new Argument<string>
            (name: "filePath1", description: "The path to the first file to be compared");
        var filePath2Argument = new Argument<string>
            (name: "filePath2", description: "The path to the second file to be compared");
        var outputOption = new Option<string>
            (name: "--output", description: "Output file path");
        var scoreOption = new Option<double>
        (name: "--score",
            description: "Score for matching",
            getDefaultValue: () => 0.8);

        var rootCommand = new RootCommand();
        rootCommand.Add(filePath1Argument);
        rootCommand.Add(filePath2Argument);
        rootCommand.Add(outputOption);
        rootCommand.Add(scoreOption);

        rootCommand.SetHandler(
            async (filePath1ArgumentValue, filePath2ArgumentValue, outputOptionValue, scoreOptionValue) =>
            {
                using var readerFile1 = new StreamReader(filePath1ArgumentValue);
                using var csvRecords1 = new CsvReader(readerFile1, CultureInfo.InvariantCulture);
                var records1FromCsv = csvRecords1.GetRecords<PatientRecord>();
                var records1 = records1FromCsv.ToArray();

                using var reader = new StreamReader(filePath2ArgumentValue);
                using var csvRecords2 = new CsvReader(reader, CultureInfo.InvariantCulture);
                var records2FromCsv = csvRecords2.GetRecords<PatientRecord>();
                var records2 = records2FromCsv.ToArray();

                await Run.RunFileComparisons(records1, records2,
                    outputOptionValue, true, 1, 100, true, 1, 100, false, scoreOptionValue);
            },
            filePath1Argument, filePath2Argument, outputOption, scoreOption);

        await rootCommand.InvokeAsync(args);
    }
}