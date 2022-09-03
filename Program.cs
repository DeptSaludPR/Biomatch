using System.CommandLine;
using System.Globalization;
using CsvHelper;
using MatchingEngine.Models;

namespace MatchingEngine;

public static class Program
{
    private static async Task Main(string[] args)
    {
        var filePath1Option = new Option<string>
            (name: "--filepath1", description: "File path for first file");
        var filePath2Option = new Option<string>
            (name: "--filepath2", description: "File path for second file");
        var outputOption = new Option<string>
            (name: "--output", description: "Output file path");
        var scoreOption = new Option<double>
        (name: "--score",
            description: "Score for matching",
            getDefaultValue: () => 0.8);

        var rootCommand = new RootCommand();
        rootCommand.Add(filePath1Option);
        rootCommand.Add(filePath2Option);
        rootCommand.Add(outputOption);
        rootCommand.Add(scoreOption);

        rootCommand.SetHandler(async (filePath1OptionValue, filePath2OptionValue, outputOptionValue, scoreOptionValue) =>
            {
                using var readerFile1 = new StreamReader(filePath1OptionValue);
                using var csvRecords1 = new CsvReader(readerFile1, CultureInfo.InvariantCulture);
                var records1FromCsv = csvRecords1.GetRecords<PatientRecord>();
                var records1 = records1FromCsv.ToList();
                
                using var reader = new StreamReader(filePath2OptionValue);
                using var csvRecords2 = new CsvReader(reader, CultureInfo.InvariantCulture);
                var records2FromCsv = csvRecords2.GetRecords<PatientRecord>();
                var records2 = records2FromCsv.ToList();
                
                await Run.Run_TwoFileComparison_v2(records1, records2,
                    outputOptionValue, true, 1, 100, true, 1, 100, false, scoreOptionValue);
            },
            filePath1Option, filePath2Option, outputOption, scoreOption);

        await rootCommand.InvokeAsync(args);
    }
}