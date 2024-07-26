using System.Text.Json.Serialization;

namespace CronicleClient.Models;

internal record EventHistoryResponse : BaseEventResponse
{
  /// <summary>
  ///   A collection of previous event job data.
  /// </summary>
  [JsonPropertyName("rows")]
  public JobData[]? JobData { get; set; }

  /// <summary>
  ///   Details about the job data collection.
  /// </summary>
  [JsonPropertyName("list")]
  public ListHistoryDetails? ListDetails { get; set; }
}