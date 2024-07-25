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

public class GetHistoryTest
{
    private readonly Client _cronicleClient;
    private readonly CancellationToken _cancellationToken;

    public GetHistoryTest(ITestOutputHelper outputHelper)
    {
        var serverUrl = "http://localhost:3012";
        var apiKey = "240db114e152267349f17cf768808169";
        var logger = outputHelper.ToLogger<CreateEvent>();

        _cronicleClient = new Client(baseUrl: serverUrl, apiToken: apiKey, logger: logger);
        _cancellationToken = new CancellationTokenSource().Token;
    }

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
        var history = await _cronicleClient.Job.GetHistory(limit, offset, cancellationToken: _cancellationToken);

        // Assert
        history.Should().NotBeNull();
    }


    [Fact(DisplayName = "Get an history for all events with validation")]
    public async Task GetNewEventsInHistory()
    {
        // Arrange
        var newEvent = new NewEvent()
        {
            Title = "New title",
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

        var ids = await _cronicleClient.Event.RunEventById(eventId: eventId, cancellationToken: _cancellationToken);
        await Task.Delay(500);

        // Act
        var history = await _cronicleClient.Job.GetHistory(10, cancellationToken: _cancellationToken);

        // Assert
        history.Should().NotBeNull();   
        history.FirstOrDefault(j => j.Id == ids.First()).Should().NotBeNull();
        

        // Cleanup
        await _cronicleClient.Event.Delete(eventId: eventId, cancellationToken: _cancellationToken);
    }

    [Fact(DisplayName = "Get an history for all events with aborted events")]
    public async Task GetNewAbortEventsInHistory()
    {
        // Arrange
        var newEvent = new NewEvent()
        {
            Title = "New title",
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

        var ids = await _cronicleClient.Event.RunEventById(eventId: eventId, cancellationToken: _cancellationToken);
        await _cronicleClient.Job.AbortJob(ids.First(), cancellationToken: _cancellationToken);
        await Task.Delay(1500);

        // Act
        var history = await _cronicleClient.Job.GetHistory(10, cancellationToken: _cancellationToken);

        // Assert
        history.Should().NotBeNull();
        var abortedJob = history.FirstOrDefault(j => j.Id == ids.First());
        abortedJob.Should().NotBeNull();
        abortedJob.Description.Should().StartWith("Job Aborted");

        // Cleanup
        await _cronicleClient.Event.Delete(eventId: eventId, cancellationToken: _cancellationToken);
    }

}
