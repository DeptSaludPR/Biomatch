using System.Globalization;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using CsvHelper;
using MatchingEngine.Domain;
using MatchingEngine.Domain.Models;

namespace MatchingEngine.Benchmark;

[SimpleJob(RunStrategy.Throughput, warmupCount: 2, invocationCount: 5)]
[MemoryDiagnoser]
public class FindDuplicateBenchmark
{
    private PatientRecord[] RecordsToMatch { get; }
    private PatientRecord[] SampleRecords { get; }

    public FindDuplicateBenchmark()
    {
        using var readerFile1 = new StreamReader("./Data/persons_to_match.csv");
        using var csvRecords1 = new CsvReader(readerFile1, CultureInfo.InvariantCulture);
        var records1FromCsv = csvRecords1.GetRecords<PatientRecord>();
        RecordsToMatch = records1FromCsv.ToArray();

        using var reader = new StreamReader("./Data/persons.csv");
        using var csvRecords2 = new CsvReader(reader, CultureInfo.InvariantCulture);
        var records2FromCsv = csvRecords2.GetRecords<PatientRecord>();
        SampleRecords = records2FromCsv.ToArray();
    }

    [Benchmark]
    public void DuplicateBenchmark()
    {
        Duplicate.GetPotentialDuplicates(RecordsToMatch, SampleRecords, 0.8, 1.0);
    }
}