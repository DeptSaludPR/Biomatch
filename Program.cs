using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using MatchingEngine.Models;

namespace MatchingEngine;

static class Program
{
    private static async Task Main(string[] args)
    {
        //Comparison from cases vs active
        const string? filePath1 =
            "C:/Users/Andres/Desktop/CSV Example Code - Copy/ACTIVE_CASE_API_2021_07_23 - Copy.csv"; //Necesito el archivo al que el path apunta el algo.csv
        using var readerFile1 = new StreamReader(filePath1);
        using var csvRecords1 = new CsvReader(readerFile1, CultureInfo.InvariantCulture);

        //
        var records1FromCsv = csvRecords1.GetRecords<PatientRecord>();
        //
        var records1 = records1FromCsv.ToList();


        const string? filePath2 =
            "C:/Users/Andres/Desktop/CSV Example Code - Copy/CASE_API_2021_07_23 - Copy.csv"; //Necesito el archivo al que el path apunta el algo.csv
        using var reader = new StreamReader(filePath2);
        using var csvRecords2 = new CsvReader(reader, CultureInfo.InvariantCulture);

        // 
        var records2FromCsv = csvRecords2.GetRecords<PatientRecord>();
        //
        var records2 = records2FromCsv.ToList();

        await Run.Run_SameFileComparison_v2(records1, "testing_same_file_comparison");
        await Run.Run_TwoFileComparison_v2(records1, records2,
            "2021_08_11_test_run_for_duplicates", false, 123, 151, true, 1, 100, true, false, 0.7);
    }
}