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
public class DeleteEvent
{
    private readonly Client _cronicleClient;
    private readonly CancellationToken _cancellationToken;

    public DeleteEvent(ITestOutputHelper outputHelper)
    {
        var serverUrl = "http://localhost:3012";
        var apiKey = "240db114e152267349f17cf768808169";
        var logger = outputHelper.ToLogger<CreateEvent>();

        _cronicleClient = new Client(baseUrl: serverUrl, apiToken: apiKey, logger: logger);
        _cancellationToken = new CancellationTokenSource().Token;
    }

    [Fact(DisplayName = "Delete an event")]
    public async Task DeleteEventById()
    {
        // Arrange
        var newEvent = new NewEvent()
        {
            Title = "A title for delete",
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

        // Act & Assert
        await FluentActions.Invoking(() => _cronicleClient.Event.Delete(eventId: eventId, cancellationToken: _cancellationToken))
            .Should().NotThrowAsync<Exception>();

    }

    [Fact(DisplayName = "Delete an event that does not exist")]
    public async Task DeleteNotExistentEvent()
    {
        // Arrange
        var eventId = "000000";

        // Act & Assert
        await FluentActions.Invoking(() => _cronicleClient.Event.Delete(eventId: eventId, cancellationToken: _cancellationToken))
        .Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "Delete an event and Validate")]
    public async Task DeleteEventByIdValidate()
    {
        // Arrange
        var newEvent = new NewEvent()
        {
            Title = "A title for delete",
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
        await FluentActions.Invoking(() => _cronicleClient.Event.Delete(eventId: eventId, cancellationToken: _cancellationToken))
            .Should().NotThrowAsync<Exception>();

        // Assert
        await FluentActions.Invoking(() => _cronicleClient.Event.GetById(eventId, cancellationToken: _cancellationToken))
            .Should().ThrowAsync<KeyNotFoundException>();
    }


    [Fact(DisplayName = "Delete an event with an active job")]
    public async Task DeleteWithActiveJob()
    {
        var newEvent = new NewEvent()
        {
            Title = "A title for delete",
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

        var jobId = await _cronicleClient.Event.RunEventById(eventId);

        // Act & Assert
        await FluentActions.Invoking(() => _cronicleClient.Event.Delete(eventId: eventId, cancellationToken: _cancellationToken))
            .Should().ThrowAsync<Exception>();

        // Cleanup
        await _cronicleClient.Job.AbortJob(jobId.First());

        await Task.Delay(1500);
        await _cronicleClient.Event.Delete(eventId: eventId, cancellationToken: _cancellationToken);
    }

}

