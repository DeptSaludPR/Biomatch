using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Biomatch.Benchmark.Csv;
using Biomatch.Domain;
using Biomatch.Domain.Models;

namespace Biomatch.Benchmark;

[SimpleJob(RunStrategy.Throughput, warmupCount: 5, iterationCount: 30)]
[MemoryDiagnoser]
public class FindDuplicateBenchmark
{
  private PersonRecordForMatch[] RecordsToMatch { get; }
  private PersonRecordForMatch[] SampleRecords { get; }

  public FindDuplicateBenchmark()
  {
    var records1FromCsv = PatientRecordParser.ParseCsv("./Data/records-to-match.csv");
    RecordsToMatch = records1FromCsv.PreprocessData().ToArray();

    var records2FromCsv = PatientRecordParser.ParseCsv("./Data/records.csv");
    SampleRecords = records2FromCsv.PreprocessData().ToArray();

    Console.WriteLine($"Records to match: {RecordsToMatch.Length}");
    Console.WriteLine($"Sample records: {SampleRecords.Length}");
  }

  [Benchmark]
  public void DuplicateBenchmarkSameDataSet()
  {
    Match.GetPotentialMatchesFromSameDataSet(RecordsToMatch, SampleRecords, 0.80, 1.0);
  }

  [Benchmark]
  public void DuplicateBenchmarkDifferentDataSet()
  {
    Match.GetPotentialMatchesFromDifferentDataSet(RecordsToMatch, SampleRecords, 0.80, 1.0);
  }
}
