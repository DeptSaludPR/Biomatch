using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using Biomatch.CLI.Csv;
using CsvHelper;
using Biomatch.Domain;
using Biomatch.Domain.Models;

namespace Biomatch.CLI.Services;

public static class DuplicateService
{
  public static async Task RunFileComparisons(IEnumerable<PatientRecord> records1,
    IEnumerable<PatientRecord> records2, FileInfo outputFileName, bool exactMatchesAllowed = false,
    double lowerScoreThreshold = 0.65, WordDictionary? firstNamesDictionary = null,
    WordDictionary? middleNamesDictionary = null, WordDictionary? lastNamesDictionary = null,
    bool sameDataSetOptionValue = false)
  {
    //start a stopwatch
    var timer = new Stopwatch();
    timer.Start();

    var preprocessedRecords1 =
      records1.PreprocessData(firstNamesDictionary, middleNamesDictionary, lastNamesDictionary).ToArray();
    var preprocessedRecords2 =
      records2.PreprocessData(firstNamesDictionary, middleNamesDictionary, lastNamesDictionary).ToArray();

    //check for whether the exact matches are allowed, and set the upper threshold accordingly
    var upperScoreThreshold = exactMatchesAllowed ? 1.0 : 0.99999;

    ConcurrentBag<PotentialMatch> potentialDuplicates;
    if (sameDataSetOptionValue)
    {
      potentialDuplicates = Match.GetPotentialMatchesFromSameDataSet(preprocessedRecords1, preprocessedRecords2,
        lowerScoreThreshold, upperScoreThreshold);
    }
    else
    {
      potentialDuplicates = Match.GetPotentialMatchesFromDifferentDataSet(preprocessedRecords1, preprocessedRecords2,
        lowerScoreThreshold, upperScoreThreshold);
    }

    //write the total elapsed time on the log
    timer.Stop();

    var urlDocs = potentialDuplicates
      .Select(e => new DuplicateRecord(e));
    await using var writer = new StreamWriter(outputFileName.FullName, false, Encoding.UTF8);
    await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
    csv.Context.RegisterClassMap<DuplicateRecordMap>();
    await csv.WriteRecordsAsync(urlDocs);
  }
}
