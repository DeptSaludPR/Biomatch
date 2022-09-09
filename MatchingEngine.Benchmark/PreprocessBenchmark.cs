using System.Globalization;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using CsvHelper;
using MatchingEngine.Domain;
using MatchingEngine.Domain.Models;

namespace MatchingEngine.Benchmark;

[SimpleJob(RunStrategy.Throughput, warmupCount: 5, targetCount: 10)]
[MemoryDiagnoser]
public class PreprocessBenchmark
{
    private PatientRecord[] RecordsToMatch { get; }
    
    public PreprocessBenchmark()
    {
        using var readerFile1 = new StreamReader("./Data/testPatients.csv");
        using var csvRecords1 = new CsvReader(readerFile1, CultureInfo.InvariantCulture);
        var records1FromCsv = csvRecords1.GetRecords<PatientRecord>();
        RecordsToMatch = records1FromCsv.ToArray();
    }
    
    [Benchmark]
    public void PreprocessData()
    {
        Preprocess.PreprocessData(RecordsToMatch);
    }
}