using CronicleClient;
using CronicleClient.Models;
using FluentAssertions;
using Xunit.Abstractions;

namespace Tests;

[Collection("Get History")]
public class GetHistoryTest(ITestOutputHelper outputHelper)
{
  private readonly CancellationToken _cancellationToken = new CancellationTokenSource().Token;
  private readonly Client _cronicleClient = Common.InitCronicleClient(outputHelper);

  [Fact(DisplayName = "Get an history for all events in Cronicle")]
  public async Task GetExistentHistory()
  {
    // Arrange

    // Act
    var history = await _cronicleClient.Job.GetHistory(10, cancellationToken: _cancellationToken);

    // Assert
    history.Should().NotBeNull();
  }

  [Theory]
  [InlineData(50, 0)]
  [InlineData(10, 10)]
  public async Task GetHistoryWithLimitAndOffset(int limit, int offset)
  {
    // Arrange

    // Act
    var history = await _cronicleClient.Job.GetHistory(limit, offset, _cancellationToken);

    // Assert
    history.Should().NotBeNull();
  }


  [Fact(DisplayName = "Get an history for all events with validation")]
  public async Task GetNewEventsInHistory()
  {
    // Arrange
    var newEvent = new NewEvent
    {
      Title = "New title",
      Enabled = true,
      Category = "general",
      Plugin = "testplug",
      Target = "allgrp",
      Timing = new Timing
      {
        Hours = [4],
        Minutes = [27],
        Days = [25],
        Months = [8],
        Years = [2024]
      },
      Parameters = new Dictionary<string, object>
      {
        { "duration", 1 } // 1 second
      }
    };
    var eventId = await _cronicleClient.Event.Create(newEvent, _cancellationToken);
    eventId.Should().NotBeEmpty();

    var ids = await _cronicleClient.Event.RunEventById(eventId, _cancellationToken);
    await Task.Delay(2000, _cancellationToken);

    // Act
    var history = await _cronicleClient.Job.GetHistory(10, cancellationToken: _cancellationToken);

    // Assert
    history.Should().NotBeNull();
    history.Should().HaveCountGreaterThan(0);

    // Cleanup
    await _cronicleClient.Event.Delete(eventId, _cancellationToken);
  }

  [Fact(DisplayName = "Get an history for all events with aborted events")]
  public async Task GetNewAbortEventsInHistory()
  {
    // Arrange
    var newEvent = new NewEvent
    {
      Title = "New title",
      Enabled = true,
      Category = "general",
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

    var ids = await _cronicleClient.Event.RunEventById(eventId, _cancellationToken);
    await _cronicleClient.Job.AbortJob(ids.First(), _cancellationToken);
    await Task.Delay(1500, _cancellationToken);

    // Act
    var history = await _cronicleClient.Job.GetHistory(10, cancellationToken: _cancellationToken);

    // Assert
    history.Should().NotBeNull();
    var abortedJob = history.FirstOrDefault(j => j.Id == ids.First());
    abortedJob.Should().NotBeNull();
    abortedJob!.Description.Should().StartWith("Job Aborted");

    // Cleanup
    await _cronicleClient.Event.Delete(eventId, _cancellationToken);
  }
}