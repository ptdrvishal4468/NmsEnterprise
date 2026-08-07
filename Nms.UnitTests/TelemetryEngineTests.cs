using System;
using System.Collections.Generic;
using System.Linq;
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
        var tenantId = Guid.NewGuid();
        var failedResult = new SnmpPollResult
        {
            DeviceId = Guid.NewGuid(),
            IsSuccess = false,
            ErrorMessage = "Connection timed out"
        };

        // Act
        var metrics = _telemetryEngine.ProcessPollResult(tenantId, failedResult);

        // Assert
        Assert.Empty(metrics);
    }

    [Fact]
    public void ProcessPollResult_ShouldExtractMetrics_WhenPollSucceeded()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var deviceId = Guid.NewGuid();
        var pollResult = new SnmpPollResult
        {
            DeviceId = deviceId,
            IsSuccess = true,
            ResponseTimeMs = 45,
            OidValues = new Dictionary<string, string>
            {
                { OidConstants.CiscoCpu5Min, "35.5" },
                { OidConstants.HostMemoryUsed, "62.0" },
                { OidConstants.DiskUtilization, "45.0" },
                { OidConstants.CiscoEnvMonTemperature, "38.0" },
                { OidConstants.CiscoEnvMonFanStatus, "1" },
                { OidConstants.CiscoEnvMonSupplyStatus, "1" }
            }
        };

        // Act
        var metrics = _telemetryEngine.ProcessPollResult(tenantId, pollResult).ToList();

        // Assert
        Assert.NotEmpty(metrics);
        var metric = metrics.First();
        Assert.Equal(tenantId, metric.TenantId);
        Assert.Equal(deviceId, metric.DeviceId);
        Assert.Equal(35.5m, metric.CpuUtilization);
        Assert.Equal(62.0m, metric.RamUtilization);
        Assert.Equal(45.0m, metric.DiskUtilization);
        Assert.Equal(38.0m, metric.Temperature);
        Assert.Equal(1, metric.FanStatus);
        Assert.Equal(1, metric.PowerSupplyStatus);
        Assert.Equal(45, metric.LatencyMs);
    }
}