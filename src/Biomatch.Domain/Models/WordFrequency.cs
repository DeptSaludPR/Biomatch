using System.Text.Json.Serialization;

namespace Biomatch.Domain.Models;

public sealed record WordFrequency(string Word, int Frequency);

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(List<WordFrequency>))]
public sealed partial class WordFrequencySerializationContext : JsonSerializerContext;
