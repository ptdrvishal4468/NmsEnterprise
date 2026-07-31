using Nms.Application.Common.Models;
using Nms.Infrastructure.Telemetry;
using Xunit;

namespace Nms.UnitTests;

public class TelemetryEngineTests
{
    private readonly TelemetryEngine _telemetryEngine;

    public TelemetryEngineTests()
    {
        _telemetryEngine = new TelemetryEngine();
    }

    [Fact]
    public void ProcessPollResult_ShouldReturnEmptyList_WhenPollFailed()
    {
        // Arrange
        var failedResult = new SnmpPollResult
        {
            DeviceId = Guid.NewGuid(),
            IsSuccess = false,
            ErrorMessage = "Connection timed out"
        };

        // Act
        var metrics = _telemetryEngine.ProcessPollResult(failedResult);

        // Assert
        Assert.Empty(metrics);
    }

    [Fact]
    public void ProcessPollResult_ShouldExtractMetrics_WhenPollSucceeded()
    {
        // Arrange
        var deviceId = Guid.NewGuid();
        var pollResult = new SnmpPollResult
        {
            DeviceId = deviceId,
            IsSuccess = true,
            ResponseTimeMs = 45,
            OidValues = new Dictionary<string, string>
            {
                { OidConstants.CiscoCpu5Min, "35.5" },
                { OidConstants.HostMemoryUsed, "62.0" }
            }
        };

        // Act
        var metrics = _telemetryEngine.ProcessPollResult(pollResult).ToList();

        // Assert
        Assert.NotEmpty(metrics);
        var metric = metrics.First();
        Assert.Equal(deviceId, metric.DeviceId);
        Assert.Equal(45, metric.LatencyMs);
    }
}