using System.Text.Json.Serialization;

namespace CronicleClient.Models;

internal record CreateEventResponse: BaseEventResponse
{
  /// <summary>
  /// The id of the new event.
  /// </summary>
  [JsonPropertyName("id")]
  public string? Id { get; set; }
}