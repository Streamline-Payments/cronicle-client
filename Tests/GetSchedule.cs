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
public class GetSchedule
{
    private readonly Client _cronicleClient;
    private readonly CancellationToken _cancellationToken;

    public GetSchedule(ITestOutputHelper outputHelper)
    {
        var serverUrl = "http://localhost:3012";
        var apiKey = "240db114e152267349f17cf768808169";
        var logger = outputHelper.ToLogger<CreateEvent>();

        _cronicleClient = new Client(baseUrl: serverUrl, apiToken: apiKey, logger: logger);
        _cancellationToken = new CancellationTokenSource().Token;
    }

    [Fact(DisplayName = "Get an schedule that exists in Cronicle")]
    public async Task GetExistentSchedule()
    {
        // Arrange

        // Act
        var (events, _) = await _cronicleClient.Event.GetSchedule(cancellationToken: _cancellationToken);

        // Assert
        events.Should().NotBeNull();
    }

    [Fact(DisplayName = "Get an schedule with limit in Cronicle")]
    public async Task GetExistentScheduleWithLimit()
    {
        // Arrange
        int limit = 10;

        // Act
        var (events, _) = await _cronicleClient.Event.GetSchedule(limit, 0, cancellationToken: _cancellationToken);

        // Assert
        events.Should().NotBeNull();
        events.Count().Should().BeLessThanOrEqualTo(limit);
    }

    [Fact(DisplayName = "Get an schedule with offset in Cronicle")]
    public async Task GetExistentScheduleWithOffset()
    {
        // Arrange
        int offset = 3;

        // Act
        var (events, _) = await _cronicleClient.Event.GetSchedule(50, offset, cancellationToken: _cancellationToken);

        // Assert
        events.Should().NotBeNull();
    }

    [Fact(DisplayName = "Get an schedule with new event in Cronicle")]
    public async Task GetScheduleWithEvents()
    {
        // Arrange
        var newEvent = new NewEvent()
        {
            Title = "New event",
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

        // Act
        var (events, _) = await _cronicleClient.Event.GetSchedule(cancellationToken: _cancellationToken);

        // Assert
        events.Should().NotBeNull();
        events.FirstOrDefault(e => e.Id == eventId).Should().NotBeNull();

        // Cleanup
        await _cronicleClient.Event.Delete(eventId: eventId, cancellationToken: _cancellationToken);
    }

    [Theory]
    [InlineData(4)]
    [InlineData(2)]
    [InlineData(5)]
    public async Task GetScheduleWithMultipleNewEvents(int eventsNumber)
    {
        // Arrange
        List<string> newIds = new List<string>();
        for (int i = 0; i < eventsNumber; i++) {
            var newEvent = new NewEvent()
            {
                Title = $"New event {i+1}",
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
            newIds.Add(eventId);
        }

        // Act
        var (events, _) = await _cronicleClient.Event.GetSchedule(eventsNumber, 0, cancellationToken: _cancellationToken);

        // Assert
        events.Should().NotBeNull();
        foreach (var id in newIds) {
            events.FirstOrDefault(e => e.Id == id).Should().NotBeNull();
            // Cleanup
            await _cronicleClient.Event.Delete(eventId: id, cancellationToken: _cancellationToken);
        }

    }
}

