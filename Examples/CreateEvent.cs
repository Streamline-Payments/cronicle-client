using CronicleClient;
using CronicleClient.Models;
using Microsoft.Extensions.Logging;

namespace Examples;

public class CreateEvent(string croniclePrimaryServerUrl, string cronicleApiKey, ILogger logger)
{
  private readonly Client _cronicleClient = new Client(new Uri(croniclePrimaryServerUrl), cronicleApiKey, logger);

  /// <summary>
  ///   Every year on Christmas at midnight UTC
  /// </summary>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  public Task<string> OnlyOnChristmasUTC(CancellationToken cancellationToken)
  {
    var cronicleEvent = new NewEvent
    {
      Title = "This is my event",
      Category = "general",
      Enabled = true,
      Plugin = "XXXXX",
      Target = "allgrp",
      Parameters = new Dictionary<string, string>
      {
        { "SOMETHING", "123asd" }
      },
      Timing = new Timing
      {
        Months = new List<int> { 12 },
        Days = new List<int> { 25 },
        Hours = new List<int> { 11 },
        Minutes = new List<int> { 59 }
      },
      Timeout = 60, // 1 minute
      Retries = 0, // don't retry on failure
      Timezone = "UTC"
    };

    return _cronicleClient.Event.Create(cronicleEvent, cancellationToken);
  }

  /// <summary>
  ///   The 15th of every month at 3:15 PM UTC
  /// </summary>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  public Task<string> FifteenthMonthly(CancellationToken cancellationToken)
  {
    var cronicleEvent = new NewEvent
    {
      Title = "This is my event",
      Category = "general",
      Enabled = true,
      Plugin = "XXXXX",
      Target = "allgrp",
      Parameters = new Dictionary<string, string>
      {
        { "SOMETHING", "123asd" }
      },
      Timing = new Timing
      {
        Days = new List<int> { 15 },
        Hours = new List<int> { 3 },
        Minutes = new List<int> { 15 }
      },
      Timeout = 60, // 1 minute
      Retries = 0, // don't retry on failure
      Timezone = "UTC"
    };

    return _cronicleClient.Event.Create(cronicleEvent, cancellationToken);
  }
}