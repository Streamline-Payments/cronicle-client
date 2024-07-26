using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CronicleClient.Models;

internal record GetEventResponse : BaseEventResponse
{
  /// <summary>
  ///   When applicable, the data of an event.
  /// </summary>
  [JsonPropertyName("event")]
  public EventData? EventData { get; set; }

  public string ToJson()
  {
    return JsonSerializer.Serialize(this);
  }

  public GetEventResponse FromJson(string eventJson)
  {
    return JsonSerializer.Deserialize<GetEventResponse>(eventJson) ?? throw new ArgumentNullException(nameof(eventJson));
  }
}