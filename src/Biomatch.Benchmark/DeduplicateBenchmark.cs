using System.Globalization;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Biomatch.Domain;
using Biomatch.Domain.Models;
using CsvHelper;

namespace Biomatch.Benchmark;

[SimpleJob(RunStrategy.Throughput, warmupCount: 1, iterationCount: 1)]
[MemoryDiagnoser]
public class DeduplicateBenchmark
{
  private PatientRecord[] RecordsToDeduplicate { get; }

  public DeduplicateBenchmark()
  {
    using var readerFile1 = new StreamReader("./Data/records.csv");
    using var csvRecords1 = new CsvReader(readerFile1, CultureInfo.InvariantCulture);
    var records1FromCsv = csvRecords1.GetRecords<PatientRecord>();
    RecordsToDeduplicate = records1FromCsv.Take(60_000).PreprocessData().ToArray();

    Console.WriteLine($"Records to match: {RecordsToDeduplicate.Length}");
  }

  [Benchmark]
  public void TryDeduplicate()
  {
    Deduplicate.TryDeduplicate(RecordsToDeduplicate, 0.85);
  }
}
