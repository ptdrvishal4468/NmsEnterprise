using Nms.Domain.Enums;
using Nms.Domain.Models;
using Nms.Infrastructure.Connectivity;
using Xunit;

namespace Nms.UnitTests;

public class IcmpConnectionAdapterTests
{
    private readonly IcmpConnectionAdapter _adapter = new();

    [Fact]
    public async Task TestConnectionAsync_ShouldReturnFailure_WhenHostOrIpIsEmpty()
    {
        // Arrange
        var parameters = ConnectionParameters.ForIcmp(string.Empty);

        // Act
        var result = await _adapter.TestConnectionAsync(parameters, CancellationToken.None);

        // Assert
        Assert.False(result.IsConnected);
        Assert.Equal(NetworkProtocol.Icmp, result.Protocol);
        Assert.Contains("Host or IP address is required", result.ErrorMessage);
    }

    [Fact]
    public async Task TestConnectionAsync_ShouldReturnFailure_WhenPingTimesOutOrFails()
    {
        // Arrange - Using a non-routable IP address to force timeout/failure safely
        var parameters = ConnectionParameters.ForIcmp("192.0.2.254", TimeSpan.FromMilliseconds(500));

        // Act
        var result = await _adapter.TestConnectionAsync(parameters, CancellationToken.None);

        // Assert
        Assert.False(result.IsConnected);
        Assert.Equal(NetworkProtocol.Icmp, result.Protocol);
        Assert.NotNull(result.ErrorMessage);
    }
}