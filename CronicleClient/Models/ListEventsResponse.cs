using System.Text.Json.Serialization;

namespace CronicleClient.Models;

/// <summary>
/// ListEventsResponse represents the response from a list events request.
/// </summary>
internal record ListEventsResponse : BaseEventResponse
{
  /// <summary>
  ///   A collection of events.
  /// </summary>
  [JsonPropertyName("rows")]
  public EventData[]? EventDataCollection { get; set; }
}