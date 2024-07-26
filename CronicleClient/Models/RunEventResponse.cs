using System.Text.Json.Serialization;

namespace CronicleClient.Models;

internal record RunEventResponse : BaseEventResponse
{
  /// <summary>
  ///   A collection of ids.
  /// </summary>
  [JsonPropertyName("ids")]
  public string[]? Ids { get; set; }
}