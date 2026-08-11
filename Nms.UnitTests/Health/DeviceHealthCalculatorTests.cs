using Nms.Application.Health.Services;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Xunit;

namespace Nms.UnitTests.Health;

public class DeviceHealthCalculatorTests
{
    private readonly DeviceHealthCalculator _calculator;

    public DeviceHealthCalculatorTests()
    {
        _calculator = new DeviceHealthCalculator();
    }

    [Fact]
    public void CalculateHealth_WhenDeviceIsHealthy_ReturnsMaxScoreAndOnlineStatus()
    {
        // Arrange
        var device = CreateTestDevice();
        var reachability = new DeviceReachabilityHistory(
            Guid.NewGuid(), device.Id, device.TenantId, ReachabilityStatus.Online,
            minLatencyMs: 10, maxLatencyMs: 30, avgLatencyMs: 20, currentLatencyMs: 20,
            packetsSent: 4, packetsReceived: 4, packetLossPercentage: 0.0,
            DateTime.UtcNow);

        var metric = new DeviceMetricRaw(
            device.TenantId, device.Id,
            cpuUtilization: 30m, ramUtilization: 40m, diskUtilization: 20m,
            interfaceUtilization: 10m, temperature: 35m, fanStatus: 1, powerSupplyStatus: 1, latencyMs: 20);

        var interfaces = new List<NetworkInterface>();

        // Act
        var result = _calculator.CalculateHealth(device, reachability, metric, interfaces);

        // Assert
        Assert.Equal(100.0, result.HealthScore);
        Assert.Equal(DeviceStatus.Online, result.Status);
        Assert.Contains("normal operational parameters", result.SummaryReason);
    }

    [Fact]
    public void CalculateHealth_WhenDeviceIsUnreachable_ReturnsZeroScoreAndUnreachableStatus()
    {
        // Arrange
        var device = CreateTestDevice();
        var reachability = new DeviceReachabilityHistory(
            Guid.NewGuid(), device.Id, device.TenantId, ReachabilityStatus.Unreachable,
            minLatencyMs: 0, maxLatencyMs: 0, avgLatencyMs: 0, currentLatencyMs: 0,
            packetsSent: 4, packetsReceived: 0, packetLossPercentage: 100.0,
            DateTime.UtcNow);

        var interfaces = new List<NetworkInterface>();

        // Act
        var result = _calculator.CalculateHealth(device, reachability, null, interfaces);

        // Assert
        Assert.Equal(0.0, result.HealthScore);
        Assert.Equal(DeviceStatus.Unreachable, result.Status);
        Assert.Contains("100% packet loss", result.SummaryReason);
    }

    [Fact]
    public void CalculateHealth_WhenHighCpuAndMemory_DeductsTelemetryScore()
    {
        // Arrange
        var device = CreateTestDevice();
        var reachability = new DeviceReachabilityHistory(
            Guid.NewGuid(), device.Id, device.TenantId, ReachabilityStatus.Online,
            minLatencyMs: 10, maxLatencyMs: 30, avgLatencyMs: 20, currentLatencyMs: 20,
            packetsSent: 4, packetsReceived: 4, packetLossPercentage: 0.0,
            DateTime.UtcNow);

        var metric = new DeviceMetricRaw(
            device.TenantId, device.Id,
            cpuUtilization: 95m, ramUtilization: 95m, diskUtilization: 50m,
            interfaceUtilization: 10m, temperature: 40m, fanStatus: 1, powerSupplyStatus: 1, latencyMs: 20);

        var interfaces = new List<NetworkInterface>();

        // Act
        var result = _calculator.CalculateHealth(device, reachability, metric, interfaces);

        // Assert
        Assert.True(result.HealthScore < 80.0);
        Assert.Equal(DeviceStatus.Degraded, result.Status);
        Assert.Contains("Critical CPU usage", result.SummaryReason);
        Assert.Contains("Critical memory usage", result.SummaryReason);
    }

    [Fact]
    public void CalculateHealth_WhenNoReachabilityData_ReturnsDegradedBaseline()
    {
        // Arrange
        var device = CreateTestDevice();
        var interfaces = new List<NetworkInterface>();

        // Act
        var result = _calculator.CalculateHealth(device, null, null, interfaces);

        // Assert
        Assert.True(result.HealthScore < 80.0);
        Assert.Contains("No reachability data available", result.SummaryReason);
    }

    private static Device CreateTestDevice()
    {
        return new Device(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Core-Switch-01",
            "192.168.1.1",
            DeviceType.Switch);
    }
}