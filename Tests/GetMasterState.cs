using CronicleClient;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Tests;

public class GetMasterState
{
    private readonly Client _cronicleClient;
    private readonly CancellationToken _cancellationToken;

    public GetMasterState(ITestOutputHelper outputHelper)
    {
        var serverUrl = "http://localhost:3012";
        var apiKey = "240db114e152267349f17cf768808169";
        var logger = outputHelper.ToLogger<CreateEvent>();

        _cronicleClient = new Client(baseUrl: serverUrl, apiToken: apiKey, logger: logger);
        _cancellationToken = new CancellationTokenSource().Token;
    }

    [Fact(DisplayName = "Get master state in Cronicle")]
    public async Task GetMasterStatus()
    {
        // Arrange

        // Act & Assert
        await FluentActions.Invoking(() => _cronicleClient.Master.GetMasterState(cancellationToken: _cancellationToken))
            .Should().NotThrowAsync<Exception>();
    }

    [Fact(DisplayName = "Get master state multiple times for consistency")]
    public async Task GetMasterStateMultipleTimesForConsistency()
    {
        // Act
        var masterState1 = await _cronicleClient.Master.GetMasterState(cancellationToken: _cancellationToken);
        var masterState2 = await _cronicleClient.Master.GetMasterState(cancellationToken: _cancellationToken);

        // Assert
        masterState1.Should().Be(masterState2);
    }

    [Fact(DisplayName = "Get master state and verify enabled")]
    public async Task GetMasterStateAndVerifyEnabled()
    {
        // Arrange
        await _cronicleClient.Master.UpdateMasterState(true, cancellationToken: _cancellationToken);

        // Act
        var masterState = await _cronicleClient.Master.GetMasterState(cancellationToken: _cancellationToken);

        // Assert
        masterState.Should().Be(true);
    }

    [Fact(DisplayName = "Get master state and verify disabled")]
    public async Task GetMasterStateAndVerifyDisabled()
    {
        // Arrange
        await _cronicleClient.Master.UpdateMasterState(false, cancellationToken: _cancellationToken);

        // Act
        var masterState = await _cronicleClient.Master.GetMasterState(cancellationToken: _cancellationToken);

        // Assert
        masterState.Should().Be(false);
    }


}
