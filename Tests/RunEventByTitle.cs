using CronicleClient;
using CronicleClient.Models;
using FluentAssertions;
using Xunit.Abstractions;

namespace Tests;

public class RunEventByTitle
{
    private readonly Client _cronicleClient;
    private readonly CancellationToken _cancellationToken;

    public RunEventByTitle(ITestOutputHelper outputHelper)
    {
        var serverUrl = "http://localhost:3012";
        var apiKey = "240db114e152267349f17cf768808169";
        var logger = outputHelper.ToLogger<CreateEvent>();

        _cronicleClient = new Client(baseUrl: serverUrl, apiToken: apiKey, logger: logger);
        _cancellationToken = new CancellationTokenSource().Token;
    }


    [Fact(DisplayName = "Run an event by its title")]
    public async Task RunEvent()
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


        // Act
        var jobIds = await _cronicleClient.Event.RunEventByTitle(eventTitle: newEvent.Title, cancellationToken: _cancellationToken);

        // Assert
        jobIds.Should().NotBeEmpty();

        // Cleanup
        await Task.Delay(500);
        await _cronicleClient.Event.Delete(eventId: eventId, cancellationToken: _cancellationToken);
    }

    [Fact(DisplayName = "Run an event by its title and validate")]
    public async Task RunEventValidate()
    {
        // Arrange
        var newEvent = new NewEvent()
        {
            Title = "Run event validate",
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

        // Act
        var jobIds = await _cronicleClient.Event.RunEventByTitle(eventTitle: newEvent.Title, cancellationToken: _cancellationToken);

        // Assert
        jobIds.Should().NotBeEmpty();
        await Task.Delay(500);
        var jobs = await _cronicleClient.Job.GetByEventId(eventId, 10, cancellationToken: _cancellationToken);

        jobs.FirstOrDefault(x => x.Id == jobIds.First()).Should().NotBeNull();

        // Cleanup
        await Task.Delay(500);
        await _cronicleClient.Event.Delete(eventId: eventId, cancellationToken: _cancellationToken);

    }

    [Fact(DisplayName = "Run a non-existent event by its title")]
    public async Task RunNonExistentEventByTitle()
    {
        // Arrange
        var nonExistentEventId = "non-existent-title";

        // Act & Assert
        await FluentActions.Invoking(() => _cronicleClient.Event.RunEventByTitle(nonExistentEventId, cancellationToken: _cancellationToken))
            .Should().ThrowAsync<Exception>();
    }

    [Fact(DisplayName = "Run the same event multiple times")]
    public async Task RunSameEventMultipleTimes()
    {
        // Arrange
        var newEvent = new NewEvent()
        {
            Title = "Repeatable Event",
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

        // Act
        var jobIds1 = await _cronicleClient.Event.RunEventByTitle(eventTitle: newEvent.Title, cancellationToken: _cancellationToken);
        var jobIds2 = await _cronicleClient.Event.RunEventByTitle(eventTitle: newEvent.Title, cancellationToken: _cancellationToken);

        // Assert
        jobIds1.Should().NotBeEmpty();
        jobIds2.Should().NotBeEmpty();

        // Cleanup
        await Task.Delay(500);
        await _cronicleClient.Event.Delete(eventId: eventId, cancellationToken: _cancellationToken);
    }




}
