using CronicleClient;
using CronicleClient.Models;
using FluentAssertions;
using Xunit.Abstractions;

namespace Tests;

[Collection("Cronicle Client collection")]
public class GetEventById(ITestOutputHelper outputHelper)
{
  private readonly CancellationToken _cancellationToken = new CancellationTokenSource().Token;
  private readonly Client _cronicleClient = Common.InitCronicleClient(outputHelper);

  [Fact(DisplayName = "Get an event that exists in Cronicle")]
  public async Task GetExistingEvent()
  {
    // Arrange
    var newEvent = new NewEvent
    {
      Title = "A title",
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
    var eventId = await _cronicleClient.Event.Create(newEvent, _cancellationToken);
    eventId.Should().NotBeEmpty();

    // Act
    var eventDetails = await _cronicleClient.Event.GetById(eventId, _cancellationToken);

    // Assert
    eventDetails.Should().NotBeNull();

    // Cleanup
    await _cronicleClient.Event.Delete(eventId, _cancellationToken);
  }


  [Fact(DisplayName = "Get an event that does not exists in Cronicle")]
  public async Task GetNotExistentEvent()
  {
    // Arrange
    var eventId = "00000000000";

    // Act & Assert
    await FluentActions.Invoking(() => _cronicleClient.Event.GetById(eventId, _cancellationToken))
      .Should().ThrowAsync<KeyNotFoundException>();
  }

  [Fact(DisplayName = "Get an event with invalid id")]
  public async Task GetEventWithInvalidId()
  {
    // Arrange
    var invalidId = string.Empty;

    // Act & Assert
    await FluentActions.Invoking(() => _cronicleClient.Event.GetById(invalidId, _cancellationToken))
      .Should().ThrowAsync<Exception>();
  }
}