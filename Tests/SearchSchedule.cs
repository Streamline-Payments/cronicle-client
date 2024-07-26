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

public class SearchSchedule
{
    private readonly Client _cronicleClient;
    private readonly CancellationToken _cancellationToken;

    public SearchSchedule(ITestOutputHelper outputHelper)
    {
        var serverUrl = "http://localhost:3012";
        var apiKey = "240db114e152267349f17cf768808169";
        var logger = outputHelper.ToLogger<CreateEvent>();

        _cronicleClient = new Client(baseUrl: serverUrl, apiToken: apiKey, logger: logger);
        _cancellationToken = new CancellationTokenSource().Token;
    }

    [Fact(DisplayName = "Search on schedule")]
    public async Task GetSearchSchedule()
    {
        // Arrange
        SearchScheduleRequest request = new(null, "duration");

        // Act
        var eventSearched = await _cronicleClient.Event.SearchSchedule(request, cancellationToken: _cancellationToken);
        eventSearched.Should().NotBeNull();
        // Assert
    }


    [Fact(DisplayName = "Search on schedule asynchronously")]
    public async Task GetSearchScheduleAsync()
    {
        // Arrange
        string searchedTitle = "SearchedWord";

        // Act
        var eventSearched = await _cronicleClient.Event.SearchScheduleAsync(searchedTitle, null, cancellationToken: _cancellationToken);
        eventSearched.Should().NotBeNull();
        // Assert
    }

    [Fact(DisplayName = "Search on schedule asynchronously and validate")]
    public async Task GetSearchScheduleValidate()
    {
        // Arrange
        var newEvent = new NewEvent()
        {
            Title = $"Searched new title {Guid.NewGuid()}",
            Enabled = true,
            Category = "general",
            Plugin = "testplug",
            Target = "allgrp",
            Timing = new Timing()
            {
                Hours = [10],
                Minutes = [10],
                Days = [10],
                Months = [10],
                Years = [2028]
            }
        };
        var eventId = await _cronicleClient.Event.Create(eventData: newEvent, cancellationToken: _cancellationToken);
        eventId.Should().NotBeEmpty();

        // Act
        var eventSearched = await _cronicleClient.Event.SearchScheduleAsync(newEvent.Title, null, cancellationToken: _cancellationToken);

        // Assert
        eventSearched.Should().NotBeNull();
        eventSearched.Should().NotBeEmpty();
        eventSearched.FirstOrDefault(e => e.Id == eventId).Should().NotBeNull();

        // Cleanup
        await _cronicleClient.Event.Delete(eventId: eventId, cancellationToken: _cancellationToken);
    }

    [Fact(DisplayName = "Search on schedule non existent event")]
    public async Task GetSearchScheduleNonExistentEvent()
    {
        // Arrange
        string searchedTitle = $"{Guid.NewGuid()}";
  
        // Act
        var eventSearched = await _cronicleClient.Event.SearchScheduleAsync(searchedTitle, null, cancellationToken: _cancellationToken);

        // Assert
        eventSearched.Should().NotBeNull();
        eventSearched.Should().BeEmpty();
    }

}
