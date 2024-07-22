using System;
using System.Net.Http;
using Microsoft.Extensions.Logging;

namespace CronicleClient;

public class Client(string baseUrl, string apiToken, ILogger logger)
{
  private readonly HttpClient _apiClient = new()
  {
    BaseAddress = new Uri(string.Concat(baseUrl.TrimEnd('/'),"/api/app")),
    DefaultRequestHeaders = { { "X-API-Key", apiToken } }
  };

  public CronicleEvent Event => new(_apiClient, logger);
  public CronicleJob Job => new(_apiClient, logger);
}