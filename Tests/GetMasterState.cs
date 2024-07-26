using CronicleClient;
using FluentAssertions;
using Xunit.Abstractions;

namespace Tests;

public class GetMasterState(ITestOutputHelper outputHelper)
{
  private readonly CancellationToken _cancellationToken = new CancellationTokenSource().Token;
  private readonly Client _cronicleClient = Common.InitCronicleClient(outputHelper);

  [Fact(DisplayName = "Get master state in Cronicle")]
  public async Task GetMasterStatus()
  {
    // Arrange

    // Act & Assert
    await FluentActions.Invoking(() => _cronicleClient.Master.GetMasterState(_cancellationToken))
      .Should().NotThrowAsync<Exception>();
  }

  [Fact(DisplayName = "Get master state multiple times for consistency")]
  public async Task GetMasterStateMultipleTimesForConsistency()
  {
    // Act
    var masterState1 = await _cronicleClient.Master.GetMasterState(_cancellationToken);
    var masterState2 = await _cronicleClient.Master.GetMasterState(_cancellationToken);

    // Assert
    masterState1.Should().Be(masterState2);
  }

  [Fact(DisplayName = "Get master state and verify enabled")]
  public async Task GetMasterStateAndVerifyEnabled()
  {
    // Arrange
    await _cronicleClient.Master.UpdateMasterState(true, _cancellationToken);

    // Act
    var masterState = await _cronicleClient.Master.GetMasterState(_cancellationToken);

    // Assert
    masterState.Should().Be(true);
  }

  [Fact(DisplayName = "Get master state and verify disabled")]
  public async Task GetMasterStateAndVerifyDisabled()
  {
    // Arrange
    await _cronicleClient.Master.UpdateMasterState(false, _cancellationToken);

    // Act
    var masterState = await _cronicleClient.Master.GetMasterState(_cancellationToken);

    // Assert
    masterState.Should().Be(false);
  }
}