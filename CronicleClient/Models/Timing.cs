using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CronicleClient.Models;

public record Timing
{
  [JsonPropertyName("years")] public List<int> Years { get; set; } = new ();
  [JsonPropertyName("months")] public List<int> Months { get; set; } = new ();
  [JsonPropertyName("days")] public List<int> Days { get; set; } = new ();
  [JsonPropertyName("weekdays")] public List<int> Weekdays { get; set; } = new ();
  [JsonPropertyName("hours")] public List<int> Hours { get; set; } = new ();
  [JsonPropertyName("minutes")] public List<int> Minutes { get; set; } = new ();
}