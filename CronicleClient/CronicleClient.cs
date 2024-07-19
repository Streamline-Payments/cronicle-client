using System;
using System.Net.Http;
using Microsoft.Extensions.Logging;

namespace CronicleClient;

public class Client
{
  private readonly ILogger _logger;
  private readonly HttpClient _apiClient;

  public Client(Uri baseUrl, string apiToken, ILogger logger)
  {
    _logger = logger;
    _apiClient = new HttpClient()
    {
      BaseAddress = baseUrl,
      DefaultRequestHeaders = { { "X-API-Key", apiToken } }
    };
  }
  
  public CronicleEvent Event => new(_apiClient, _logger);
  public CronicleJob Job => new(_apiClient, _logger);
}