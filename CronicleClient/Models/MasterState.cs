using System.Text.Json.Serialization;

namespace CronicleClient.Models;

/// <summary>
/// A structured object containing the response from the MasterState API endpoint.
/// </summary>
internal record MasterState : BaseEventResponse
{
  /// <summary>
  /// Cronicle master state.
  /// </summary>
  public State? State { get; set; }
}

/// <summary>
/// A structured object containing the state of the Cronicle master.
/// </summary>
internal record State
{
  /// <summary>
  /// A flag indicating whether the Cronicle master is enabled.
  /// </summary>
  [JsonPropertyName("enabled")]
  [JsonConverter(typeof(Common.BoolToIntJsonConverter))]
  public bool Enabled { get; set; }
}