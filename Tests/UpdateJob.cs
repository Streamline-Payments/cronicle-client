using CronicleClient;
using CronicleClient.Models;
using FluentAssertions;
using Xunit.Abstractions;

namespace Tests;

[Collection("Cronicle Client collection")]
public class UpdateJob(ITestOutputHelper outputHelper)
{
  private readonly CancellationToken _cancellationToken = new CancellationTokenSource().Token;
  private readonly Client _cronicleClient = Common.InitCronicleClient(outputHelper);

  [Fact(DisplayName = "Update an Job")]
  public async Task UpdateAnJob()
  {
    // Arrange
    var newEvent = new NewEvent
    {
      Title = "A title for job update",
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
    var newEventId = await _cronicleClient.Event.Create(newEvent, _cancellationToken);
    newEventId.Should().NotBeEmpty();

    var jobIds = await _cronicleClient.Event.RunEventById(newEventId, _cancellationToken);
;
    var updatedJob = new JobDataUpdateRequest
    {
      Id = jobIds.First(),
      NotifySuccess = "succes@yopmail.com",
      Timeout = 600
    };

    // Act & Assert
    await _cronicleClient.Job.Update(updatedJob, _cancellationToken);

    // Cleanup
    await _cronicleClient.Job.AbortJob(jobIds.First(), _cancellationToken);
    await Task.Delay(2000, _cancellationToken);
    await _cronicleClient.Event.Delete(newEventId, _cancellationToken);
  }

  [Fact(DisplayName = "Update the CPU limit of a Job")]
  public async Task UpdateJobCpuLimit()
  {
    // Arrange
    var newEvent = new NewEvent
    {
      Title = "Event to update job CPU limit",
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
    var newEventId = await _cronicleClient.Event.Create(newEvent, _cancellationToken);
    newEventId.Should().NotBeEmpty();

    var jobIds = await _cronicleClient.Event.RunEventById(newEventId, _cancellationToken);

    var updatedJob = new JobDataUpdateRequest
    {
      Id = jobIds.First(),
      CpuLimit = 200,
      CpuSustain = 120
    };

    // Act
    await _cronicleClient.Job.Update(updatedJob, _cancellationToken);

    // Cleanup
    await _cronicleClient.Job.AbortJob(jobIds.First(), _cancellationToken);
    await Task.Delay(2000, _cancellationToken);
    await _cronicleClient.Event.Delete(newEventId, _cancellationToken);
  }

  [Fact(DisplayName = "Update an not-existent Job")]
  public async Task UpdateNotExistentJob()
  {
    // Arrange
    var updatedJob = new JobDataUpdateRequest
    {
      Id = "not_existent_job",
      Timeout = 300
    };

    // Act & Assert
    await FluentActions.Invoking(() => _cronicleClient.Job.Update(updatedJob, _cancellationToken))
      .Should().ThrowAsync<KeyNotFoundException>();
  }

  [Fact(DisplayName = "Update an invalid Job")]
  public async Task UpdateInvalidJob()
  {
    // Arrange
    var updatedJob = new JobDataUpdateRequest
    {
      Id = string.Empty,
      Timeout = 200
    };

    // Act & Assert
    await FluentActions.Invoking(() => _cronicleClient.Job.Update(updatedJob, _cancellationToken))
      .Should().ThrowAsync<Exception>();
  }
  
  [Fact(DisplayName = "Update a completed Job")]
  public async Task UpdateJobCompleted()
  {
    // Arrange
    var newEvent = new NewEvent
    {
      Title = "Event to update job completed",
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
      Parameters = new Dictionary<string, object>()
      {
        { "duration", 1 } // 1 second
      }
    };
    var newEventId = await _cronicleClient.Event.Create(newEvent, _cancellationToken);
    newEventId.Should().NotBeEmpty();

    var jobIds = await _cronicleClient.Event.RunEventById(newEventId, _cancellationToken);
    await Task.Delay(2000, _cancellationToken);
    
    // Act & Assert
    var updatedJob = new JobDataUpdateRequest
    {
      Id = jobIds.First(),
      CpuLimit = 200,
      CpuSustain = 120
    };
    await FluentActions.Invoking(() => _cronicleClient.Job.Update(updatedJob, _cancellationToken))
      .Should().ThrowAsync<KeyNotFoundException>();

    // Cleanup
    await _cronicleClient.Event.Delete(newEventId, _cancellationToken);
  }

  [Fact(DisplayName = "Update an aborted Job")]
  public async Task UpdateJobAborted()
  {
    // Arrange
    var newEvent = new NewEvent
    {
      Title = "Event to update job CPU aborted",
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
    var newEventId = await _cronicleClient.Event.Create(newEvent, _cancellationToken);
    newEventId.Should().NotBeEmpty();

    var jobIds = await _cronicleClient.Event.RunEventById(newEventId, _cancellationToken);

    var updatedJob = new JobDataUpdateRequest
    {
      Id = jobIds.First(),
      CpuLimit = 200,
      CpuSustain = 120
    };

    await _cronicleClient.Job.AbortJob(jobIds.First(), _cancellationToken);
    await Task.Delay(2000, _cancellationToken);

    // Act & Assert
    await FluentActions.Invoking(() => _cronicleClient.Job.Update(updatedJob, _cancellationToken))
      .Should().ThrowAsync<KeyNotFoundException>();

    // Cleanup
    await _cronicleClient.Event.Delete(newEventId, _cancellationToken);
  }
}