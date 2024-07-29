using System.Text.Json.Serialization;

namespace CronicleClient.Models;

/// <summary>
/// A structured object containing the response from the Job API endpoint.
/// </summary>
internal record JobResponse : BaseEventResponse
{
  /// <summary>
  ///   A job.
  /// </summary>
  [JsonPropertyName("job")]
  public JobData? Job { get; set; }
}