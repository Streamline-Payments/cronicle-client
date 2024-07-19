using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using CronicleClient.Models;
using Microsoft.Extensions.Logging;

namespace CronicleClient;

public class CronicleJob
{
  private readonly ILogger _logger;
  private readonly HttpClient _httpClient;

  public CronicleJob(HttpClient httpClient, ILogger logger)
  {
    _logger = logger;
    _httpClient = httpClient;
  }
  
  /// <summary>
  /// This fetches the event history (i.e. previously completed jobs) for a specific event. The response array is sorted by reverse timestamp (descending), so the latest jobs are listed first.
  /// </summary>
  /// <param name="eventId"></param>
  /// <param name="numToFetch"></param>
  /// <param name="offset"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  /// <exception cref="ArgumentNullException"></exception>
  public async Task<JobData[]> GetByEventId(string eventId, int numToFetch, int offset = 0, CancellationToken cancellationToken = default)
  {
    if(eventId == default) throw new ArgumentNullException(nameof(eventId));
    
    _logger.LogDebug($"Fetching job history for event '{eventId}' in Cronicle");
    var requestPathWithQuery = $"get_event_history/v1?id={eventId}&offset={offset}&limit={numToFetch}";

    var resp = await _httpClient.GetFromJsonAsync<EventHistoryResponse>(requestPathWithQuery, cancellationToken);
    resp.EnsureSuccessStatusCode();
    return resp!.JobData ?? Array.Empty<JobData>();
  }
}