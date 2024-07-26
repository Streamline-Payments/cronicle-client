using System.Text.Json.Serialization;

namespace CronicleClient.Models;

internal record ListEventsResponse: BaseEventResponse
{
  /// <summary>
  /// A collection of events.
  /// </summary>
  [JsonPropertyName("rows")]
  public EventData[]? EventDataCollection { get; set; }

    /// <summary>
    /// Contains internal metadata about the list structure in storage.
    /// </summary>
    [JsonPropertyName("list")]
    public ScheduleListDetails List { get; set; }
}

internal record ScheduleListDetails
{
    /// <summary>
    /// Page size
    /// </summary>
    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }

    /// <summary>
    /// First page of the list
    /// </summary>
    [JsonPropertyName("first_page")]
    public int FirstPage { get; set; }

    /// <summary>
    /// Last page of the list
    /// </summary>
    [JsonPropertyName("last_page")]
    public int LastPage { get; set; }

    /// <summary>
    /// Total number of events in the schedule
    /// </summary>
    [JsonPropertyName("length")]
    public int Length { get; set; }

    /// <summary>
    /// Type
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; }

}