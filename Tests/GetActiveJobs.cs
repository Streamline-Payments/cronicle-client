using CronicleClient;
using CronicleClient.Models;
using FluentAssertions;
using Xunit.Abstractions;

namespace Tests;

[Collection("Active Jobs")]
public class GetActiveJobs(ITestOutputHelper outputHelper)
{
  private readonly CancellationToken _cancellationToken = new CancellationTokenSource().Token;
  private readonly Client _cronicleClient = Common.InitCronicleClient(outputHelper);

  [Fact(DisplayName = "Get active jobs that exists in Cronicle")]
  public async Task GetExistentActiveJobs()
  {
    // Arrange

    // Act
    var activeJobs = await _cronicleClient.Job.GetActiveJobs(_cancellationToken);

    // Assert
    activeJobs.Should().NotBeNull();
  }

  [Fact(DisplayName = "Get active jobs in Cronicle")]
  public async Task GetActiveJobsForNewEvents()
  {
    // Arrange
    var newEvent = new NewEvent
    {
      Title = "Active job",
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

    var jobIds = await _cronicleClient.Event.RunEventById(eventId, _cancellationToken);

    // Act
    var activeJobs = await _cronicleClient.Job.GetActiveJobs(_cancellationToken);

    // Assert
    activeJobs.Should().NotBeNull();

    // Cleanup
    await _cronicleClient.Job.AbortJob(jobIds.First(), _cancellationToken);
    await Task.Delay(2000, _cancellationToken);
    await _cronicleClient.Event.Delete(eventId, _cancellationToken);
  }
  
  [Fact(DisplayName = "Get active jobs and verify")]
  public async Task GetActiveJobsAndVerifyEvents()
  {
    // Arrange
    var newEvent = new NewEvent
    {
      Title = "Active job",
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

    var jobIds = await _cronicleClient.Event.RunEventById(eventId, _cancellationToken);

    // Act
    var activeJobs = await _cronicleClient.Job.GetActiveJobs(_cancellationToken);

    // Assert
    activeJobs!.First(q => q.Value.EventTitle == newEvent.Title).Should().NotBeNull();

    // Cleanup
    await _cronicleClient.Job.AbortJob(jobIds.First(), _cancellationToken);
    await Task.Delay(2000, _cancellationToken);
    await _cronicleClient.Event.Delete(eventId, _cancellationToken);
  }
}