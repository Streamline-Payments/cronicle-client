using CronicleClient;
using CronicleClient.Models;
using FluentAssertions;
using Xunit.Abstractions;

namespace Tests;

[Collection("Cronicle Client collection")]
public class GetJobs(ITestOutputHelper outputHelper)
{
  private readonly CancellationToken _cancellationToken = new CancellationTokenSource().Token;
  private readonly Client _cronicleClient = Common.InitCronicleClient(outputHelper);

  [Fact(DisplayName = "Get jobs")]
  public async Task GetJobsEvent()
  {
    // Arrange
    var newEvent = new NewEvent
    {
      Title = "New title",
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
    var resultJobs = await _cronicleClient.Job.GetByEventId(eventId, 10, cancellationToken: _cancellationToken);

    // Assert
    resultJobs.Should().NotBeNull();
    foreach (var jobId in ids)
    {
      var findJob = resultJobs.FirstOrDefault(j => j.Id == jobId);
      findJob.Should().NotBeNull();
    }

    // Cleanup
    await _cronicleClient.Event.Delete(eventId, _cancellationToken);
  }


  [Fact(DisplayName = "Get aborted jobs")]
  public async Task GetAbortedJobs()
  {
    // Arrange
    var newEvent = new NewEvent
    {
      Title = "New title",
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
      },
      Parameters = new Dictionary<string, object>
      {
        { "duration", 1 } // 1 seconds
      }
    };
    var eventId = await _cronicleClient.Event.Create(newEvent, _cancellationToken);
    eventId.Should().NotBeEmpty();

    var ids = await _cronicleClient.Event.RunEventById(eventId, _cancellationToken);
    await _cronicleClient.Job.AbortJob(ids.First(), _cancellationToken);
    await Task.Delay(2000, _cancellationToken);

    // Act
    var resultJobs = await _cronicleClient.Job.GetByEventId(eventId, 1, cancellationToken: _cancellationToken);

    // Assert
    resultJobs.Should().NotBeNull();

    var findJob = resultJobs.FirstOrDefault(j => j.Id == ids.First());
    findJob.Should().NotBeNull();

    // Cleanup
    await _cronicleClient.Event.Delete(eventId, _cancellationToken);
  }

  [Theory]
  [InlineData(50, 0)]
  public async Task GetJobsEventWithLimitAndOffset(int limit, int offset)
  {
    // Arrange
    var newEvent = new NewEvent
    {
      Title = "New title",
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
    var resultJobs = await _cronicleClient.Job.GetByEventId(eventId, limit, offset, _cancellationToken);

    // Assert
    resultJobs.Should().NotBeNull();

    var findJob = resultJobs.FirstOrDefault(j => j.Id == ids.First());
    findJob.Should().NotBeNull();

    // Cleanup
    await _cronicleClient.Event.Delete(eventId, _cancellationToken);
  }


  [Fact(DisplayName = "Get event history when no jobs are completed")]
  public async Task GetEventHistoryWhenNoJobsCompleted()
  {
    // Arrange
    var newEvent = new NewEvent
    {
      Title = "No Completed Jobs",
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
        Years = []
      }
    };
    var eventId = await _cronicleClient.Event.Create(newEvent, _cancellationToken);
    eventId.Should().NotBeEmpty();

    // Act
    var resultJobs = await _cronicleClient.Job.GetByEventId(eventId, 10, cancellationToken: _cancellationToken);

    // Assert
    resultJobs.Should().NotBeNull();
    resultJobs.Should().BeEmpty();

    // Cleanup
    await _cronicleClient.Event.Delete(eventId, _cancellationToken);
  }
}