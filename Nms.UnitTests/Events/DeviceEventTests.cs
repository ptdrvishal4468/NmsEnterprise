using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Xunit;

namespace Nms.UnitTests.Events;

public class DeviceEventTests
{
    [Fact]
    public void Constructor_WithValidArguments_ShouldCreateDeviceEvent()
    {
        // Arrange
        var id = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var category = EventCategory.Device;
        var severity = EventSeverity.Warning;
        var source = "PollingEngine";
        var message = "Device response time degraded above threshold.";

        // Act
        var deviceEvent = new DeviceEvent(
            id: id,
            tenantId: tenantId,
            category: category,
            severity: severity,
            source: source,
            message: message);

        // Assert
        Assert.Equal(id, deviceEvent.Id);
        Assert.Equal(tenantId, deviceEvent.TenantId);
        Assert.Equal(category, deviceEvent.Category);
        Assert.Equal(severity, deviceEvent.Severity);
        Assert.Equal(source, deviceEvent.Source);
        Assert.Equal(message, deviceEvent.Message);
    }

    [Fact]
    public void Constructor_WithNullOrWhiteSpaceSource_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new DeviceEvent(
                id: Guid.NewGuid(),
                tenantId: Guid.NewGuid(),
                category: EventCategory.System,
                severity: EventSeverity.Informational,
                source: "",
                message: "Valid message"));
    }

    [Fact]
    public void Constructor_WithNullOrWhiteSpaceMessage_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new DeviceEvent(
                id: Guid.NewGuid(),
                tenantId: Guid.NewGuid(),
                category: EventCategory.System,
                severity: EventSeverity.Informational,
                source: "ValidSource",
                message: " "));
    }
}