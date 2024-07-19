using CronicleClient;
using CronicleClient.Models;
using Microsoft.Extensions.Logging;

namespace Examples;

public class GetEvent(string croniclePrimaryServerUrl, string cronicleApiKey, ILogger logger)
{
  private readonly Client _cronicleClient = new Client(new Uri(croniclePrimaryServerUrl), cronicleApiKey, logger);

  public Task<EventData?> Get(string eventId, CancellationToken cancellationToken)
  {
    return _cronicleClient.Event.GetById(eventId: eventId, cancellationToken: cancellationToken);
  }
}