using CronicleClient;
using CronicleClient.Models;
using FluentAssertions;
using FluentAssertions.Equivalency;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Tests;

public class CreateEvent
{
    private readonly Client _cronicleClient;
    private readonly CancellationToken _cancellationToken;

    public CreateEvent(ITestOutputHelper outputHelper)
    {
        var serverUrl = "http://localhost:3012";
        var apiKey = "240db114e152267349f17cf768808169";
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
            Category = "general",
            Plugin = "testplug",
            Target = "allgrp",
            Timing = new Timing()
            {
                Hours = [4],
                Minutes = [27],
                Days = [5],
                Months = [1],
                Years = [0]
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
            Category = "general",
            Plugin = "shellplug",
            Target = "allgrp",
            Timing = new Timing()
            {
                Hours = [4],
                Minutes = [27],
                Days = [5],
                Months = [1],
                Years = [2024]
            },
            Parameters = new Dictionary<string, object>()
            {
                { "script", """
                            echo 'Hello, World!'

                            echo 'This is a test of the Cronicle Shell Plugin'
                            """ },
                { "annotate", true },
                { "json", true }
            }
        };

        // Act
        var newEventId = await _cronicleClient.Event.Create(eventData: newEvent, cancellationToken: _cancellationToken);

        // Assert
        newEventId.Should().NotBeEmpty();

        // Cleanup
        await _cronicleClient.Event.Delete(eventId: newEventId, cancellationToken: _cancellationToken);
    }

    [Fact(DisplayName = "Create a new event with invalid params")]
    public async Task CreateEventWithInvalidParams()
    {
        // Arrange
        var newEvent = new NewEvent()
        {
            Title = "A title",
            Enabled = true,
            Category = string.Empty,
            Plugin = string.Empty,
            Target = string.Empty,
            Timing = new Timing()
            {
                Hours = [4],
                Minutes = [27],
                Days = [5],
                Months = [1],
                Years = [2024]
            }
        };

        // Act & Assert
        await FluentActions.Invoking(() => _cronicleClient.Event.Create(eventData: newEvent, cancellationToken: _cancellationToken))
            .Should().ThrowAsync<Exception>();
    }

    [Fact(DisplayName = "Create a new event and validate it exists in Cronicle")]
    public async Task CreateNewEventAndValidate()
    {
        // Arrange
        var newEvent = new NewEvent()
        {
            Title = "A title",
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

        // Cleanup
        await _cronicleClient.Event.Delete(eventId: newEventId, cancellationToken: _cancellationToken);
    }

    [Theory]
    [InlineData(new[] { 1, 2, 3 }, new[] { 15, 30, 45 }, new[] { 1, 2, 3 }, new[] { 1, 2, 3 }, new[] { 2023, 2024, 2025 })]
    [InlineData(new[] { 0, 12, 23 }, new[] { 0, 30, 59 }, new[] { 10, 20, 30 }, new[] { 4, 6, 12 }, new[] { 2020, 2021, 2022 })]
    [InlineData(new[] { 6, 18 }, new[] { 15, 45 }, new[] { 5, 15 }, new[] { 3, 9 }, new[] { 2022, 2023 })]
    public async Task CreateEventInlineParams(int[] hours, int[] minutes, int[] days, int[] months, int[] year)
    {
        // Arrange
        var newEvent = new NewEvent()
        {
            Title = "New event",
            Enabled = true,
            Category = "general",
            Plugin = "testplug",
            Target = "allgrp",
            Timing = new Timing()
            {
                Hours = new List<int>(hours),
                Minutes = new List<int>(minutes),
                Days = new List<int>(days),
                Months = new List<int>(months),
                Years = new List<int>(year)
            }
        };
        var eventId = await _cronicleClient.Event.Create(eventData: newEvent, cancellationToken: _cancellationToken);
        eventId.Should().NotBeEmpty();

        // Act
        var eventDetails = await _cronicleClient.Event.GetById(eventId, cancellationToken: _cancellationToken);

        // Assert
        eventDetails.Should().NotBeNull();
        eventDetails.Timing.Hours.Should().BeEquivalentTo(hours);
        eventDetails.Timing.Minutes.Should().BeEquivalentTo(minutes);
        eventDetails.Timing.Days.Should().BeEquivalentTo(days);
        eventDetails.Timing.Months.Should().BeEquivalentTo(months);
        eventDetails.Timing.Years.Should().BeEquivalentTo(year);

        // Cleanup
        await _cronicleClient.Event.Delete(eventId: eventId, cancellationToken: _cancellationToken);
    }

    [Fact]
    public async Task CreateEventMoreParams()
    {
        // Arrange
        var newEvent = new NewEvent()
        {
            Title = "New event",
            Enabled = true,
            Category = "general",
            Plugin = "testplug",
            Target = "allgrp",
            Timing = new Timing()
            {
                Hours = [2],
                Minutes = [30],
                Days = [6],
                Months = [12],
                Years = []
            },
            WebHook = "http://myserver.com/notify-chronos.php",
            CatchUp = true,
            CpuLimit = 100,
            CpuSustain = 0,
            Detached = false,
            LogMaxSize = 100,
            MaxChildren = 1,
            MemoryLimit = 100,
            MemorySustain = 0,
            Multiplex = false,
            NotifyFail = "example@fail.com",
            Timezone = "America/New_York",
            Timeout = 500,
            Stagger = 55,
            RetryDelay = 0,
            Retries = 4,
            NotifySuccess = "exmple@success.com",
            Notes="Test note"
        };
        var eventId = await _cronicleClient.Event.Create(eventData: newEvent, cancellationToken: _cancellationToken);
        eventId.Should().NotBeEmpty();

        // Act
        var eventDetails = await _cronicleClient.Event.GetById(eventId, cancellationToken: _cancellationToken);

        // Assert
        eventDetails.Should().NotBeNull();
        eventDetails.Timing.Hours.Should().BeEquivalentTo(newEvent.Timing.Hours);
        eventDetails.Timing.Minutes.Should().BeEquivalentTo(newEvent.Timing.Minutes);
        eventDetails.Timing.Days.Should().BeEquivalentTo(newEvent.Timing.Days);
        eventDetails.Timing.Months.Should().BeEquivalentTo(newEvent.Timing.Months);
        eventDetails.Timing.Years.Should().BeEquivalentTo(newEvent.Timing.Years);
        eventDetails.Notes.Should().Be(newEvent.Notes);
        eventDetails.NotifySuccess.Should().Be(newEvent.NotifySuccess);
        eventDetails.NotifyFail.Should().Be(newEvent.NotifyFail);
        eventDetails.Retries.Should().Be(newEvent.Retries);
        eventDetails.RetryDelay.Should().Be(newEvent.RetryDelay);
        eventDetails.CatchUp.Should().Be(newEvent.CatchUp);
        eventDetails.WebHook.Should().Be(newEvent.WebHook);
        eventDetails.CpuLimit.Should().Be(newEvent.CpuLimit);
        eventDetails.CpuSustain.Should().Be(newEvent.CpuSustain);
        eventDetails.Timezone.Should().Be(newEvent.Timezone);
        eventDetails.MaxChildren.Should().Be(newEvent.MaxChildren);

        // Cleanup
        await _cronicleClient.Event.Delete(eventId: eventId, cancellationToken: _cancellationToken);
    }


    [Theory]
    [InlineData("Example title", "http://iamawebhook.com/v1.php", false, "note test", "success@yopmail.com", "fail@yopmail.com", 600)]
    [InlineData("A title", "http://mywebhook.com.php", false, "new note", "success@example.com", "fail@example.com", 0)]
    [InlineData("A new title", "http://hello.com.com", false, "example note", "success@mail.com", "", 100)]
    public async Task CreateEventMoreInlineParams(string Title, string WebHook, bool CatchUp, string Notes, string NotifySuccess, string NotifyFail, int Timeout)
    {
        // Arrange
        var newEvent = new NewEvent()
        {
            Title = Title,
            Enabled = true,
            Category = "general",
            Plugin = "testplug",
            Target = "allgrp",
            Timing = new Timing()
            {
                Hours = [2],
                Minutes = [30],
                Days = [6],
                Months = [12],
                Years = []
            },
            WebHook = WebHook,
            CatchUp = CatchUp,
            NotifyFail = NotifyFail,
            Timeout = Timeout,
            NotifySuccess = NotifySuccess,
            Notes = Notes
        };
        var eventId = await _cronicleClient.Event.Create(eventData: newEvent, cancellationToken: _cancellationToken);
        eventId.Should().NotBeEmpty();

        // Act
        var eventDetails = await _cronicleClient.Event.GetById(eventId, cancellationToken: _cancellationToken);

        // Assert
        eventDetails.Should().NotBeNull();
        eventDetails.Notes.Should().Be(newEvent.Notes);
        eventDetails.NotifySuccess.Should().Be(newEvent.NotifySuccess);
        eventDetails.NotifyFail.Should().Be(newEvent.NotifyFail);
        eventDetails.CatchUp.Should().Be(newEvent.CatchUp);
        eventDetails.WebHook.Should().Be(newEvent.WebHook);
        eventDetails.Timeout.Should().Be(newEvent.Timeout);
        eventDetails.Title.Should().Be(newEvent.Title);

        // Cleanup
        await _cronicleClient.Event.Delete(eventId: eventId, cancellationToken: _cancellationToken);
    }
}