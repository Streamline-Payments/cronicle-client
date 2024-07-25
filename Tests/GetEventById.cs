using CronicleClient;
using CronicleClient.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Tests;

public class GetEventById
{
    private readonly Client _cronicleClient;
    private readonly CancellationToken _cancellationToken;

    public GetEventById(ITestOutputHelper outputHelper)
    {
        var serverUrl = "http://localhost:3012";
        var apiKey = "240db114e152267349f17cf768808169";
        var logger = outputHelper.ToLogger<CreateEvent>();

        _cronicleClient = new Client(baseUrl: serverUrl, apiToken: apiKey, logger: logger);
        _cancellationToken = new CancellationTokenSource().Token;
    }

    [Fact(DisplayName = "Get an event that exists in Cronicle")]
    public async Task GetExistentEvent()
    {
        // Arrange
        var newEvent = new NewEvent()
        {
            Title = "A title",
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
        var eventDetails = await _cronicleClient.Event.GetById(eventId: eventId, cancellationToken: _cancellationToken);

        // Assert
        eventDetails.Should().NotBeNull();

        // Cleanup
        await _cronicleClient.Event.Delete(eventId: eventId, cancellationToken: _cancellationToken);

    }


    [Fact(DisplayName = "Get an event that does not exists in Cronicle")]
    public async Task GetNotExistentEvent()
    {
        // Arrange
        var eventId = "00000000000";

        // Act & Assert
        await FluentActions.Invoking(() =>  _cronicleClient.Event.GetById(eventId: eventId, cancellationToken: _cancellationToken))
            .Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "Get an event with invalid id")]
    public async Task GetEventWithInvalidId()
    {
        // Arrange
        var invalidId = string.Empty;

        // Act & Assert
        await FluentActions.Invoking(() => _cronicleClient.Event.GetById(eventId: invalidId, cancellationToken: _cancellationToken))
            .Should().ThrowAsync<Exception>();
    }
}

