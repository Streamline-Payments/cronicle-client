using System;
using System.Net.Http;
using Microsoft.Extensions.Logging;

namespace CronicleClient;

public interface ICronicleClient
{
    CronicleEvent Event { get; }
    CronicleJob Job { get; }
    CronicleMaster Master { get; }
}


/// <summary>
/// Cronicle API client
/// </summary>
/// <param name="baseUrl"></param>
/// <param name="apiToken"></param>
/// <param name="logger"></param>
public class Client : ICronicleClient
{

    private readonly HttpClient _apiClient;
    private readonly ILogger _logger;

    public Client(string baseUrl, string apiToken, ILogger logger)
    {
        _logger = logger;
        _apiClient = new HttpClient
        {
            BaseAddress = new Uri(string.Concat(baseUrl.TrimEnd('/'), "/api/app/")),
            DefaultRequestHeaders = { { "X-API-Key", apiToken } }
        };
    }

  /// <summary>
  /// A Cronicle event
  /// </summary>
  public CronicleEvent Event => new(_apiClient, _logger);
  /// <summary>
  /// A Cronicle job
  /// </summary>
  public CronicleJob Job => new(_apiClient, _logger);
  /// <summary>
  /// A Cronicle master
  /// </summary>
  public CronicleMaster Master => new(_apiClient, _logger);
}