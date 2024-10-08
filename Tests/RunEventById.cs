﻿using CronicleClient;
using CronicleClient.Models;
using FluentAssertions;
using Xunit.Abstractions;

namespace Tests;

[Collection("Cronicle Client collection")]
public class RunEventById(ITestOutputHelper outputHelper)
{
    private readonly CancellationToken _cancellationToken = new CancellationTokenSource().Token;
    private readonly Client _cronicleClient = Common.InitCronicleClient(outputHelper);

    [Fact(DisplayName = "Run an event by its id")]
    public async Task RunEvent()
    {
        // Arrange
        var newEvent = new NewEvent
        {
            Title = "A title",
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
            Parameters = new Dictionary<string, object>()
      {
        { "duration", 1 } // 1 second
      }
        };
        var eventId = await _cronicleClient.Event.Create(newEvent, _cancellationToken);
        eventId.Should().NotBeEmpty();

        // Act
        var jobIds = await _cronicleClient.Event.RunEventById(eventId, _cancellationToken);

        // Assert
        jobIds.Should().NotBeEmpty();

        // Cleanup
        await Task.Delay(2000, _cancellationToken);
        await _cronicleClient.Event.Delete(eventId, _cancellationToken);
    }

    [Fact(DisplayName = "Run an event by its id and validate")]
    public async Task RunEventValidate()
    {
        // Arrange
        var newEvent = new NewEvent
        {
            Title = "Run event validate",
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

        // Act
        var jobIds = await _cronicleClient.Event.RunEventById(eventId, _cancellationToken);

        // Assert
        jobIds.Should().NotBeEmpty();
        await Task.Delay(2000, _cancellationToken);

        var jobs = await _cronicleClient.Job.GetByEventId(eventId, 10, cancellationToken: _cancellationToken);
        jobs.Should().HaveCount(1);
        jobs!.First().Code.Should().Be(0);

        // Cleanup
        await _cronicleClient.Event.Delete(eventId, _cancellationToken);
    }

    [Fact(DisplayName = "Run a non-existent event by its id")]
    public async Task RunNonExistentEventById()
    {
        // Arrange
        var nonExistentEventId = "non-existent-id";

        // Act
        await FluentActions.Invoking(() => _cronicleClient.Event.RunEventById(nonExistentEventId, _cancellationToken))
          .Should().ThrowAsync<Exception>();
    }

    [Fact(DisplayName = "Run the same event multiple times")]
    public async Task RunSameEventMultipleTimes()
    {
        // Arrange
        var newEvent = new NewEvent
        {
            Title = "Repeatable Event",
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

        // Act
        var jobIds1 = await _cronicleClient.Event.RunEventById(eventId, _cancellationToken);
        var jobIds2 = await _cronicleClient.Event.RunEventById(eventId, _cancellationToken);

        // Assert
        await Task.Delay(2000, _cancellationToken);
        var jobs = await _cronicleClient.Job.GetByEventId(eventId, 10, cancellationToken: _cancellationToken);
        jobs.Should().HaveCount(2);
        jobs!.Select(q => q.Code == 0).Should().HaveCount(2);

        jobIds2.Should().NotBeEmpty();

        // Cleanup
        await Task.Delay(500);
        await _cronicleClient.Event.Delete(eventId, _cancellationToken);
    }

    [Fact(DisplayName = "Run event with new event data")]
    public async Task RunEventWithEventData()
    {
        // Arrange
        var newEvent = new NewEvent
        {
            Title = "Repeatable Event",
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

        var extraEventData = new EventData
        {
            Id = eventId,
            Title = newEvent.Title,
            Category = newEvent.Category,
            Plugin = newEvent.Plugin,
            Target = newEvent.Target,
            Timing = newEvent.Timing,
            Parameters = new Dictionary<string, object>
            {
                {"ParamAdded","Extra" },
                {"Param2","Extra2" },
            }
        };

        // Act
        var jobIds1 = await _cronicleClient.Event.RunEventById(eventId, extraEventData, _cancellationToken);


        // Assert
        jobIds1.Should().NotBeNull();
        var js = await _cronicleClient.Job.GetJobStatus(jobIds1.FirstOrDefault());
        js.Parameters.Keys.Should().Contain("ParamAdded");
        await Task.Delay(2000, _cancellationToken);

        var jobs = await _cronicleClient.Job.GetByEventId(eventId, 10, cancellationToken: _cancellationToken);
        jobs.Should().HaveCount(1);
        jobs!.Select(q => q.Code == 0).Should().HaveCount(1);

        // Cleanup
        await Task.Delay(500);
        await _cronicleClient.Event.Delete(eventId, _cancellationToken);

    }
}