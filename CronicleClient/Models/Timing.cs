using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CronicleClient.Models;

/// <summary>
/// Timing information for an event
/// </summary>
public record Timing
{
  /// <summary>
  /// Years to run the event
  /// </summary>
  [JsonPropertyName("years")] public List<int> Years { get; set; } = [];
  /// <summary>
  /// Months to run the event
  /// </summary>
  [JsonPropertyName("months")] public List<int> Months { get; set; } = [];
  /// <summary>
  /// Days to run the event
  /// </summary>
  [JsonPropertyName("days")] public List<int> Days { get; set; } = [];
  /// <summary>
  /// Weekdays to run the event
  /// </summary>
  [JsonPropertyName("weekdays")] public List<int> Weekdays { get; set; } = [];
  /// <summary>
  /// Hours to run the event
  /// </summary>
  [JsonPropertyName("hours")] public List<int> Hours { get; set; } = [];
  /// <summary>
  /// Minutes to run the event
  /// </summary>
  [JsonPropertyName("minutes")] public List<int> Minutes { get; set; } = [];
}