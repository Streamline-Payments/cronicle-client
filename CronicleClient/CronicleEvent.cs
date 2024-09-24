using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CronicleClient.Interfaces;
using CronicleClient.Models;
using Microsoft.Extensions.Logging;

namespace CronicleClient;


/// <summary>
/// Class for interacting with Cronicle events.
/// </summary>
/// <param name="httpClient"></param>
/// <param name="logger"></param>
public class CronicleEvent: ICronicleEvent
{
  private readonly HttpClient httpClient;
  private readonly ILogger logger;

  public CronicleEvent(HttpClient httpClient, ILogger logger)
  {
     this.httpClient = httpClient;
     this.logger = logger;
  }
  private static void EnsureValidEventData(EventData eventData)
  {
    // These are required fields
    if (eventData == default) throw new ArgumentNullException(nameof(eventData));
    if (eventData.Title == default) throw new ArgumentNullException(nameof(eventData.Title));
    if (eventData.Category == default) throw new ArgumentNullException(nameof(eventData.Category));
    if (eventData.Plugin == default) throw new ArgumentNullException(nameof(eventData.Plugin));
    if (eventData.Target == default) throw new ArgumentNullException(nameof(eventData.Target));
    if (eventData.Id == default) throw new ArgumentNullException(nameof(eventData.Id));
  }

  private static void EnsureValidEventData(NewEvent eventData)
  {
    // These are required fields
    if (eventData == default) throw new ArgumentNullException(nameof(eventData));
    if (eventData.Title == default) throw new ArgumentNullException(nameof(eventData.Title));
    if (eventData.Category == default) throw new ArgumentNullException(nameof(eventData.Category));
    if (eventData.Plugin == default) throw new ArgumentNullException(nameof(eventData.Plugin));
    if (eventData.Target == default) throw new ArgumentNullException(nameof(eventData.Target));
  }

  /// <summary>
  /// This fetches all events in the schedule, up to the specified limit.
  /// </summary>
  /// <param name="limit"></param>
  /// <param name="offset"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  public async Task<IEnumerable<EventData>> GetSchedule(int limit = 50, int offset = 0, CancellationToken cancellationToken = default)
  {
    logger.LogDebug("Fetching all events Cronicle");
    var resp = await httpClient.GetFromJsonAsync<ListEventsResponse>($"get_schedule/v1?offset={offset}&limit={limit}", cancellationToken);
    resp.EnsureSuccessStatusCode();
    return resp?.EventDataCollection ?? new List<EventData>().AsEnumerable();
  }

  /// <summary>
  ///   This fetches details about a single event, given its ID.
  /// </summary>
  /// <param name="eventId"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  public async Task<EventData?> GetById(string eventId, CancellationToken cancellationToken = default)
  {
    logger.LogDebug($"Fetching event '{eventId}' from Cronicle");
    var resp = await httpClient.GetFromJsonAsync<GetEventResponse>($"get_event/v1?id={eventId}", cancellationToken);
    resp.EnsureSuccessStatusCode();
    return resp?.EventData;
  }

  /// <summary>
  ///   This fetches details about a single event, given its exact title.
  /// </summary>
  /// <param name="eventTitle"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  public async Task<EventData?> GetByTitle(string eventTitle, CancellationToken cancellationToken = default)
  {
    logger.LogDebug($"Fetching event '{eventTitle}' from Cronicle");

    var content = new StringContent("{\"title\":\"" + eventTitle + "\"}", Encoding.UTF8, "application/json");

    var resp = await httpClient.PostAsync("get_event/v1", content, cancellationToken);
    resp.EnsureSuccessStatusCode();

    var cronicleResponse = await resp.Content.ReadFromJsonAsync<GetEventResponse>(cancellationToken);
    cronicleResponse.EnsureSuccessStatusCode();

    return cronicleResponse?.EventData;
  }

  /// <summary>
  ///   This creates a new event and adds it to the schedule.
  /// </summary>
  /// <param name="eventData"></param>
  /// <param name="cancellationToken"></param>
  /// <returns>The ID of the new event</returns>
  /// <exception cref="ArgumentNullException"></exception>
  /// <exception cref="ArgumentOutOfRangeException"></exception>
  public async Task<string> Create(NewEvent eventData, CancellationToken cancellationToken = default)
  {
    EnsureValidEventData(eventData);

    logger.LogDebug($"Creating event '{eventData.Title}' in Cronicle");
    var resp = await httpClient.PostAsJsonAsync("create_event/v1", eventData, cancellationToken);
    resp.EnsureSuccessStatusCode();

    var cronicleResponse = await resp.Content.ReadFromJsonAsync<CreateEventResponse>(cancellationToken);
    cronicleResponse.EnsureSuccessStatusCode();

    if (string.IsNullOrEmpty(cronicleResponse!.Id)) throw new Exception("Cronicle event was created but the id was not provided");

    return cronicleResponse.Id;
  }

  /// <summary>
  ///   This updates an existing event given its ID, replacing any properties you specify.
  /// </summary>
  /// <param name="eventData"></param>
  /// <param name="resetCursor"></param>
  /// <param name="abortJobs"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  /// <exception cref="ArgumentNullException"></exception>
  /// <exception cref="ArgumentOutOfRangeException"></exception>
  public async Task Update(EventData eventData, bool resetCursor = false, bool abortJobs = false, CancellationToken cancellationToken = default)
  {
    EnsureValidEventData(eventData);

    logger.LogDebug($"Updating event '{eventData.Title}' in Cronicle");
    var requestPathWithQuery = $"update_event/v1?id={eventData.Id}&reset_cursor={(resetCursor ? "1" : "0")}&abort_jobs={(abortJobs ? "1" : "0")}";

    var resp = await httpClient.PutAsJsonAsync(requestPathWithQuery, eventData, cancellationToken);
    resp.EnsureSuccessStatusCode();

    var cronicleResponse = await resp.Content.ReadFromJsonAsync<BaseEventResponse>(cancellationToken);
    cronicleResponse.EnsureSuccessStatusCode();
  }

  /// <summary>
  ///   This deletes an existing event given its ID. Note that the event must not have any active jobs still running (or else an error will be returned).
  /// </summary>
  /// <param name="eventId"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  /// <exception cref="ArgumentNullException"></exception>
  public async Task Delete(string eventId, CancellationToken cancellationToken = default)
  {
    if (eventId == default) throw new ArgumentNullException(nameof(eventId));

    logger.LogDebug($"Deleting event '{eventId}' in Cronicle");

    var content = new StringContent("{\"id\":\"" + eventId + "\"}", Encoding.UTF8, "application/json");

    var resp = await httpClient.PostAsync("delete_event/v1", content, cancellationToken);
    resp.EnsureSuccessStatusCode();

    var cronicleResponse = await resp.Content.ReadFromJsonAsync<BaseEventResponse>(cancellationToken);
    //if (cronicleResponse.Description?.Contains("Failed to locate event") == true)
    //{
    //    throw new KeyNotFoundException();
    //}
    cronicleResponse.EnsureSuccessStatusCode();
  }

  /// <summary>
  ///   This runs an event, given its ID.
  /// </summary>
  /// <param name="eventId"></param>
  /// <param name="cancellationToken"></param>
  /// <returns>The list of IDs of the new jobs</returns>
  public async Task<string[]?> RunEventById(string eventId, CancellationToken cancellationToken = default)
  {
    logger.LogDebug($"Running event '{eventId}' from Cronicle");
    var resp = await httpClient.GetAsync($"run_event/v1?id={eventId}", cancellationToken);
    resp.EnsureSuccessStatusCode();

    var cronicleResponse = await resp.Content.ReadFromJsonAsync<RunEventResponse>(cancellationToken);
    cronicleResponse.EnsureSuccessStatusCode();
    return cronicleResponse?.Ids;
  }

    /// <summary>
    ///   This runs an event, given its ID.
    /// </summary>
    /// <param name="eventId"></param>
    /// <param name="eventData"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The list of IDs of the new jobs</returns>
    public async Task<string[]?> RunEventById(string eventId, EventData eventData, CancellationToken cancellationToken = default)
    {
         logger.LogDebug($"Running event '{eventId}' from Cronicle");
         var resp = await httpClient.PostAsJsonAsync($"run_event/v1?id={eventId}", eventData, cancellationToken);
         resp.EnsureSuccessStatusCode();

         var cronicleResponse = await resp.Content.ReadFromJsonAsync<RunEventResponse>(cancellationToken);
         cronicleResponse.EnsureSuccessStatusCode();
         return cronicleResponse?.Ids;
    }

    /// <summary>
    ///   This runs event, given its exact title.
    /// </summary>
    /// <param name="eventTitle"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The list of IDs of the new jobs</returns>
    public async Task<string[]?> RunEventByTitle(string eventTitle, CancellationToken cancellationToken = default)
  {
    logger.LogDebug($"Running event '{eventTitle}' from Cronicle");

    var content = new StringContent("{\"title\":\"" + eventTitle + "\"}", Encoding.UTF8, "application/json");

    var resp = await httpClient.PostAsync("run_event/v1", content, cancellationToken);
    resp.EnsureSuccessStatusCode();

    var cronicleResponse = await resp.Content.ReadFromJsonAsync<RunEventResponse>(cancellationToken);
    cronicleResponse.EnsureSuccessStatusCode();
    return cronicleResponse?.Ids;
  }
}