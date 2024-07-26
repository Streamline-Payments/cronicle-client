using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using CronicleClient.Models;
using Microsoft.Extensions.Logging;

namespace CronicleClient;

public class CronicleMaster
{
  private readonly HttpClient _httpClient;
  private readonly ILogger _logger;

  public CronicleMaster(HttpClient httpClient, ILogger logger)
  {
    _logger = logger;
    _httpClient = httpClient;
  }

  /// <summary>
  ///   This fetches the current application "state", which contains information like the status of the scheduler (enabled or disabled).
  /// </summary>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  public async Task<bool> GetMasterState(CancellationToken cancellationToken = default)
  {
    _logger.LogDebug("Fetching master state in Cronicle");
    var requestPathWithQuery = "get_master_state/v1";

    var resp = await _httpClient.GetFromJsonAsync<MasterState>(requestPathWithQuery, cancellationToken);
    resp.EnsureSuccessStatusCode();

    return resp!.State.Enabled;
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
    _logger.LogDebug("Update master state in Cronicle");

    State content = new()
    {
      Enabled = newMasterState
    };

    var resp = await _httpClient.PostAsJsonAsync("update_master_state/v1", content, cancellationToken);
    resp.EnsureSuccessStatusCode();

    var cronicleResponse = await resp.Content.ReadFromJsonAsync<BaseEventResponse>(cancellationToken);
    cronicleResponse.EnsureSuccessStatusCode();
  }
}