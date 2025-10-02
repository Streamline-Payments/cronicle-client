using CronicleClient;
using Xunit.Abstractions;

namespace Tests;

internal static class Common
{
  public static void SetEnv()
  {
    foreach (var line in File.ReadAllLines(".env"))
    {
      var parts = line.Split('=', StringSplitOptions.RemoveEmptyEntries);

      if (parts.Length != 2)
        continue;

      Environment.SetEnvironmentVariable(parts[0], parts[1]);
    }
  }

  public static Client InitCronicleClient(ITestOutputHelper outputHelper)
  {
    SetEnv();

    var serverUrl = Environment.GetEnvironmentVariable("CRONICLE_BASE_URL") ?? throw new Exception("CRONICLE_BASE_URL is not set");
    var apiKey = Environment.GetEnvironmentVariable("CRONICLE_API_KEY") ?? throw new Exception("CRONICLE_API_KEY is not set");
    var logger = outputHelper.ToLogger<CreateEvent>();

    return new Client(serverUrl, apiKey, logger);
  }
}