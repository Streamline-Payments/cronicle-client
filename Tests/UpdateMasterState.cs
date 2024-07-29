using CronicleClient;
using FluentAssertions;
using Xunit.Abstractions;

namespace Tests;

[Collection("Cronicle Client collection")]
public class UpdateMasterState(ITestOutputHelper outputHelper)
{
  private readonly CancellationToken _cancellationToken = new CancellationTokenSource().Token;
  private readonly Client _cronicleClient = Common.InitCronicleClient(outputHelper);

  [Theory]
  [InlineData(true)]
  [InlineData(false)]
  public async Task UpdateMasterStatus(bool newStatus)
  {
    // Arrange

    // Act
    await _cronicleClient.Master.UpdateMasterState(newStatus, _cancellationToken);

    // Assert
    var masterStatus = await _cronicleClient.Master.GetMasterState(_cancellationToken);
    masterStatus.Should().Be(newStatus);
  }

  [Fact(DisplayName = "Toggles master state correctly")]
  public async Task ToggleMasterStatus()
  {
    // Arrange
    var initialStatus = await _cronicleClient.Master.GetMasterState(_cancellationToken);
    var newStatus = !initialStatus;

    // Act
    await  _cronicleClient.Master.UpdateMasterState(newStatus, _cancellationToken);

    // Assert
    var updatedStatus = await _cronicleClient.Master.GetMasterState(_cancellationToken);
    updatedStatus.Should().Be(newStatus);

    // Cleanup 
    await _cronicleClient.Master.UpdateMasterState(initialStatus, _cancellationToken);
  }
}