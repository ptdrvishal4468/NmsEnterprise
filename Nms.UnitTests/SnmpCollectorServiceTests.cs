using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Infrastructure.Telemetry;
using Xunit;

namespace Nms.UnitTests;

public class SnmpCollectorServiceTests
{
    private readonly SnmpCollectorService _service;

    public SnmpCollectorServiceTests()
    {
        _service = new SnmpCollectorService();
    }

    [Fact]
    public async Task PollDeviceAsync_ShouldReturnFailedResult_WhenDeviceIpIsInvalid()
    {
        // Arrange
        var device = new Device(
            id: Guid.NewGuid(),
            tenantId: Guid.NewGuid(),
            name: "Test Router",
            ipAddress: "256.256.256.256",
            deviceType: DeviceType.Router
        );

        IEnumerable<string> oids = new List<string>
        {
            OidConstants.CiscoCpu5Min,
            OidConstants.HostMemoryUsed
        };

        // Act
        var result = await _service.PollDeviceAsync(device, oids, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.ErrorMessage);
    }

    [Fact]
    public async Task TestConnectionAsync_ShouldReturnFalse_WhenHostUnreachable()
    {
        // Arrange
        var device = new Device(
            id: Guid.NewGuid(),
            tenantId: Guid.NewGuid(),
            name: "Unreachable Device",
            ipAddress: "127.0.0.254",
            deviceType: DeviceType.Switch
        );

        // Act
        var result = await _service.TestConnectionAsync(device, CancellationToken.None);

        // Assert
        Assert.False(result);
    }
}