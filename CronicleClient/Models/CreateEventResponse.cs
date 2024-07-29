using System.Text.Json.Serialization;

namespace CronicleClient.Models;

/// <summary>
/// A structured object containing the response from the CreateEvent API endpoint.
/// </summary>
internal record CreateEventResponse : BaseEventResponse
{
  /// <summary>
  ///   The id of the new event.
  /// </summary>
  [JsonPropertyName("id")]
  public string? Id { get; set; }
}