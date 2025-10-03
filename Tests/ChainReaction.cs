using CronicleClient;
using CronicleClient.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Tests;

[Collection("Cronicle Client collection")]
public class ChainReaction(ITestOutputHelper outputHelper)
{
    private readonly CancellationToken _cancellationToken = new CancellationTokenSource().Token;
    private readonly Client _cronicleClient = Common.InitCronicleClient(outputHelper);


    [Fact(DisplayName = "Create a new event with a chain event")]
    public async Task CreateEventWithChain()
    {
        var randomIdentifier = Path.GetRandomFileName();

        // Arrange
        var chainEvent = new NewEvent
        {
            Title= $"Chain event {randomIdentifier}",
            Enabled= true,
            Category = "cmga2zvlc1j",
            Plugin = "testplug",
            Target = "allgrp",
            Parameters = new Dictionary<string, object>
            {
               { "duration", 1 }, // 1 second
            }
        };

        var chainId = await _cronicleClient.Event.Create(eventData: chainEvent);

        var newEvent = new NewEvent
        {
            Title = $"Event parent of a chain {randomIdentifier}",
            Enabled = true,
            Category = "cmga2zvlc1j",
            Plugin = "testplug",
            Target = "allgrp",
            Chain = chainId,
            ChainData = new Dictionary<string, object>() {
                { "Param1", "P1" },
                { "Param2", 120 },
                { "Param3", "P2" },
            },
            ChainParams = new Dictionary<string, object>() {
                { "Param1", "P1" },
                { "Param2", 120 },
                { "Param3", "P2" },
            },
            Timing = new Timing
            {
                Hours = [4],
                Minutes = [27],
                Days = [5],
                Months = [1],
                Years = [2024]
            },
            Parameters = new Dictionary<string, object>
            {
               { "duration", 1 }, // 1 second
            }
        };
        var newEventId = await _cronicleClient.Event.Create(newEvent, _cancellationToken);

        // Act
        var eventRunning = await _cronicleClient.Event.RunEventById(newEventId);
        await Task.Delay(2000, _cancellationToken);

        // Assert
        var eventCreated = await _cronicleClient.Event.GetById(newEventId);

        eventCreated.ChainData.Count.Should().Be(3);
        eventCreated.ChainData.ContainsKey("Param1").Should().BeTrue();
        eventCreated.ChainData.ContainsKey("Param2").Should().BeTrue();
        eventCreated.ChainData.ContainsKey("Param3").Should().BeTrue();

        // Cleanup
        await Task.Delay(1500);

        await _cronicleClient.Event.Delete(chainId, _cancellationToken);
        await _cronicleClient.Event.Delete(newEventId, _cancellationToken);
    }

    [Fact(DisplayName = "Create a event with chain and validate chained event")]
    public async Task ValidateChainedEvent()
    {
        var randomIdentifier = Path.GetRandomFileName();

        // Arrange
        var chainEvent = new NewEvent
        {
            Title = $"Chain event {randomIdentifier}",
            Enabled = true,
            Category = "cmga2zvlc1j",
            Plugin = "testplug",
            Target = "allgrp",
            Parameters = new Dictionary<string, object>
            {
               { "duration", 1 }, // 1 second
            }
        };

        var chainId = await _cronicleClient.Event.Create(eventData: chainEvent);

        var newEvent = new NewEvent
        {
            Title = $"Event parent of a chain {randomIdentifier}",
            Enabled = true,
            Category = "cmga2zvlc1j",
            Plugin = "testplug",
            Target = "allgrp",
            Chain = chainId,
            Timing = new Timing
            {
                Hours = [4],
                Minutes = [27],
                Days = [5],
                Months = [1],
                Years = [2024]
            },
            Parameters = new Dictionary<string, object>
            {
               { "duration", 1 }, // 1 second
            }
        };
        var newEventId = await _cronicleClient.Event.Create(newEvent, _cancellationToken);

        // Act
        var eventRunning = await _cronicleClient.Event.RunEventById(newEventId);
        await Task.Delay(2000, _cancellationToken);

        // Assert

        var runningEvents = await _cronicleClient.Job.GetActiveJobs();
        runningEvents!.First(q => q.Value.EventTitle == chainEvent.Title).Should().NotBeNull();

        // Cleanup
        await Task.Delay(1500);

        await _cronicleClient.Event.Delete(chainId, _cancellationToken);
        await _cronicleClient.Event.Delete(newEventId, _cancellationToken);
    }

    [Fact(DisplayName = "Validate chain when event fails")]
    public async Task ValidateEventFails()
    {
        var randomIdentifier = Path.GetRandomFileName();

        // Arrange
        var chainEvent = new NewEvent
        {
            Title = $"Chain event {randomIdentifier}",
            Enabled = true,
            Category = "cmga2zvlc1j",
            Plugin = "testplug",
            Target = "allgrp",
            Parameters = new Dictionary<string, object>
            {
               { "duration", 1 }, // 1 second
            },
            Timing = new Timing
            {
                Hours = [4],
                Minutes = [27],
                Days = [25],
                Months = [8],
                Years = [2024]
            }
        };

        var chainId = await _cronicleClient.Event.Create(eventData: chainEvent);

        // Arrange
        var newEvent = new NewEvent
        {
            Title = $"Event parent of a chain {randomIdentifier}",
            Enabled = true,
            Category = "cmga2zvlc1j",
            Plugin = "testplug",
            Target = "allgrp",
            Chain = chainId,
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

        var jobId = await _cronicleClient.Event.RunEventById(newEventId, _cancellationToken);
        jobId.Should().NotBeEmpty();
        jobId.Length.Should().Be(1);

        // Act
        await _cronicleClient.Job.AbortJob(jobId.First(), _cancellationToken);
        await Task.Delay(1500);

        var runningEvents = await _cronicleClient.Job.GetActiveJobs();
        runningEvents.Should().BeEmpty();

        // Cleanup

        await _cronicleClient.Event.Delete(chainId, _cancellationToken);
        await _cronicleClient.Event.Delete(newEventId, _cancellationToken);
    }


}