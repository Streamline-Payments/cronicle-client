using System.Text.Json;
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
       var eventDetails = await _cronicleClient.Event.GetById(eventId, _cancellationToken);

       // Assert
       eventDetails.Should().NotBeNull();

       // Cleanup
       await _cronicleClient.Event.Delete(eventId, _cancellationToken);
     }

  [Fact(DisplayName = "Get an on-demand event that exists in Cronicle")]
  public async Task GetOnDemandEvent()
  {
    // Arrange
    var newEvent = new NewEvent
    {
      Title = "A title",
      Enabled = true,
      Category = "cmga2zvlc1j",
      Plugin = "testplug",
      Target = "allgrp",
      Timing = null
    };
    var eventId = await _cronicleClient.Event.Create(newEvent, _cancellationToken);
    eventId.Should().NotBeEmpty();

    // Act
    var eventDetails = await _cronicleClient.Event.GetById(eventId, _cancellationToken);

    // Assert
    eventDetails.Should().NotBeNull();
    eventDetails!.Timing.Should().BeNull();

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

  [Fact(DisplayName = "Parse an event with bool timing value")]
  public async Task GetBoolTimingEvent()
  {
    // Arrange
    var newEvent = """
                    {
                     "catch_up": 0,
                     "category": "general",
                     "enabled": 1,
                     "params": {
                       "log_level": "",
                       "unit_of_time": "Day",
                       "start_date": ""
                     },
                     "plugin": "12345",
                     "retries": 0,
                     "target": "allgrp",
                     "timezone": "UTC",
                     "timing": false,
                     "title": "API Enabled 45179 1759433458310.6936",
                     "id": "emg9t94zs03",
                     "modified": 1759448614,
                     "created": 1759433458,
                     "max_children": 1,
                     "timeout": 0,
                     "api_key": "aaaaaaaaaaaa",
                     "salt": "",
                     "silent": 0,
                     "concurrent_arg": 0,
                     "graph_icon": "",
                     "args": "",
                     "ticks": "",
                     "algo": "random",
                     "multiplex": 0,
                     "stagger": 0,
                     "options": {},
                     "start_time": null,
                     "end_time": null,
                     "interval": false,
                     "interval_start": false,
                     "repeat": false,
                     "retry_delay": 0,
                     "detached": 0,
                     "queue": 0,
                     "queue_max": 0,
                     "chain": "",
                     "chain_error": "",
                     "notify_success": "",
                     "notify_fail": "",
                     "web_hook": "",
                     "web_hook_start": "",
                     "web_hook_error": 0,
                     "cpu_limit": 0,
                     "cpu_sustain": 0,
                     "memory_limit": 0,
                     "memory_sustain": 0,
                     "log_max_size": 0,
                     "notes": ""
                   }
                   """;

    // Act
    var newEventDetails = JsonSerializer.Deserialize<NewEvent>(newEvent);
    newEventDetails.Should().NotBeNull();
    newEventDetails!.Timing.Should().BeNull();

    // Act
    var eventDataDetails = JsonSerializer.Deserialize<EventData>(newEvent);
    eventDataDetails.Should().NotBeNull();
    eventDataDetails!.Timing.Should().BeNull();

    // Act
    var newEventStr = JsonSerializer.Serialize(newEventDetails);
    newEventStr.Should().NotBeEmpty();
    newEventStr.Should().Contain("\"timing\":null");

    // Act
    var eventDataStr = JsonSerializer.Serialize(eventDataDetails);
    eventDataStr.Should().NotBeEmpty();
    eventDataStr.Should().Contain("\"timing\":null");
  }

  [Fact(DisplayName = "Parse an event with null timing value")]
  public async Task GetNullTimingEvent()
  {
    // Arrange
    var newEvent = """
                    {
                     "catch_up": 0,
                     "category": "general",
                     "enabled": 1,
                     "params": {
                       "log_level": "",
                       "unit_of_time": "Day",
                       "start_date": ""
                     },
                     "plugin": "12345",
                     "retries": 0,
                     "target": "allgrp",
                     "timezone": "UTC",
                     "timing": null,
                     "title": "API Enabled 45179 1759433458310.6936",
                     "id": "emg9t94zs03",
                     "modified": 1759448614,
                     "created": 1759433458,
                     "max_children": 1,
                     "timeout": 0,
                     "api_key": "aaaaaaaaaaaa",
                     "salt": "",
                     "silent": 0,
                     "concurrent_arg": 0,
                     "graph_icon": "",
                     "args": "",
                     "ticks": "",
                     "algo": "random",
                     "multiplex": 0,
                     "stagger": 0,
                     "options": {},
                     "start_time": null,
                     "end_time": null,
                     "interval": false,
                     "interval_start": false,
                     "repeat": false,
                     "retry_delay": 0,
                     "detached": 0,
                     "queue": 0,
                     "queue_max": 0,
                     "chain": "",
                     "chain_error": "",
                     "notify_success": "",
                     "notify_fail": "",
                     "web_hook": "",
                     "web_hook_start": "",
                     "web_hook_error": 0,
                     "cpu_limit": 0,
                     "cpu_sustain": 0,
                     "memory_limit": 0,
                     "memory_sustain": 0,
                     "log_max_size": 0,
                     "notes": ""
                   }
                   """;

    // Act
    var newEventDetails = JsonSerializer.Deserialize<NewEvent>(newEvent);
    newEventDetails.Should().NotBeNull();
    newEventDetails!.Timing.Should().BeNull();

    // Act
    var eventDataDetails = JsonSerializer.Deserialize<EventData>(newEvent);
    eventDataDetails.Should().NotBeNull();
    eventDataDetails!.Timing.Should().BeNull();
  }
}