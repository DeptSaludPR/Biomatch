using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Biomatch.Benchmark.Csv;
using Biomatch.Domain;
using Biomatch.Domain.Models;

namespace Biomatch.Benchmark;

[SimpleJob(RunStrategy.Throughput, warmupCount: 1, iterationCount: 1)]
[MemoryDiagnoser]
public class DeduplicateBenchmark
{
  private IPersonRecord[] RecordsToDeduplicate { get; }

  public DeduplicateBenchmark()
  {
    var records1FromCsv = PatientRecordParser.ParseCsv("./Data/records.csv");
    RecordsToDeduplicate = records1FromCsv.Take(60_000).ToArray();

    Console.WriteLine($"Records to match: {RecordsToDeduplicate.Length}");
  }

  [Benchmark]
  public void TryDeduplicate()
  {
    Deduplicate.TryDeduplicate(RecordsToDeduplicate, 0.85);
  }
}
