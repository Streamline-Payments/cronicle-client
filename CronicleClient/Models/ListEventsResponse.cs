using System.Text.Json.Serialization;

namespace CronicleClient.Models;

internal record ListEventsResponse : BaseEventResponse
{
  /// <summary>
  ///   A collection of events.
  /// </summary>
  [JsonPropertyName("rows")]
  public EventData[]? EventDataCollection { get; set; }
}