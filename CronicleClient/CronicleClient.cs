using System;
using System.Net.Http;
using Microsoft.Extensions.Logging;

namespace CronicleClient;

public class Client(Uri baseUrl, string apiToken, ILogger logger)
{
  private readonly HttpClient _apiClient = new()
  {
    BaseAddress = baseUrl,
    DefaultRequestHeaders = { { "X-API-Key", apiToken } }
  };

  public CronicleEvent Event => new(_apiClient, logger);
  public CronicleJob Job => new(_apiClient, logger);
}