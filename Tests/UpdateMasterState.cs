using CronicleClient;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Tests
{
    public class UpdateMasterState
    {
        private readonly Client _cronicleClient;
        private readonly CancellationToken _cancellationToken;

        public UpdateMasterState(ITestOutputHelper outputHelper)
        {
            var serverUrl = "http://localhost:3012";
            var apiKey = "240db114e152267349f17cf768808169";
            var logger = outputHelper.ToLogger<CreateEvent>();

            _cronicleClient = new Client(baseUrl: serverUrl, apiToken: apiKey, logger: logger);
            _cancellationToken = new CancellationTokenSource().Token;
        }      
        
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task UpdateMasterStatus(bool newStatus)
        {
            // Arrange

            // Act
            await FluentActions.Invoking(() => _cronicleClient.Master.UpdateMasterState(newStatus, cancellationToken: _cancellationToken))
                .Should().NotThrowAsync<Exception>();

            // Assert
            var masterStatus = await _cronicleClient.Master.GetMasterState(cancellationToken: _cancellationToken);
            masterStatus.Should().Be(newStatus);
        }

        [Fact(DisplayName = "Toggles master state correctly")]
        public async Task ToggleMasterStatus()
        {
            // Arrange
            var initialStatus = await _cronicleClient.Master.GetMasterState(cancellationToken: _cancellationToken);
            var newStatus = !initialStatus;

            // Act
            await FluentActions.Invoking(() => _cronicleClient.Master.UpdateMasterState(newStatus, cancellationToken: _cancellationToken))
                .Should().NotThrowAsync<Exception>();

            // Assert
            var updatedStatus = await _cronicleClient.Master.GetMasterState(cancellationToken: _cancellationToken);
            updatedStatus.Should().Be(newStatus);

            // Cleanup 
            await FluentActions.Invoking(() => _cronicleClient.Master.UpdateMasterState(initialStatus, cancellationToken: _cancellationToken))
                .Should().NotThrowAsync<Exception>();
        }

    }
}
