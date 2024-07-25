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
public class GetEventByTitle
{
    private readonly Client _cronicleClient;
    private readonly CancellationToken _cancellationToken;

    public GetEventByTitle(ITestOutputHelper outputHelper)
    {
        var serverUrl = "http://localhost:3012";
        var apiKey = "240db114e152267349f17cf768808169";
        var logger = outputHelper.ToLogger<CreateEvent>();

        _cronicleClient = new Client(baseUrl: serverUrl, apiToken: apiKey, logger: logger);
        _cancellationToken = new CancellationTokenSource().Token;
    }

    [Fact(DisplayName = "Get an event that exists in Cronicle by its title")]
    public async Task GettEventByTitle()
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

        // Act
        await FluentActions.Invoking(() => _cronicleClient.Event.GetByTitle(eventTitle: newEvent.Title, cancellationToken: _cancellationToken))
            .Should().NotThrowAsync<KeyNotFoundException>();

        // Cleanup
        await _cronicleClient.Event.Delete(eventId: eventId, cancellationToken: _cancellationToken);
    }

    [Fact(DisplayName = "Get an event that exists does not exist in Cronicle by its title")]
    public async Task GetNotExistentEventByTitle()
    {
        // Arrange
        var eventTitle = "NoTitle";

        // Act
        await FluentActions.Invoking(() => _cronicleClient.Event.GetByTitle(eventTitle: eventTitle, cancellationToken: _cancellationToken))
            .Should().ThrowAsync<KeyNotFoundException>();

    }

    [Fact(DisplayName = "Get an event with invalid title")]
    public async Task GetEventWithInvalidId()
    {
        // Arrange
        var invalidTitle = string.Empty;

        // Act & Assert
        await FluentActions.Invoking(() => _cronicleClient.Event.GetByTitle(eventTitle: invalidTitle, cancellationToken: _cancellationToken))
            .Should().ThrowAsync<Exception>();
    }

}

