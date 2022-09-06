// See https://aka.ms/new-console-template for more information

using System.CommandLine;
using System.Globalization;
using CsvHelper;
using MatchingEngine.CLI.Csv;
using MatchingEngine.CLI.Services;
using MatchingEngine.Domain.Models;

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

var rootCommand = new RootCommand
{
    filePath1Argument,
    filePath2Argument,
    outputOption,
    scoreOption
};

rootCommand.SetHandler(
    async (filePath1ArgumentValue, filePath2ArgumentValue, outputOptionValue, scoreOptionValue) =>
    {
        using var readerFile1 = new StreamReader(filePath1ArgumentValue);
        using var csvRecords1 = new CsvReader(readerFile1, CultureInfo.InvariantCulture);
        csvRecords1.Context.RegisterClassMap<PatientRecordMap>();
        var records1FromCsv = csvRecords1.GetRecords<PatientRecord>();
        var records1 = records1FromCsv.ToArray();

        using var reader = new StreamReader(filePath2ArgumentValue);
        using var csvRecords2 = new CsvReader(reader, CultureInfo.InvariantCulture);
        csvRecords2.Context.RegisterClassMap<PatientRecordMap>();
        var records2FromCsv = csvRecords2.GetRecords<PatientRecord>();
        var records2 = records2FromCsv.ToArray();

        await DuplicateService.RunFileComparisons(records1, records2,
            outputOptionValue, true, 1, 100, true, 1, 100, false, scoreOptionValue);
    },
    filePath1Argument, filePath2Argument, outputOption, scoreOption);

await rootCommand.InvokeAsync(args);