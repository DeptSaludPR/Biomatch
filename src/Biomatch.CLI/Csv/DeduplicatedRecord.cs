namespace Biomatch.CLI.Csv;

public record DeduplicatedRecord
(
  string RecordId,
  string DuplicateRecordIds
);
