using CronicleClient;
using CronicleClient.Models;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Tests;

public class GetJobStatus
{
    private readonly Client _cronicleClient;
    private readonly CancellationToken _cancellationToken;

    public GetJobStatus(ITestOutputHelper outputHelper)
    {
        var serverUrl = "http://localhost:3012";
        var apiKey = "240db114e152267349f17cf768808169";
        var logger = outputHelper.ToLogger<CreateEvent>();

        _cronicleClient = new Client(baseUrl: serverUrl, apiToken: apiKey, logger: logger);
        _cancellationToken = new CancellationTokenSource().Token;
    }

    [Fact(DisplayName = "Get job status")]
    public async Task GetJobsStatus()
    {
        // Arrange
        var newEvent = new NewEvent()
        {
            Title = "A job event",
            Enabled = true,
            Category = "general",
            Plugin = "testplug",
            Target = "allgrp",
            Timing = new Timing()
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

        var ids = await _cronicleClient.Event.RunEventById(eventId: eventId, cancellationToken: _cancellationToken);

        // Act
        var jobData = await _cronicleClient.Job.GetJobStatus(ids.First(), cancellationToken: _cancellationToken);
        jobData.Should().NotBeNull();

        // Cleanup
        await _cronicleClient.Job.AbortJob(ids.First(), cancellationToken: _cancellationToken);
        await Task.Delay(1500);
        await _cronicleClient.Event.Delete(eventId: eventId, cancellationToken: _cancellationToken);
    }

    [Fact(DisplayName = "Get job status for non-existent job")]
    public async Task GetJobStatusForNonExistentJob()
    {
        // Arrange
        string nonExistentJobId = "nonExistentJobId";

        // Act & Assert
        await FluentActions.Invoking(() => _cronicleClient.Job.GetJobStatus(nonExistentJobId, cancellationToken: _cancellationToken))
            .Should().ThrowAsync<Exception>();
    }

    [Fact(DisplayName = "Get job status for aborted job")]
    public async Task GetJobStatusForAbortedJob()
    {
        // Arrange
        var newEvent = new NewEvent()
        {
            Title = "Abort job event",
            Enabled = true,
            Category = "general",
            Plugin = "testplug",
            Target = "allgrp",
            Timing = new Timing()
            {
                Hours = [4],
                Minutes = [27],
                Days = [5],
                Months = [1],
                Years = [2024]
            }
        };

        var eventId = await _cronicleClient.Event.Create(newEvent, cancellationToken: _cancellationToken);
        eventId.Should().NotBeEmpty();

        var ids = await _cronicleClient.Event.RunEventById(eventId: eventId, cancellationToken: _cancellationToken);
        await _cronicleClient.Job.AbortJob(ids.First(), cancellationToken: _cancellationToken);
        await Task.Delay(1500);

        // Act
        var jobData = await _cronicleClient.Job.GetJobStatus(ids.First(), cancellationToken: _cancellationToken);

        // Assert
        jobData.Should().NotBeNull();
        jobData.AbortReason.Should().NotBeNull();

        // Cleanup
        await _cronicleClient.Event.Delete(eventId: eventId, cancellationToken: _cancellationToken);
    }

    [Fact(DisplayName = "Get job status for completed job")]
    public async Task GetJobStatusForCompletedJob()
    {
        // Arrange
        var newEvent = new NewEvent()
        {
            Title = "Completed job event",
            Enabled = true,
            Category = "general",
            Plugin = "plyyyhtht1w",
            Target = "allgrp",
            Timing = new Timing()
            {
                Hours = [4],
                Minutes = [27],
                Days = [5],
                Months = [1],
                Years = [2024]
            }
        };

        var eventId = await _cronicleClient.Event.Create(newEvent, cancellationToken: _cancellationToken);
        eventId.Should().NotBeEmpty();

        var ids = await _cronicleClient.Event.RunEventById(eventId: eventId, cancellationToken: _cancellationToken);
        await Task.Delay(1500); 

        // Act
        var jobData = await _cronicleClient.Job.GetJobStatus(ids.First(), cancellationToken: _cancellationToken);

        // Assert
        jobData.Should().NotBeNull();
        jobData.AbortReason.Should().BeNull();
        jobData.Description.Should().NotStartWith("Job Aborted");

        // Cleanup
        await _cronicleClient.Event.Delete(eventId: eventId, cancellationToken: _cancellationToken);
    }




}
