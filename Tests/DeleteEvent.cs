using CronicleClient;
using CronicleClient.Models;
using FluentAssertions;
using Xunit.Abstractions;

namespace Tests;

[Collection("Cronicle Client collection")]
public class DeleteEvent(ITestOutputHelper outputHelper)
{
  private readonly CancellationToken _cancellationToken = new CancellationTokenSource().Token;
  private readonly Client _cronicleClient = Common.InitCronicleClient(outputHelper);

  [Fact(DisplayName = "Delete an event")]
  public async Task DeleteEventById()
  {
    // Arrange
    var newEvent = new NewEvent
    {
      Title = "A title for delete",
      Enabled = true,
      Category = "cmga2zvlc1j",
      Plugin = "testplug",
      Target = "allgrp",
      Timing = new Timing
      {
        Hours = [4],
        Minutes = [27],
        Days = [25],
        Months = [8],
        Years = [2024]
      }
    };
    var eventId = await _cronicleClient.Event.Create(newEvent, _cancellationToken);
    eventId.Should().NotBeEmpty();

    // Act
    await _cronicleClient.Event.Delete(eventId, _cancellationToken);
  }

  [Fact(DisplayName = "Delete an event that does not exist")]
  public async Task DeleteNotExistentEvent()
  {
    // Arrange
    var eventId = "000000";

    // Act & Assert
    await FluentActions.Invoking(() => _cronicleClient.Event.Delete(eventId, _cancellationToken))
      .Should().ThrowAsync<KeyNotFoundException>();
  }

  [Fact(DisplayName = "Delete an event and Validate")]
  public async Task DeleteEventByIdValidate()
  {
    // Arrange
    var newEvent = new NewEvent
    {
      Title = "A title for delete",
      Enabled = true,
      Category = "cmga2zvlc1j",
      Plugin = "testplug",
      Target = "allgrp",
      Timing = new Timing
      {
        Hours = [4],
        Minutes = [27],
        Days = [25],
        Months = [8],
        Years = [2024]
      }
    };
    var eventId = await _cronicleClient.Event.Create(newEvent, _cancellationToken);
    eventId.Should().NotBeEmpty();

    // Act
    await _cronicleClient.Event.Delete(eventId, _cancellationToken);

    // Assert
    await FluentActions.Invoking(() => _cronicleClient.Event.GetById(eventId, _cancellationToken))
      .Should().ThrowAsync<KeyNotFoundException>();
  }


  [Fact(DisplayName = "Delete an event with an active job")]
  public async Task DeleteWithActiveJob()
  {
    var newEvent = new NewEvent
    {
      Title = "A title for delete",
      Enabled = true,
      Category = "cmga2zvlc1j",
      Plugin = "testplug",
      Target = "allgrp",
      Timing = new Timing
      {
        Hours = [4],
        Minutes = [27],
        Days = [25],
        Months = [8],
        Years = [2024]
      }
    };
    var eventId = await _cronicleClient.Event.Create(newEvent, _cancellationToken);
    eventId.Should().NotBeEmpty();

    var jobId = await _cronicleClient.Event.RunEventById(eventId, _cancellationToken);

    // Act & Assert
    await FluentActions.Invoking(() => _cronicleClient.Event.Delete(eventId, _cancellationToken))
      .Should().ThrowAsync<Exception>();

    // Cleanup
    await _cronicleClient.Job.AbortJob(jobId.First());

    await Task.Delay(1500);
    await _cronicleClient.Event.Delete(eventId, _cancellationToken);
  }
}