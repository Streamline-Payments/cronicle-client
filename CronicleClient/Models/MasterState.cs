using System.Text.Json.Serialization;

namespace CronicleClient.Models;

internal record MasterState : BaseEventResponse
{
  public State State { get; set; }
}

internal record State
{
  [JsonPropertyName("enabled")]
  [JsonConverter(typeof(Common.BoolToIntJsonConverter))]
  public bool Enabled { get; set; }
}