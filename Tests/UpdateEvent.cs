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
public class UpdateEvent
{
    private readonly Client _cronicleClient;
    private readonly CancellationToken _cancellationToken;

    public UpdateEvent(ITestOutputHelper outputHelper)
    {
        var serverUrl = "http://localhost:3012";
        var apiKey = "240db114e152267349f17cf768808169";
        var logger = outputHelper.ToLogger<CreateEvent>();

        _cronicleClient = new Client(baseUrl: serverUrl, apiToken: apiKey, logger: logger);
        _cancellationToken = new CancellationTokenSource().Token;
    }

    [Fact(DisplayName = "Update an Event")]
    public async Task UpdateAnEvent()
    {
        // Arrange
        var newEvent = new NewEvent()
        {
            Title = "A title for update",
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

        var eventDetails = await _cronicleClient.Event.GetById(eventId: newEventId, cancellationToken: _cancellationToken);
        eventDetails.Should().NotBeNull();

        EventData eventData = eventDetails;

        eventData.Title = "UpdatedTitle 2";
        eventData.Category = "general";
        eventData.Plugin = "testplug";
        eventData.Target = "allgrp";
        eventData.Timing = new Timing()
        {
            Hours = [8],
            Minutes = [32],
            Days = [12],
            Months = [2],
            Years = [2024]
        };

        // Act & Assert
        await FluentActions.Invoking(() => _cronicleClient.Event.Update(eventData))
            .Should().NotThrowAsync<Exception>();

        // Cleanup
        await _cronicleClient.Event.Delete(eventId: newEventId, cancellationToken: _cancellationToken);
    }

    [Fact(DisplayName = "Update more properties in Event")]
    public async Task UpdateEventAndAbortRunningJobs()
    {
        // Arrange
        var newEvent = new NewEvent()
        {
            Title = "Event to update and abort jobs",
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

        var eventDetails = await _cronicleClient.Event.GetById(eventId: newEventId, cancellationToken: _cancellationToken);
        eventDetails.Should().NotBeNull();

        EventData updateData = eventDetails;

        updateData.Enabled = false;
        updateData.NotifySuccess= "Success@yopmail.com";
        updateData.Timeout = 600;

        // Act


        await FluentActions.Invoking(() => _cronicleClient.Event.Update(updateData))
            .Should().NotThrowAsync<Exception>();

        // Assert
        var updatedEvent = await _cronicleClient.Event.GetById(eventId: newEventId, cancellationToken: _cancellationToken);
        updatedEvent.Enabled.Should().BeFalse();
        updatedEvent.NotifySuccess.Should().Be(updateData.NotifySuccess);
        updatedEvent.Timeout.Should().Be(updateData.Timeout);

        // Cleanup
        await _cronicleClient.Event.Delete(eventId: newEventId, cancellationToken: _cancellationToken);
    }


    [Fact(DisplayName = "Update an Event with invalid timing")]
    public async Task UpdateAnEventBadTiming()
    {
        // Arrange
        var newEvent = new NewEvent()
        {
            Title = "A title for update",
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

        var eventDetails = await _cronicleClient.Event.GetById(eventId: newEventId, cancellationToken: _cancellationToken);
        eventDetails.Should().NotBeNull();

        EventData eventData = eventDetails;

        eventData.Title = "UpdatedTitle 2";
        eventData.Category = "general";
        eventData.Plugin = "testplug";
        eventData.Target = "allgrp";
        eventData.Timing = new Timing()
        {
            Hours = [8],
            Minutes = [32],
            Days = [12],
            Months = [2],
            Years = [0]
        };

        // Act & Assert
        await FluentActions.Invoking(() => _cronicleClient.Event.Update(eventData))
            .Should().ThrowAsync<Exception>();


        // Cleanup
        await _cronicleClient.Event.Delete(eventId: newEventId, cancellationToken: _cancellationToken);
    }

    [Fact(DisplayName = "Update Event with invalid data")]
    public async Task UpdateEventWithInvalidData()
    {
        // Arrange
        var newEvent = new NewEvent()
        {
            Title = "Event to update with invalid data",
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

        var eventDetails = await _cronicleClient.Event.GetById(eventId: newEventId, cancellationToken: _cancellationToken);
        eventDetails.Should().NotBeNull();

        // Act
        var updateData = new EventData() { Id = newEventId, Title = "" };
        await FluentActions.Invoking(() => _cronicleClient.Event.Update(updateData))
            .Should().ThrowAsync<Exception>();

        // Cleanup
        await _cronicleClient.Event.Delete(eventId: newEventId, cancellationToken: _cancellationToken);
    }



}

