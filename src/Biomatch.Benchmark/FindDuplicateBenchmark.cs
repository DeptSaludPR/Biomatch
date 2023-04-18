using System.Globalization;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Biomatch.Domain;
using Biomatch.Domain.Models;
using CsvHelper;

namespace Biomatch.Benchmark;

[SimpleJob(RunStrategy.Throughput, warmupCount: 2, iterationCount: 5)]
[MemoryDiagnoser]
public class FindDuplicateBenchmark
{
  private PatientRecord[] RecordsToMatch { get; }
  private PatientRecord[] SampleRecords { get; }

  public FindDuplicateBenchmark()
  {
    using var readerFile1 = new StreamReader("./Data/records-to-match.csv");
    using var csvRecords1 = new CsvReader(readerFile1, CultureInfo.InvariantCulture);
    var records1FromCsv = csvRecords1.GetRecords<PatientRecord>();
    RecordsToMatch = records1FromCsv.PreprocessData().ToArray();

    using var reader = new StreamReader("./Data/records.csv");
    using var csvRecords2 = new CsvReader(reader, CultureInfo.InvariantCulture);
    var records2FromCsv = csvRecords2.GetRecords<PatientRecord>();
    SampleRecords = records2FromCsv.PreprocessData().ToArray();

    Console.WriteLine($"Records to match: {RecordsToMatch.Length}");
    Console.WriteLine($"Sample records: {SampleRecords.Length}");
  }

  [Benchmark]
  public void DuplicateBenchmarkSameDataSet()
  {
    Match.GetPotentialMatchesFromSameDataSet(RecordsToMatch, SampleRecords, 0.85, 1.0);
  }

  [Benchmark]
  public void DuplicateBenchmarkDifferentDataSet()
  {
    Match.GetPotentialMatchesFromDifferentDataSet(RecordsToMatch, SampleRecords, 0.85, 1.0);
  }
}
