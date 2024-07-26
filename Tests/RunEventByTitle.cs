﻿using CronicleClient;
using CronicleClient.Models;
using FluentAssertions;
using Xunit.Abstractions;

namespace Tests;

public class RunEventByTitle(ITestOutputHelper outputHelper)
{
  private readonly CancellationToken _cancellationToken = new CancellationTokenSource().Token;
  private readonly Client _cronicleClient = Common.InitCronicleClient(outputHelper);


  [Fact(DisplayName = "Run an event by its title")]
  public async Task RunEvent()
  {
    // Arrange
    var newEvent = new NewEvent
    {
      Title = "A title",
      Enabled = true,
      Category = "general",
      Plugin = "plyyyhtht1w",
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
    var eventId = await _cronicleClient.Event.Create(newEvent, _cancellationToken);
    eventId.Should().NotBeEmpty();


    // Act
    var jobIds = await _cronicleClient.Event.RunEventByTitle(newEvent.Title, _cancellationToken);

    // Assert
    jobIds.Should().NotBeEmpty();

    // Cleanup
    await Task.Delay(500);
    await _cronicleClient.Event.Delete(eventId, _cancellationToken);
  }

  [Fact(DisplayName = "Run an event by its title and validate")]
  public async Task RunEventValidate()
  {
    // Arrange
    var newEvent = new NewEvent
    {
      Title = "Run event validate",
      Enabled = true,
      Category = "general",
      Plugin = "plyyyhtht1w",
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
    var eventId = await _cronicleClient.Event.Create(newEvent, _cancellationToken);
    eventId.Should().NotBeEmpty();

    // Act
    var jobIds = await _cronicleClient.Event.RunEventByTitle(newEvent.Title, _cancellationToken);

    // Assert
    jobIds.Should().NotBeEmpty();
    await Task.Delay(500);
    var jobs = await _cronicleClient.Job.GetByEventId(eventId, 10, cancellationToken: _cancellationToken);

    jobs.FirstOrDefault(x => x.Id == jobIds.First()).Should().NotBeNull();

    // Cleanup
    await Task.Delay(500);
    await _cronicleClient.Event.Delete(eventId, _cancellationToken);
  }

  [Fact(DisplayName = "Run a non-existent event by its title")]
  public async Task RunNonExistentEventByTitle()
  {
    // Arrange
    var nonExistentEventId = "non-existent-title";

    // Act & Assert
    await FluentActions.Invoking(() => _cronicleClient.Event.RunEventByTitle(nonExistentEventId, _cancellationToken))
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
      Plugin = "plyyyhtht1w",
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
    var eventId = await _cronicleClient.Event.Create(newEvent, _cancellationToken);
    eventId.Should().NotBeEmpty();

    // Act
    var jobIds1 = await _cronicleClient.Event.RunEventByTitle(newEvent.Title, _cancellationToken);
    var jobIds2 = await _cronicleClient.Event.RunEventByTitle(newEvent.Title, _cancellationToken);

    // Assert
    jobIds1.Should().NotBeEmpty();
    jobIds2.Should().NotBeEmpty();

    // Cleanup
    await Task.Delay(500);
    await _cronicleClient.Event.Delete(eventId, _cancellationToken);
  }
}