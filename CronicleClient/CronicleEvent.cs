using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using CronicleClient.Models;
using Microsoft.Extensions.Logging;

namespace CronicleClient;

public class CronicleEvent(HttpClient httpClient, ILogger logger)
{
  private static void EnsureValidEventData(EventData eventData)
  {
    // These are required fields
    if(eventData == default) throw new ArgumentNullException(nameof(eventData));
    if(eventData.Title == default) throw new ArgumentNullException(nameof(eventData.Title));
    if(eventData.Category == default) throw new ArgumentNullException(nameof(eventData.Category));
    if(eventData.Plugin == default) throw new ArgumentNullException(nameof(eventData.Plugin));
    if(eventData.Target == default) throw new ArgumentNullException(nameof(eventData.Target));
    if(eventData.Id == default) throw new ArgumentNullException(nameof(eventData.Id));
  }
  
  private static void EnsureValidEventData(NewEvent eventData)
  {
    // These are required fields
    if(eventData == default) throw new ArgumentNullException(nameof(eventData));
    if(eventData.Title == default) throw new ArgumentNullException(nameof(eventData.Title));
    if(eventData.Category == default) throw new ArgumentNullException(nameof(eventData.Category));
    if(eventData.Plugin == default) throw new ArgumentNullException(nameof(eventData.Plugin));
    if(eventData.Target == default) throw new ArgumentNullException(nameof(eventData.Target));
  }
  
  public async Task<(IEnumerable<EventData>, int)> GetSchedule(int limit = 50, int offset = 0, CancellationToken cancellationToken = default)
  {
    logger.LogDebug($"Fetching all events Cronicle");
    var resp = await httpClient.GetFromJsonAsync<ListEventsResponse>($"get_schedule/v1?offset={offset}&limit={limit}", cancellationToken);
    resp.EnsureSuccessStatusCode();
    return (resp?.EventDataCollection ?? new List<EventData>().AsEnumerable(), resp?.List.Length ?? 0);
    }
    /// <summary>
    /// This fetches all schedule and search and event by a filter asynchronously.
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IEnumerable<EventData>> SearchScheduleAsync(string titleFilter, Timing? dateFilter, CancellationToken cancellationToken = default)
    {
        int limit = 50;
        int offset = 0;
        titleFilter = titleFilter.ToLower();

        List<EventData> itemsFound = new List<EventData>();

        logger.LogDebug("Fetching search of events in Cronicle");

        var initialResponse = await httpClient.GetFromJsonAsync<ListEventsResponse>($"get_schedule/v1?offset={offset}&limit={limit}", cancellationToken);
        int totalEvents = initialResponse.List.Length;

        int numberOfRequests = 0;
        if (totalEvents % limit == 0)
        {
            numberOfRequests = totalEvents / limit;
        }
        else { 
            numberOfRequests = (totalEvents + limit - 1) / limit;
        }

        List<Task<List<EventData>>> tasks = new List<Task<List<EventData>>>();

        for (int i = 0; i < numberOfRequests; i++)
        {
            int currentOffset = i * limit;
            tasks.Add(FetchSchedule(currentOffset, limit, titleFilter, cancellationToken));
        }

        var results = await Task.WhenAll(tasks);
        itemsFound.AddRange(results.SelectMany(list => list).ToList());

        return itemsFound.AsEnumerable();
    }
    async Task<List<EventData>> FetchSchedule(int offset, int limit, string filter, CancellationToken cancellationToken)
    {        
        var (events, _) = await GetSchedule(limit, offset, cancellationToken);
       return events.Where(i => i.Title.ToLower().Contains(filter)).ToList();
    }

    public async Task<IEnumerable<EventData>> SearchSchedule(SearchScheduleRequest searchRequest, CancellationToken cancellationToken = default)
    {
        int limit = 50;
        int offset = 0;
        int length = 51;
        List<EventData> itemsFound = new List<EventData>();

        logger.LogDebug("Fetching search of events in Cronicle");

        var (_, totalEventsCount) = await GetSchedule(1, 0, cancellationToken);
        length = totalEventsCount;

        for (offset = 0; offset < length; offset += 50)
        {
            var (events, _) = await GetSchedule(limit, offset, cancellationToken);
            events = FilterSchedule(events.ToList(), searchRequest);
            itemsFound.AddRange(events);
        }

        return itemsFound.AsEnumerable();
    }

    public List<EventData> FilterSchedule(List<EventData> events, SearchScheduleRequest searchRequest)
    {
        if (events.Count() > 0 && searchRequest.title is not null)
        {
            events = events.Where(i => i.Title.Contains(searchRequest?.title)).ToList();
        }
        if (events.Count() > 0 && searchRequest.parameterKeyName is not null)
        {
            events = events.Where(i => i.Parameters.ContainsKey(searchRequest?.parameterKeyName)).ToList();
        }
        return events;
    }


    /// <summary>
    /// This fetches details about a single event, given its ID.
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
  /// This fetches details about a single event, given its exact title.
  /// </summary>
  /// <param name="eventTitle"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  public async Task<EventData?> GetByTitle(string eventTitle, CancellationToken cancellationToken = default)
  {
    logger.LogDebug($"Fetching event '{eventTitle}' from Cronicle");
    
    var content = new StringContent("{\"title\":\""+eventTitle+"\"}", System.Text.Encoding.UTF8, "application/json");
    
    var resp = await httpClient.PostAsync("get_event/v1", content, cancellationToken);
    resp.EnsureSuccessStatusCode();
    
    var cronicleResponse = await resp.Content.ReadFromJsonAsync<GetEventResponse>(cancellationToken: cancellationToken);
    cronicleResponse.EnsureSuccessStatusCode();
    
    return cronicleResponse?.EventData;
  }
  
  /// <summary>
  /// This creates a new event and adds it to the schedule.
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
    
    var cronicleResponse = await resp.Content.ReadFromJsonAsync<CreateEventResponse>(cancellationToken: cancellationToken);
    cronicleResponse.EnsureSuccessStatusCode();

    if (string.IsNullOrEmpty(cronicleResponse!.Id))
    {
      throw new Exception("Cronicle event was created but the id was not provided");
    }
    
    return cronicleResponse.Id;
  }
  
  /// <summary>
  /// This updates an existing event given its ID, replacing any properties you specify. 
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

    var cronicleResponse = await resp.Content.ReadFromJsonAsync<BaseEventResponse>(cancellationToken: cancellationToken);
    cronicleResponse.EnsureSuccessStatusCode();
  }
  
  /// <summary>
  /// This deletes an existing event given its ID. Note that the event must not have any active jobs still running (or else an error will be returned).
  /// </summary>
  /// <param name="eventId"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  /// <exception cref="ArgumentNullException"></exception>
  public async Task Delete(string eventId, CancellationToken cancellationToken = default)
  {
    if(eventId == default) throw new ArgumentNullException(nameof(eventId));
    
    logger.LogDebug($"Deleting event '{eventId}' in Cronicle");
    
    var content = new StringContent("{\"id\":\""+eventId+"\"}", System.Text.Encoding.UTF8, "application/json");
    
    var resp = await httpClient.PostAsync($"delete_event/v1", content, cancellationToken);
    resp.EnsureSuccessStatusCode();

    var cronicleResponse = await resp.Content.ReadFromJsonAsync<BaseEventResponse>(cancellationToken: cancellationToken);
    //if (cronicleResponse.Description?.Contains("Failed to locate event") == true)
    //{
    //    throw new KeyNotFoundException();
    //}
    cronicleResponse.EnsureSuccessStatusCode();
  }

    /// <summary>
    /// This runs an event, given its ID.
    /// </summary>
    /// <param name="eventId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The list of IDs of the new jobs</returns>
    public async Task<string[]> RunEventById(string eventId, CancellationToken cancellationToken = default)
    {
        logger.LogDebug($"Running event '{eventId}' from Cronicle");
        var resp = await httpClient.GetAsync($"run_event/v1?id={eventId}", cancellationToken);
        resp.EnsureSuccessStatusCode();

        var cronicleResponse = await resp.Content.ReadFromJsonAsync<RunEventResponse>(cancellationToken: cancellationToken);
        cronicleResponse.EnsureSuccessStatusCode();
        return cronicleResponse.Ids;
    }

    /// <summary>
    /// This runs event, given its exact title.
    /// </summary>
    /// <param name="eventTitle"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The list of IDs of the new jobs</returns>
    public async Task<string[]> RunEventByTitle(string eventTitle, CancellationToken cancellationToken = default)
    {
        logger.LogDebug($"Running event '{eventTitle}' from Cronicle");

        var content = new StringContent("{\"title\":\"" + eventTitle + "\"}", System.Text.Encoding.UTF8, "application/json");

        var resp = await httpClient.PostAsync("run_event/v1", content, cancellationToken);
        resp.EnsureSuccessStatusCode();

        var cronicleResponse = await resp.Content.ReadFromJsonAsync<RunEventResponse>(cancellationToken: cancellationToken);
        cronicleResponse.EnsureSuccessStatusCode();
        return cronicleResponse.Ids;
    }
}