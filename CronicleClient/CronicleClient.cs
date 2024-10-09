using System;
using System.Net.Http;
using CronicleClient.Interfaces;
using Microsoft.Extensions.Logging;

namespace CronicleClient;

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
    public ICronicleEvent Event => new CronicleEvent(_apiClient, _logger);
    /// <summary>
    /// A Cronicle job
    /// </summary>
    public ICronicleJob Job => new CronicleJob(_apiClient, _logger);
    /// <summary>
    /// A Cronicle master
    /// </summary>
    public ICronicleMaster Master => new CronicleMaster(_apiClient, _logger);
}