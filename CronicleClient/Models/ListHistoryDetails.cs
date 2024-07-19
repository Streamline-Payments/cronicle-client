using System.Text.Json.Serialization;

namespace CronicleClient.Models;

public record ListHistoryDetails
{
  [JsonPropertyName("page_size")]
  public int? PageSize { get; set; }
  
  [JsonPropertyName("first_page")]
  public int? FirstPage { get; set; }
  
  [JsonPropertyName("last_page")]
  public int? LastPage { get; set; }
  
  [JsonPropertyName("length")]
  public int? Length { get; set; }
  
  [JsonPropertyName("type")]
  public string? ListType { get; set; }
}