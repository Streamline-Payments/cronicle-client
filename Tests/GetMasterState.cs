﻿using CronicleClient;
using FluentAssertions;
using Xunit.Abstractions;

namespace Tests;

[Collection("Cronicle Client collection")]
public class GetMasterState(ITestOutputHelper outputHelper)
{
  private readonly CancellationToken _cancellationToken = new CancellationTokenSource().Token;
  private readonly Client _cronicleClient = Common.InitCronicleClient(outputHelper);

  [Fact(DisplayName = "Get master state in Cronicle")]
  public async Task GetMasterStatus()
  {
    // Arrange

    // Act
    await _cronicleClient.Master.GetMasterState(_cancellationToken);
  }

  [Fact(DisplayName = "Get master state and verify enabled")]
  public async Task GetMasterStateAndVerifyEnabled()
  {
    // Arrange

    // Act
    await _cronicleClient.Master.UpdateMasterState(true, _cancellationToken);

    // Assert
    var masterState = await _cronicleClient.Master.GetMasterState(_cancellationToken);
    masterState.Should().BeTrue();
  }

  [Fact(DisplayName = "Get master state and verify disabled")]
  public async Task GetMasterStateAndVerifyDisabled()
  {
    // Arrange

    // Act
    await _cronicleClient.Master.UpdateMasterState(false, _cancellationToken);
    await Task.Delay(1000, _cancellationToken);

    // Assert
    var masterState = await _cronicleClient.Master.GetMasterState(_cancellationToken);
    masterState.Should().BeFalse();
    
    // Cleanup
    await _cronicleClient.Master.UpdateMasterState(true, _cancellationToken);
  }
}