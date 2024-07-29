using CronicleClient;
using CronicleClient.Models;
using FluentAssertions;
using Xunit.Abstractions;

namespace Tests;

[Collection("Cronicle Client collection")]
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

    // Act
    var eventDetails = await _cronicleClient.Event.GetById(newEventId, _cancellationToken);
    eventDetails.Should().NotBeNull();

    eventDetails!.Title = "UpdatedTitle 2";
    eventDetails.Category = "general";
    eventDetails.Plugin = "testplug";
    eventDetails.Target = "allgrp";
    eventDetails.Timing = new Timing
    {
      Hours = [8],
      Minutes = [32],
      Days = [12],
      Months = [2],
      Years = [2024]
    };
    
    await _cronicleClient.Event.Update(eventDetails, cancellationToken: _cancellationToken);

    // Assert
    var updatedEventDetails = await _cronicleClient.Event.GetById(newEventId, _cancellationToken);
    updatedEventDetails.Should().NotBeNull();
    updatedEventDetails!.Title.Should().Be(eventDetails.Title);
    updatedEventDetails.Category.Should().Be(eventDetails.Category);
    updatedEventDetails.Plugin.Should().Be(eventDetails.Plugin);
    updatedEventDetails.Timing.Should().BeEquivalentTo(eventDetails.Timing);
    
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

    eventDetails!.Timing = new Timing
    {
      Hours = [8],
      Minutes = [32],
      Days = [12],
      Months = [2],
      Years = [0]
    };

    // Act & Assert
    await FluentActions.Invoking(() => _cronicleClient.Event.Update(eventDetails, cancellationToken: _cancellationToken))
      .Should().ThrowAsync<Exception>();
    
    // Cleanup
    await _cronicleClient.Event.Delete(newEventId, _cancellationToken);
  }
}