using System.Text.Json.Serialization;

namespace CronicleClient.Models;

/// <summary>
/// ListHistoryDetails represents the details of a list history request.
/// </summary>
public record ListHistoryDetails
{
  /// <summary>
  /// Number of items to return per page.
  /// </summary>
  [JsonPropertyName("page_size")] public int? PageSize { get; set; }
  /// <summary>
  /// The first page of the list.
  /// </summary>
  [JsonPropertyName("first_page")] public int? FirstPage { get; set; }
  /// <summary>
  /// The last page of the list.
  /// </summary>
  [JsonPropertyName("last_page")] public int? LastPage { get; set; }
  /// <summary>
  /// The total number of items in the list.
  /// </summary>
  [JsonPropertyName("length")] public int? Length { get; set; }
  /// <summary>
  /// Type of list.
  /// </summary>
  [JsonPropertyName("type")] public string? ListType { get; set; }
}