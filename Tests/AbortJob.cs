using CronicleClient;
using CronicleClient.Models;
using FluentAssertions;
using Xunit.Abstractions;

namespace Tests;

public class AbortJob(ITestOutputHelper outputHelper)
{
  private readonly CancellationToken _cancellationToken = new CancellationTokenSource().Token;
  private readonly Client _cronicleClient = Common.InitCronicleClient(outputHelper);
  
  [Fact(DisplayName = "Abort a Job")]
  public async Task AbortAJob()
  {
    // Arrange
    var newEvent = new NewEvent
    {
      Title = "A title for abort",
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
    var newEventId = await _cronicleClient.Event.Create(newEvent, _cancellationToken);
    newEventId.Should().NotBeEmpty();

    var jobId = await _cronicleClient.Event.RunEventById(newEventId);
    jobId.Should().NotBeEmpty();
    jobId.Length.Should().Be(1);

    // Act & Assert
    await FluentActions.Invoking(() => _cronicleClient.Job.AbortJob(jobId.First(), _cancellationToken))
      .Should().NotThrowAsync<Exception>();

    // Cleanup
    await Task.Delay(1000);
    await _cronicleClient.Event.Delete(newEventId, _cancellationToken);
  }


  [Fact(DisplayName = "Abort a Job and validate")]
  public async Task AbortAJobValidate()
  {
    // Arrange
    var newEvent = new NewEvent
    {
      Title = "A title for abort",
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
    var newEventId = await _cronicleClient.Event.Create(newEvent, _cancellationToken);
    newEventId.Should().NotBeEmpty();

    var jobId = await _cronicleClient.Event.RunEventById(newEventId);
    jobId.Should().NotBeEmpty();
    jobId.Length.Should().Be(1);

    // Act
    await FluentActions.Invoking(() => _cronicleClient.Job.AbortJob(jobId.First(), _cancellationToken))
      .Should().NotThrowAsync<Exception>();

    // Assert
    var abortedJob = await _cronicleClient.Job.GetJobStatus(jobId.First());
    abortedJob.Should().NotBeNull();
    abortedJob.AbortReason.Should().NotBeNull();

    // Cleanup
    await Task.Delay(1000);
    await _cronicleClient.Event.Delete(newEventId, _cancellationToken);
  }

  [Fact(DisplayName = "Fails to abort a non-existent job")]
  public async Task AbortNonExistentJob()
  {
    // Arrange
    var nonExistentJobId = "invalid_job_id";

    // Act & Assert
    await FluentActions.Invoking(() => _cronicleClient.Job.AbortJob(nonExistentJobId, _cancellationToken))
      .Should().ThrowAsync<KeyNotFoundException>();
  }

  [Fact(DisplayName = "Fails to abort job with invalid data")]
  public async Task AbortJobInvalidData()
  {
    // Arrange
    var invalidJobId = string.Empty;

    // Act & Assert
    await FluentActions.Invoking(() => _cronicleClient.Job.AbortJob(invalidJobId, _cancellationToken))
      .Should().ThrowAsync<Exception>();
  }

  [Fact(DisplayName = "Fails to abort a completed job")]
  public async Task AbortCompletedJob()
  {
    // Arrange
    var newEvent = new NewEvent
    {
      Title = "A title",
      Enabled = true,
      Category = "general",
      Plugin = "plyyyhtht1w",
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
    var jobIds = await _cronicleClient.Event.RunEventById(eventId, _cancellationToken);
    jobIds.Should().NotBeEmpty();
    await Task.Delay(1000);

    var jobsCompleted = await _cronicleClient.Job.GetByEventId(eventId, 1, cancellationToken: _cancellationToken);
    jobsCompleted.FirstOrDefault(j => j.Id == jobIds.First()).Should().NotBeNull();

    // Act & Assert
    await FluentActions.Invoking(() => _cronicleClient.Job.AbortJob(eventId, _cancellationToken))
      .Should().ThrowAsync<KeyNotFoundException>();

    // Cleanup
    await _cronicleClient.Event.Delete(eventId, _cancellationToken);
  }
}