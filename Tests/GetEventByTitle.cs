using CronicleClient;
using CronicleClient.Models;
using FluentAssertions;
using Xunit.Abstractions;

namespace Tests;

[Collection("Cronicle Client collection")]
public class GetEventByTitle(ITestOutputHelper outputHelper)
{
  private readonly CancellationToken _cancellationToken = new CancellationTokenSource().Token;
  private readonly Client _cronicleClient = Common.InitCronicleClient(outputHelper);

  [Fact(DisplayName = "Get an event that exists in Cronicle by its title")]
  public async Task GettEventByTitle()
  {
    // Arrange
    var newEvent = new NewEvent
    {
      Title = "New title",
      Enabled = true,
      Category = "cmga2zvlc1j",
      Plugin = "shellplug",
      Target = "allgrp",
      Timing = new Timing
      {
        Hours = [4],
        Minutes = [27],
        Days = [25],
        Months = [8],
        Years = [2024]
      },
      Parameters = new Dictionary<string, object>
      {
        {
          "script", """
                    echo 'Hello, World!'

                    echo 'This is a test of the Cronicle Shell Plugin'
                    """
        },
        { "annotate", true },
        { "json", true }
      }
    };
    var eventId = await _cronicleClient.Event.Create(newEvent, _cancellationToken);
    eventId.Should().NotBeEmpty();

    // Act
    var eventData = await _cronicleClient.Event.GetByTitle(newEvent.Title, _cancellationToken);

    // Assert
    eventData.Should().NotBeNull();
    eventData!.Title.Should().Be(newEvent.Title);
    eventData!.Plugin.Should().Be(newEvent.Plugin);
    eventData!.Target.Should().Be(newEvent.Target);
    eventData!.Timing.Should().BeEquivalentTo(newEvent.Timing);
    eventData!.Category.Should().Be(newEvent.Category);
    eventData!.Enabled.Should().BeTrue();

    // Convert all params to string to compare
    eventData!.Parameters.Should().Contain(q => q.Key == "script" && q.Value.ToString() == newEvent.Parameters[q.Key].ToString());
    eventData!.Parameters.Should().Contain(q => q.Key == "annotate" && q.Value.ToString() == newEvent.Parameters[q.Key].ToString());
    eventData!.Parameters.Should().Contain(q => q.Key == "json" && q.Value.ToString() == newEvent.Parameters[q.Key].ToString());

    // Cleanup
    await _cronicleClient.Event.Delete(eventId, _cancellationToken);
  }

  [Fact(DisplayName = "Get an event that exists does not exist in Cronicle by its title")]
  public async Task GetNotExistentEventByTitle()
  {
    // Arrange
    var eventTitle = "NoTitle";

    // Act
    await FluentActions.Invoking(() => _cronicleClient.Event.GetByTitle(eventTitle, _cancellationToken))
      .Should().ThrowAsync<KeyNotFoundException>();
  }

  [Fact(DisplayName = "Get an event with invalid title")]
  public async Task GetEventWithInvalidId()
  {
    // Arrange
    var invalidTitle = string.Empty;

    // Act & Assert
    await FluentActions.Invoking(() => _cronicleClient.Event.GetByTitle(invalidTitle, _cancellationToken))
      .Should().ThrowAsync<Exception>();
  }
}