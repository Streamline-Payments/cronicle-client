using CronicleClient;
using CronicleClient.Models;
using FluentAssertions;
using Xunit.Abstractions;

namespace Tests;

[Collection("Cronicle Client collection")]
public class GetJobStatus(ITestOutputHelper outputHelper)
{
  private readonly CancellationToken _cancellationToken = new CancellationTokenSource().Token;
  private readonly Client _cronicleClient = Common.InitCronicleClient(outputHelper);

  [Fact(DisplayName = "Get job status")]
  public async Task GetJobsStatus()
  {
    // Arrange
    var newEvent = new NewEvent
    {
      Title = "A job event",
      Enabled = true,
      Category = "general",
      Plugin = "testplug",
      Target = "allgrp",
      Timing = new Timing
      {
        Hours = [4],
        Minutes = [27],
        Days = [5],
        Months = [1],
        Years = [2024]
      }
    };

    var eventId = await _cronicleClient.Event.Create(newEvent);
    eventId.Should().NotBeEmpty();

    var ids = await _cronicleClient.Event.RunEventById(eventId, _cancellationToken);

    // Act
    var jobData = await _cronicleClient.Job.GetJobStatus(ids.First(), _cancellationToken);
    jobData.Should().NotBeNull();

    // Cleanup
    await _cronicleClient.Job.AbortJob(ids.First(), _cancellationToken);
    await Task.Delay(1500);
    await _cronicleClient.Event.Delete(eventId, _cancellationToken);
  }

  [Fact(DisplayName = "Get job status for non-existent job")]
  public async Task GetJobStatusForNonExistentJob()
  {
    // Arrange
    var nonExistentJobId = "nonExistentJobId";

    // Act & Assert
    await FluentActions.Invoking(() => _cronicleClient.Job.GetJobStatus(nonExistentJobId, _cancellationToken))
      .Should().ThrowAsync<Exception>();
  }

  [Fact(DisplayName = "Get job status for aborted job")]
  public async Task GetJobStatusForAbortedJob()
  {
    // Arrange
    var newEvent = new NewEvent
    {
      Title = "Abort job event",
      Enabled = true,
      Category = "general",
      Plugin = "testplug",
      Target = "allgrp",
      Timing = new Timing
      {
        Hours = [4],
        Minutes = [27],
        Days = [5],
        Months = [1],
        Years = [2024]
      }
    };

    var eventId = await _cronicleClient.Event.Create(newEvent, _cancellationToken);
    eventId.Should().NotBeEmpty();

    var ids = await _cronicleClient.Event.RunEventById(eventId, _cancellationToken);
    await _cronicleClient.Job.AbortJob(ids.First(), _cancellationToken);
    await Task.Delay(1500);

    // Act
    var jobData = await _cronicleClient.Job.GetJobStatus(ids.First(), _cancellationToken);

    // Assert
    jobData.Should().NotBeNull();
    jobData.AbortReason.Should().NotBeNull();

    // Cleanup
    await _cronicleClient.Event.Delete(eventId, _cancellationToken);
  }

  [Fact(DisplayName = "Get job status for completed job")]
  public async Task GetJobStatusForCompletedJob()
  {
    // Arrange
    var newEvent = new NewEvent
    {
      Title = "Completed job event",
      Enabled = true,
      Category = "general",
      Plugin = "testplug",
      Target = "allgrp",
      Timing = new Timing
      {
        Hours = [4],
        Minutes = [27],
        Days = [5],
        Months = [1],
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
    var jobData = await _cronicleClient.Job.GetJobStatus(ids.First(), _cancellationToken);

    // Assert
    jobData.Should().NotBeNull();
    jobData!.AbortReason.Should().BeNull();
    jobData!.Description.Should().NotContain("Job Aborted");

    // Cleanup
    await _cronicleClient.Event.Delete(eventId, _cancellationToken);
  }
}