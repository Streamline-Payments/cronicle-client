using System.Text.Json.Serialization;

namespace CronicleClient.Models;

/// <summary>
/// A structured object containing the response from the RunEvent API endpoint.
/// </summary>
internal record RunEventResponse : BaseEventResponse
{
  /// <summary>
  ///   A collection of ids.
  /// </summary>
  [JsonPropertyName("ids")]
  public string[]? Ids { get; set; }
}