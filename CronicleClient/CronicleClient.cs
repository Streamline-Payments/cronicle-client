using System;
using System.Net.Http;
using Microsoft.Extensions.Logging;

namespace CronicleClient;

/// <summary>
/// Cronicle API client
/// </summary>
/// <param name="baseUrl"></param>
/// <param name="apiToken"></param>
/// <param name="logger"></param>
public class Client(string baseUrl, string apiToken, ILogger logger)
{
  private readonly HttpClient _apiClient = new()
  {
    BaseAddress = new Uri(string.Concat(baseUrl.TrimEnd('/'), "/api/app/")),
    DefaultRequestHeaders = { { "X-API-Key", apiToken } }
  };
  
  /// <summary>
  /// A Cronicle event
  /// </summary>
  public CronicleEvent Event => new(_apiClient, logger);
  /// <summary>
  /// A Cronicle job
  /// </summary>
  public CronicleJob Job => new(_apiClient, logger);
  /// <summary>
  /// A Cronicle master
  /// </summary>
  public CronicleMaster Master => new(_apiClient, logger);
}