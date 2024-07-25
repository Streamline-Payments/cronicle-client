using CronicleClient;
using CronicleClient.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Tests;

public class AbortJob
{
    private readonly Client _cronicleClient;
    private readonly CancellationToken _cancellationToken;

    public AbortJob(ITestOutputHelper outputHelper)
    {
        var serverUrl = "http://localhost:3012";
        var apiKey = "240db114e152267349f17cf768808169";
        var logger = outputHelper.ToLogger<CreateEvent>();

        _cronicleClient = new Client(baseUrl: serverUrl, apiToken: apiKey, logger: logger);
        _cancellationToken = new CancellationTokenSource().Token;
    }

    [Fact(DisplayName = "Abort a Job")]
    public async Task AbortAJob()
    {
        // Arrange
        var newEvent = new NewEvent()
        {
            Title = "A title for abort",
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
        var newEventId = await _cronicleClient.Event.Create(eventData: newEvent, cancellationToken: _cancellationToken);
        newEventId.Should().NotBeEmpty();

        var jobId = await _cronicleClient.Event.RunEventById(newEventId);
        jobId.Should().NotBeEmpty();
        jobId.Length.Should().Be(1);

        // Act & Assert
        await FluentActions.Invoking(() => _cronicleClient.Job.AbortJob(jobId.First(), _cancellationToken))
          .Should().NotThrowAsync<Exception>();

        // Cleanup
        await Task.Delay(1000);
        await _cronicleClient.Event.Delete(eventId: newEventId, cancellationToken: _cancellationToken);
    }


    [Fact(DisplayName = "Abort a Job and validate")]
    public async Task AbortAJobValidate()
    {
        // Arrange
        var newEvent = new NewEvent()
        {
            Title = "A title for abort",
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
        var newEventId = await _cronicleClient.Event.Create(eventData: newEvent, cancellationToken: _cancellationToken);
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
        await _cronicleClient.Event.Delete(eventId: newEventId, cancellationToken: _cancellationToken);
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
        var newEvent = new NewEvent()
        {
            Title = "A title",
            Enabled = true,
            Category = "general",
            Plugin = "plyyyhtht1w",
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
        var jobIds = await _cronicleClient.Event.RunEventById(eventId: eventId, cancellationToken: _cancellationToken);
        jobIds.Should().NotBeEmpty();
        await Task.Delay(1000);

        var jobsCompleted = await _cronicleClient.Job.GetByEventId(eventId: eventId, 1, cancellationToken: _cancellationToken);
        jobsCompleted.FirstOrDefault(j => j.Id == jobIds.First()).Should().NotBeNull();

        // Act & Assert
        await FluentActions.Invoking(() => _cronicleClient.Job.AbortJob(eventId, _cancellationToken))
            .Should().ThrowAsync<KeyNotFoundException>();

        // Cleanup
        await _cronicleClient.Event.Delete(eventId: eventId, cancellationToken: _cancellationToken);
    }




}
