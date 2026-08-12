using Nms.Application.Notifications.Services;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Xunit;

namespace Nms.UnitTests.Notifications;

public class NotificationTemplateEngineTests
{
    private readonly NotificationTemplateEngine _engine = new();

    [Fact]
    public void Render_ShouldReplaceAllAlertAndDevicePlaceholders()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var alertRuleId = Guid.NewGuid();
        var deviceId = Guid.NewGuid();

        var alert = new Alert(
            tenantId,
            alertRuleId,
            deviceId,
            MetricType.CpuUsage,
            AlertSeverity.Critical,
            88.5m,
            80.0m,
            "High CPU Utilization Detected");

        var device = new Device(
            id: deviceId,
            tenantId: tenantId,
            name: "Core-Router-01",
            ipAddress: "192.168.1.1",
            deviceType: DeviceType.Router,
            snmpPort: 161,
            vendor: "Cisco",
            model: "ISR4451");

        string template = "ALERT [{AlertSeverity}] - {MetricType} on {DeviceName} ({DeviceIp}): Value {MetricValue} breached threshold {ThresholdValue}. Message: {Message}";

        // Act
        string rendered = _engine.Render(template, alert, device);

        // Assert
        Assert.Contains("ALERT [Critical]", rendered);
        Assert.Contains("CpuUsage", rendered);
        Assert.Contains("Core-Router-01", rendered);
        Assert.Contains("192.168.1.1", rendered);
        Assert.Contains("88.50", rendered);
        Assert.Contains("80.00", rendered);
        Assert.Contains("High CPU Utilization Detected", rendered);
    }

    [Fact]
    public void Render_ShouldFallbackToDeviceId_WhenDeviceIsNull()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var alertRuleId = Guid.NewGuid();
        var deviceId = Guid.NewGuid();

        var alert = new Alert(
            tenantId,
            alertRuleId,
            deviceId,
            MetricType.MemoryUsage,
            AlertSeverity.Warning,
            75.0m,
            70.0m,
            "Memory warning");

        string template = "Device: {DeviceName}, IP: {DeviceIp}";

        // Act
        string rendered = _engine.Render(template, alert, device: null);

        // Assert
        Assert.Contains(deviceId.ToString(), rendered);
        Assert.Contains("N/A", rendered);
    }
}