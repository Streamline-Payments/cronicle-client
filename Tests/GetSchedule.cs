using CronicleClient;
using CronicleClient.Models;
using FluentAssertions;
using Xunit.Abstractions;

namespace Tests;

[Collection("Cronicle Client collection")]
public class GetSchedule(ITestOutputHelper outputHelper)
{
  private readonly CancellationToken _cancellationToken = new CancellationTokenSource().Token;
  private readonly Client _cronicleClient = Common.InitCronicleClient(outputHelper);

  [Fact(DisplayName = "Get an schedule that exists in Cronicle")]
  public async Task GetExistentSchedule()
  {
    // Arrange

    // Act
    var eventDetails = await _cronicleClient.Event.GetSchedule(cancellationToken: _cancellationToken);

    // Assert
    eventDetails.Should().NotBeNull();
  }

  [Fact(DisplayName = "Get an schedule with limit in Cronicle")]
  public async Task GetExistentScheduleWithLimit()
  {
    // Arrange
    var limit = 10;
    var offset = 0;

    // Act
    var eventDetails = await _cronicleClient.Event.GetSchedule(limit, offset, _cancellationToken);

    // Assert
    eventDetails.Should().NotBeNull();
    eventDetails.Count().Should().BeLessThanOrEqualTo(limit);
  }

  [Fact(DisplayName = "Get an schedule with offset in Cronicle")]
  public async Task GetExistentScheduleWithOffset()
  {
    // Arrange
    var offset = 3;

    // Act
    var eventDetails = await _cronicleClient.Event.GetSchedule(50, offset, _cancellationToken);

    // Assert
    eventDetails.Should().NotBeNull();
  }

  [Fact(DisplayName = "Get an schedule with new event in Cronicle")]
  public async Task GetScheduleWithEvents()
  {
    // Arrange
    var newEvent = new NewEvent
    {
      Title = "New event",
      Enabled = true,
      Category = "cmga2zvlc1j",
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
    var eventDetails = await _cronicleClient.Event.GetSchedule(cancellationToken: _cancellationToken);

    // Assert
    eventDetails.Should().NotBeNull();
    eventDetails.FirstOrDefault(e => e.Id == eventId).Should().NotBeNull();

    // Cleanup
    await _cronicleClient.Event.Delete(eventId, _cancellationToken);
  }

  [Theory]
  [InlineData(4)]
  [InlineData(2)]
  [InlineData(5)]
  public async Task GetScheduleWithMultipleNewEvents(int eventsNumber)
  {
    // Arrange
    var newIds = new List<string>();
    for (var i = 0; i < eventsNumber; i++)
    {
      var newEvent = new NewEvent
      {
        Title = $"New event {i + 1}",
        Enabled = true,
        Category = "cmga2zvlc1j",
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
      newIds.Add(eventId);
    }

    // Act
    var eventDetails = await _cronicleClient.Event.GetSchedule(eventsNumber,0, _cancellationToken);

    // Assert
    eventDetails.Should().NotBeNull();
    foreach (var id in newIds)
    {
      eventDetails.FirstOrDefault(e => e.Id == id).Should().NotBeNull();
      // Cleanup
      await _cronicleClient.Event.Delete(id, _cancellationToken);
    }
  }
}