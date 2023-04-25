using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Biomatch.Benchmark.Csv;
using Biomatch.Domain;
using Biomatch.Domain.Models;

namespace Biomatch.Benchmark;

[SimpleJob(RunStrategy.Throughput, warmupCount: 1, iterationCount: 5)]
[MemoryDiagnoser]
public class PreprocessBenchmark
{
  private IPersonRecord[] RecordsToMatch { get; }

  public PreprocessBenchmark()
  {
    var records1FromCsv = PatientRecordParser.ParseCsv("./Data/records.csv");
    RecordsToMatch = records1FromCsv.ToArray();
  }

  [Benchmark]
  public void PreprocessData()
  {
    RecordsToMatch.PreprocessData();
  }
}
