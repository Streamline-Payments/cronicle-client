using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using CronicleClient.Interfaces;
using CronicleClient.Models;
using Microsoft.Extensions.Logging;

namespace CronicleClient;

/// <summary>
/// Class for interacting with Cronicle jobs.
/// </summary>
public class CronicleJob : ICronicleJob
{

  private readonly HttpClient httpClient;
  private readonly ILogger logger;

  public CronicleJob(HttpClient httpClient, ILogger logger)
  {
     this.httpClient = httpClient;
     this.logger = logger;
  }

    private static void EnsureValidJobData(JobDataUpdateRequest jobData)
  {
    // These are required fields
    if (jobData == default) throw new ArgumentNullException(nameof(jobData));
    if (jobData.Id == default) throw new ArgumentNullException(nameof(jobData.Id));
  }

  /// <summary>
  ///   This fetches the event history (i.e. previously completed jobs) for a specific event. The response array is sorted by reverse timestamp (descending), so the
  ///   latest jobs are listed first.
  /// </summary>
  /// <param name="eventId"></param>
  /// <param name="numToFetch"></param>
  /// <param name="offset"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  /// <exception cref="ArgumentNullException"></exception>
  public async Task<JobData[]> GetByEventId(string eventId, int numToFetch, int offset = 0, CancellationToken cancellationToken = default)
  {
    if (eventId == default) throw new ArgumentNullException(nameof(eventId));

    logger.LogDebug($"Fetching job history for event '{eventId}' in Cronicle");
    var requestPathWithQuery = $"get_event_history/v1?id={eventId}&offset={offset}&limit={numToFetch}";

    var resp = await httpClient.GetFromJsonAsync<EventHistoryResponse>(requestPathWithQuery, cancellationToken);
    resp.EnsureSuccessStatusCode();
    return resp!.JobData ?? Array.Empty<JobData>();
  }


  /// <summary>
  ///   This fetches previously completed jobs for all events. The response array is sorted by reverse timestamp (descending), so the latest jobs are listed first
  /// </summary>
  /// <param name="limit"></param>
  /// <param name="offset"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  /// <exception cref="ArgumentNullException"></exception>
  public async Task<JobData[]> GetHistory(int limit, int offset = 0, CancellationToken cancellationToken = default)
  {
    logger.LogDebug("Fetching history for all events in Cronicle");
    var requestPathWithQuery = $"get_history/v1?offset={offset}&limit={limit}";

    var resp = await httpClient.GetFromJsonAsync<EventHistoryResponse>(requestPathWithQuery, cancellationToken);
    resp.EnsureSuccessStatusCode();
    return resp!.JobData ?? Array.Empty<JobData>();
  }


  /// <summary>
  ///   This fetches status for a job currently in progress, or one already completed. Both HTTP GET (query string) or HTTP POST (JSON data) are acceptable.
  /// </summary>
  /// <param name="jobId"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  /// <exception cref="ArgumentNullException"></exception>
  public async Task<JobData?> GetJobStatus(string jobId, CancellationToken cancellationToken = default)
  {
    logger.LogDebug("Fetching status for and specific job in Cronicle");
    var requestPathWithQuery = $"get_job_status/v1?id={jobId}";

    var resp = await httpClient.GetFromJsonAsync<JobResponse>(requestPathWithQuery, cancellationToken);
    resp.EnsureSuccessStatusCode();

    return resp!.Job ?? null;
  }

  /// <summary>
  ///   This fetches status for all active jobs, and returns them all at once. It takes no parameters (except an API Key of course).
  /// </summary>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  /// <exception cref="ArgumentNullException"></exception>
  public async Task<Dictionary<string, JobData>?> GetActiveJobs(CancellationToken cancellationToken = default)
  {
    logger.LogDebug("Fetching active jobs in Cronicle");
    var requestPathWithQuery = "get_active_jobs/v1";

    var resp = await httpClient.GetFromJsonAsync<JobsResponse>(requestPathWithQuery, cancellationToken);
    resp.EnsureSuccessStatusCode();

    return resp!.Jobs ?? null;
  }


  /// <summary>
  ///   This updates a job that is already in progress. Only certain job properties may be changed when the job is running, and those are listed below. This is
  ///   typically used to adjust timeouts, resource limits, or user notification settings. API Keys require the edit_events privilege to use this API. Only HTTP POST
  ///   (JSON data) is acceptable.
  /// </summary>
  /// <param name="jobData"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  /// <exception cref="ArgumentNullException"></exception>
  /// <exception cref="ArgumentOutOfRangeException"></exception>
  public async Task Update(JobDataUpdateRequest jobData, CancellationToken cancellationToken = default)
  {
    EnsureValidJobData(jobData);

    logger.LogDebug($"Updating job '{jobData.Id}' in Cronicle");
    var requestPathWithQuery = "update_job/v1";

    var resp = await httpClient.PostAsJsonAsync(requestPathWithQuery, jobData, cancellationToken);
    resp.EnsureSuccessStatusCode();

    var cronicleResponse = await resp.Content.ReadFromJsonAsync<BaseEventResponse>(cancellationToken);
    cronicleResponse.EnsureSuccessStatusCode();
  }

  /// <summary>
  ///   This aborts a running job given its ID. API Keys require the abort_events privilege to use this API. Only HTTP POST (JSON data) is acceptable.
  /// </summary>
  /// <param name="jobId"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  /// <exception cref="ArgumentNullException"></exception>
  public async Task AbortJob(string jobId, CancellationToken cancellationToken = default)
  {
    logger.LogDebug("Abort an specific job in Cronicle");

    object content = new
    {
      id = jobId
    };

    var resp = await httpClient.PostAsJsonAsync("abort_job/v1", content, cancellationToken);
    resp.EnsureSuccessStatusCode();

    var cronicleResponse = await resp.Content.ReadFromJsonAsync<BaseEventResponse>(cancellationToken);
    cronicleResponse.EnsureSuccessStatusCode();
  }
}