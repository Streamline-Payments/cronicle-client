using CronicleClient;
using CronicleClient.Models;
using FluentAssertions;
using Xunit.Abstractions;

namespace Tests;

public class UpdateEvent(ITestOutputHelper outputHelper)
{
  private readonly CancellationToken _cancellationToken = new CancellationTokenSource().Token;
  private readonly Client _cronicleClient = Common.InitCronicleClient(outputHelper);

  [Fact(DisplayName = "Update an Event")]
  public async Task UpdateAnEvent()
  {
    // Arrange
    var newEvent = new NewEvent
    {
      Title = "A title for update",
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
      }
    };
    var newEventId = await _cronicleClient.Event.Create(newEvent, _cancellationToken);
    newEventId.Should().NotBeEmpty();

    var eventDetails = await _cronicleClient.Event.GetById(newEventId, _cancellationToken);
    eventDetails.Should().NotBeNull();

    var eventData = eventDetails;

    eventData.Title = "UpdatedTitle 2";
    eventData.Category = "general";
    eventData.Plugin = "testplug";
    eventData.Target = "allgrp";
    eventData.Timing = new Timing
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
    await _cronicleClient.Event.Delete(newEventId, _cancellationToken);
  }

  [Fact(DisplayName = "Update more properties in Event")]
  public async Task UpdateEventAndAbortRunningJobs()
  {
    // Arrange
    var newEvent = new NewEvent
    {
      Title = "Event to update and abort jobs",
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
      }
    };
    var newEventId = await _cronicleClient.Event.Create(newEvent, _cancellationToken);
    newEventId.Should().NotBeEmpty();

    var eventDetails = await _cronicleClient.Event.GetById(newEventId, _cancellationToken);
    eventDetails.Should().NotBeNull();

    var updateData = eventDetails;

    updateData.Enabled = false;
    updateData.NotifySuccess = "Success@yopmail.com";
    updateData.Timeout = 600;

    // Act


    await FluentActions.Invoking(() => _cronicleClient.Event.Update(updateData))
      .Should().NotThrowAsync<Exception>();

    // Assert
    var updatedEvent = await _cronicleClient.Event.GetById(newEventId, _cancellationToken);
    updatedEvent.Enabled.Should().BeFalse();
    updatedEvent.NotifySuccess.Should().Be(updateData.NotifySuccess);
    updatedEvent.Timeout.Should().Be(updateData.Timeout);

    // Cleanup
    await _cronicleClient.Event.Delete(newEventId, _cancellationToken);
  }


  [Fact(DisplayName = "Update an Event with invalid timing")]
  public async Task UpdateAnEventBadTiming()
  {
    // Arrange
    var newEvent = new NewEvent
    {
      Title = "A title for update",
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
      }
    };
    var newEventId = await _cronicleClient.Event.Create(newEvent, _cancellationToken);
    newEventId.Should().NotBeEmpty();

    var eventDetails = await _cronicleClient.Event.GetById(newEventId, _cancellationToken);
    eventDetails.Should().NotBeNull();

    var eventData = eventDetails;

    eventData.Title = "UpdatedTitle 2";
    eventData.Category = "general";
    eventData.Plugin = "testplug";
    eventData.Target = "allgrp";
    eventData.Timing = new Timing
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
    await _cronicleClient.Event.Delete(newEventId, _cancellationToken);
  }

  [Fact(DisplayName = "Update Event with invalid data")]
  public async Task UpdateEventWithInvalidData()
  {
    // Arrange
    var newEvent = new NewEvent
    {
      Title = "Event to update with invalid data",
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
      }
    };
    var newEventId = await _cronicleClient.Event.Create(newEvent, _cancellationToken);
    newEventId.Should().NotBeEmpty();

    var eventDetails = await _cronicleClient.Event.GetById(newEventId, _cancellationToken);
    eventDetails.Should().NotBeNull();

    // Act
    var updateData = new EventData { Id = newEventId, Title = "" };
    await FluentActions.Invoking(() => _cronicleClient.Event.Update(updateData))
      .Should().ThrowAsync<Exception>();

    // Cleanup
    await _cronicleClient.Event.Delete(newEventId, _cancellationToken);
  }
}