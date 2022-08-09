using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using MatchingEngine.Models;

namespace MatchingEngine;

public static class Program
{
    private static async Task Main(string[] args)
    {
        var filePath1 = args.Length > 0 ? args[0] : null;
        if (filePath1 == null)
        {
            Console.WriteLine("Please provide a file path");
            return;
        }

        using var readerFile1 = new StreamReader(filePath1);
        using var csvRecords1 = new CsvReader(readerFile1, CultureInfo.InvariantCulture);
        var records1FromCsv = csvRecords1.GetRecords<PatientRecord>();
        var records1 = records1FromCsv.ToList();


        var filePath2 = args.Length > 1 ? args[1] : null;
        if (filePath2 == null)
        {
            Console.WriteLine("Please provide a file path");
            return;
        }

        using var reader = new StreamReader(filePath2);
        using var csvRecords2 = new CsvReader(reader, CultureInfo.InvariantCulture);
        var records2FromCsv = csvRecords2.GetRecords<PatientRecord>();
        var records2 = records2FromCsv.ToList();

        var fileOutputName = args.Length > 2 ? args[2] : null;
        if (fileOutputName == null)
        {
            Console.WriteLine("Please provide a file path");
            return;
        }
        
        var score = args.Length > 3 ? args[3] : "0.8";
        var scoreValue = double.Parse(score);

        await Run.Run_TwoFileComparison_v2(records1, records2,
            fileOutputName, false, 123, 151, true, 1, 100, false, false, scoreValue);
    }
}