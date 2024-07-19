using System.Text.Json.Serialization;

namespace CronicleClient.Models;

internal record BaseEventResponse
{
  /// <summary>
  /// This will be set to 0 upon success, or any other value if an error occurred. In the event of an error, a description property will also be included, containing the error message itself.
  /// </summary>
  [JsonPropertyName("code")]
  [JsonConverter(typeof(Common.IntToStringJsonConverter))]
  public string Code { get; set; } = null!;
  
  /// <summary>
  /// In the event of an error, this will contain a description of the error.
  /// </summary>
  [JsonPropertyName("description")]
  public string? Description { get; set; }
}