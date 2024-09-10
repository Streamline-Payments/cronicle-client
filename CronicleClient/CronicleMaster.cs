using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using CronicleClient.Models;
using Microsoft.Extensions.Logging;

namespace CronicleClient;

public interface ICronicleMaster
{
    Task<bool?> GetMasterState(CancellationToken cancellationToken = default);
    Task UpdateMasterState(bool newMasterState, CancellationToken cancellationToken = default);
}


/// <summary>
/// A class that represents the Cronicle Master API.
/// </summary>
/// <param name="httpClient"></param>
/// <param name="logger"></param>
public class CronicleMaster : ICronicleMaster
{

    private readonly HttpClient httpClient;
    private readonly ILogger logger;

    public CronicleMaster(HttpClient httpClient, ILogger logger)
    {
        this.httpClient = httpClient;
        this.logger = logger;
    }


    /// <summary>
    ///   This fetches the current application "state", which contains information like the status of the scheduler (enabled or disabled).
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool?> GetMasterState(CancellationToken cancellationToken = default)
  {
    logger.LogDebug("Fetching master state in Cronicle");
    var requestPathWithQuery = "get_master_state/v1";

    var resp = await httpClient.GetFromJsonAsync<MasterState>(requestPathWithQuery, cancellationToken);
    resp.EnsureSuccessStatusCode();

    return resp!.State?.Enabled;
  }

  /// <summary>
  ///   This updates the master application state, i.e. toggling the scheduler on/off. API Keys require the state_update privilege to use this API.
  /// </summary>
  /// <param name="newMasterState"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  /// <exception cref="ArgumentNullException"></exception>
  public async Task UpdateMasterState(bool newMasterState, CancellationToken cancellationToken = default)
  {
    logger.LogDebug("Update master state in Cronicle");

    State content = new()
    {
      Enabled = newMasterState
    };

    var resp = await httpClient.PostAsJsonAsync("update_master_state/v1", content, cancellationToken);
    resp.EnsureSuccessStatusCode();

    var cronicleResponse = await resp.Content.ReadFromJsonAsync<BaseEventResponse>(cancellationToken);
    cronicleResponse.EnsureSuccessStatusCode();
  }
}