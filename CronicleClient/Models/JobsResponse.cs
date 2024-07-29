using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CronicleClient.Models;

/// <summary>
/// A structured object containing the response from the Jobs API endpoint.
/// </summary>
internal record JobsResponse : BaseEventResponse
{
    /// <summary>
    ///   Collection of jobs
    /// </summary>
    [JsonPropertyName("jobs")]
  public Dictionary<string, JobData>? Jobs { get; set; }
}