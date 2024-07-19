using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CronicleClient.Models;

public record Timing
{
  [JsonPropertyName("years")] public List<int> Years { get; set; } = [];
  [JsonPropertyName("months")] public List<int> Months { get; set; } = [];
  [JsonPropertyName("days")] public List<int> Days { get; set; } = [];
  [JsonPropertyName("weekdays")] public List<int> Weekdays { get; set; } = [];
  [JsonPropertyName("hours")] public List<int> Hours { get; set; } = [];
  [JsonPropertyName("minutes")] public List<int> Minutes { get; set; } = [];
}