using System.Globalization;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using CsvHelper;
using MatchingEngine.Models;

namespace MatchingEngine;

[SimpleJob(RunStrategy.Throughput, warmupCount: 10, targetCount: 10)]
[MemoryDiagnoser]
public class MatchingEngineBenchmark
{
    private PatientRecord[] RecordsToMatch { get; }
    private PatientRecord[] SampleRecords { get; }

    public MatchingEngineBenchmark()
    {
        using var readerFile1 = new StreamReader("./Data/Sample100.csv");
        using var csvRecords1 = new CsvReader(readerFile1, CultureInfo.InvariantCulture);
        var records1FromCsv = csvRecords1.GetRecords<PatientRecord>();
        RecordsToMatch = records1FromCsv.ToArray();

        using var reader = new StreamReader("./Data/testPatients.csv");
        using var csvRecords2 = new CsvReader(reader, CultureInfo.InvariantCulture);
        var records2FromCsv = csvRecords2.GetRecords<PatientRecord>();
        SampleRecords = records2FromCsv.ToArray();
    }

    [Benchmark]
    public void RunBenchmark()
    {
        Run.GetPotentialDuplicates(RecordsToMatch, SampleRecords, 0, RecordsToMatch.Length, 0, SampleRecords.Length,
            0.8, 0.99999);
    }
}