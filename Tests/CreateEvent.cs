using CronicleClient;
using CronicleClient.Models;
using FluentAssertions;
using Xunit.Abstractions;

namespace Tests;

public class CreateEvent
{
  private readonly Client _cronicleClient;
  private readonly CancellationToken _cancellationToken;

  public CreateEvent(ITestOutputHelper outputHelper)
  {
    var serverUrl = "http://localhost:3012";
    var apiKey = "e15e9de13124327913bbe0c84d80a867";
    var logger = outputHelper.ToLogger<CreateEvent>();

    _cronicleClient = new Client(baseUrl: serverUrl, apiToken: apiKey, logger: logger);
    _cancellationToken = new CancellationTokenSource().Token;
  }
  
  [Fact(DisplayName = "Create event with invalid timing")]
  public async Task BadTiming()
  {
    // Arrange
    var newEvent = new NewEvent()
    {
      Title = "A title",
      Enabled = true,
      Category = "createACHReturn",
      Plugin = "testplug",
      Target = "allgrp",
      Timing = new Timing()
      {
        Hours = [4],
        Minutes = [27],
        Days = [5],
        Months = [1],
        Years = [9000]
      }
    };

    // Act
    await FluentActions.Invoking(() => _cronicleClient.Event.Create(eventData: newEvent, cancellationToken: _cancellationToken))
      .Should().ThrowAsync<Exception>();
  }
  
  [Fact(DisplayName = "Create a new event in Cronicle")]
  public async Task CreateNewEvent()
  {
    // Arrange
    var newEvent = new NewEvent()
    {
      Title = "A title",
      Enabled = true,
      Category = "createACHReturn",
      Plugin = "testplug",
      Target = "allgrp",
      Timing = new Timing()
      {
        Hours = [4],
        Minutes = [27],
        Days = [5],
        Months = [1],
        Years = [2024]
      }
    };

    // Act
    var newEventId = await _cronicleClient.Event.Create(eventData: newEvent, cancellationToken: _cancellationToken);

    // Assert
    newEventId.Should().NotBeEmpty();
  }
  
  [Fact(DisplayName = "Create a new event and validate it exists in Cronicle")]
  public async Task CreateNewEventAndValidate()
  {
    // Arrange
    var newEvent = new NewEvent()
    {
      Title = "A title",
      Enabled = true,
      Category = "createACHReturn",
      Plugin = "testplug",
      Target = "allgrp",
      Timing = new Timing()
      {
        Hours = [4],
        Minutes = [27],
        Days = [5],
        Months = [1],
        Years = [2024]
      }
    };
    var newEventId = await _cronicleClient.Event.Create(eventData: newEvent, cancellationToken: _cancellationToken);
    newEventId.Should().NotBeEmpty();

    // Act
    var eventDetails = await _cronicleClient.Event.GetById(eventId: newEventId, cancellationToken: _cancellationToken);

    // Assert
    eventDetails.Should().NotBeNull();
    eventDetails!.Title.Should().Be(newEvent.Title);
    eventDetails!.Category.Should().Be(newEvent.Category);
    eventDetails!.Plugin.Should().Be(newEvent.Plugin);
    eventDetails!.Target.Should().Be(newEvent.Target);
    eventDetails!.Timing.Should().BeEquivalentTo(newEvent.Timing);
    eventDetails.Enabled.Should().BeTrue();
  }
}