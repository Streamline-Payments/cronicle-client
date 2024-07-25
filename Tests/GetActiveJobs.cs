using CronicleClient;
using CronicleClient.Models;
using FluentAssertions;
using Xunit.Abstractions;

namespace Tests;

public class GetActiveJobs
{
    private readonly Client _cronicleClient;
    private readonly CancellationToken _cancellationToken;

    public GetActiveJobs(ITestOutputHelper outputHelper)
    {
        var serverUrl = "http://localhost:3012";
        var apiKey = "240db114e152267349f17cf768808169";
        var logger = outputHelper.ToLogger<CreateEvent>();

        _cronicleClient = new Client(baseUrl: serverUrl, apiToken: apiKey, logger: logger);
        _cancellationToken = new CancellationTokenSource().Token;
    }

    [Fact(DisplayName = "Get active jobs that exists in Cronicle")]
    public async Task GetExistentActiveJobs()
    {
        // Arrange

        // Act
        var activeJobs = await _cronicleClient.Job.GetActiveJobs(cancellationToken: _cancellationToken);

        // Assert
        activeJobs.Should().NotBeNull();
    }

    [Fact(DisplayName = "Get active jobs in Cronicle")]
    public async Task GetActiveJobsForNewEvents()
    {
        // Arrange
        var newEvent = new NewEvent()
        {
            Title = "Active job",
            Enabled = true,
            Category = "general",
            Plugin = "testplug",
            Target = "allgrp",
            Timing = new Timing()
            {
                Hours = [4],
                Minutes = [27],
                Days = [25],
                Months = [8],
                Years = [2024]
            }
        };
        var eventId = await _cronicleClient.Event.Create(eventData: newEvent, cancellationToken: _cancellationToken);
        eventId.Should().NotBeEmpty();

        var jobIds = await _cronicleClient.Event.RunEventById(eventId);

        // Act
        var activeJobs = await _cronicleClient.Job.GetActiveJobs(cancellationToken: _cancellationToken);

        // Assert
        activeJobs.Should().NotBeNull();

        // Cleanup
        await _cronicleClient.Job.AbortJob(jobIds.First());
        await Task.Delay(1000);
        await _cronicleClient.Event.Delete(eventId: eventId, cancellationToken: _cancellationToken);
    }


    [Fact(DisplayName = "Get active jobs and verify")]
    public async Task GetActiveJobsAndVerifyEvents()
    {
        // Arrange
        var newEvent = new NewEvent()
        {
            Title = "Active job",
            Enabled = true,
            Category = "general",
            Plugin = "testplug",
            Target = "allgrp",
            Timing = new Timing()
            {
                Hours = [4],
                Minutes = [27],
                Days = [25],
                Months = [8],
                Years = [2024]
            }
        };
        var eventId = await _cronicleClient.Event.Create(eventData: newEvent, cancellationToken: _cancellationToken);
        eventId.Should().NotBeEmpty();

        var jobIds = await _cronicleClient.Event.RunEventById(eventId);

        // Act
        var activeJobs = await _cronicleClient.Job.GetActiveJobs(cancellationToken: _cancellationToken);
        activeJobs.Any(a => a.Value.AbortReason != null).Should().BeFalse();

        // Assert
        activeJobs.Should().NotBeNull();

        // Cleanup
        await _cronicleClient.Job.AbortJob(jobIds.First());
        await Task.Delay(1000);
        await _cronicleClient.Event.Delete(eventId: eventId, cancellationToken: _cancellationToken);
    }

}
