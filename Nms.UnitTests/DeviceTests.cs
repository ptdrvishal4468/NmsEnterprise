using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Xunit;

namespace Nms.UnitTests;

public class DeviceTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldInstantiateDevice()
    {
        // Arrange
        var id = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var name = "Core-Switch-01";
        var ip = "10.0.0.1";

        // Act
        var device = new Device(
            id, tenantId, name, ip, DeviceType.Switch, 161,
            hostname: "sw01.internal", vendor: "Cisco", model: "C9300",
            serialNumber: "SN12345", firmwareVersion: "17.03",
            macAddress: "00:1A:2B:3C:4D:5E", site: "DC1", location: "Rack 01");

        // Assert
        Assert.Equal(id, device.Id);
        Assert.Equal(tenantId, device.TenantId);
        Assert.Equal(name, device.Name);
        Assert.Equal(ip, device.IpAddress);
        Assert.Equal(DeviceType.Switch, device.DeviceType);
        Assert.Equal("Cisco", device.Vendor);
        Assert.Equal("C9300", device.Model);
        Assert.Equal("SN12345", device.SerialNumber);
        Assert.Equal(DeviceStatus.Unknown, device.Status);
    }

    [Fact]
    public void Constructor_WithEmptyName_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Device(Guid.NewGuid(), Guid.NewGuid(), "", "10.0.0.1", DeviceType.Router));
    }

    [Fact]
    public void Constructor_WithEmptyIpAddress_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Device(Guid.NewGuid(), Guid.NewGuid(), "Core-Router", "", DeviceType.Router));
    }

    [Fact]
    public void UpdateInventory_WithValidData_ShouldUpdateProperties()
    {
        // Arrange
        var device = new Device(Guid.NewGuid(), Guid.NewGuid(), "OldName", "10.0.0.1", DeviceType.Router);

        // Act
        device.UpdateInventory(
            "NewName", "10.0.0.2", DeviceType.Firewall, 161,
            "fw01", "Palo Alto", "PA-3220", "SN9999", "10.1",
            "00-11-22-33-44-55", "SiteB", "Rack 02");

        // Assert
        Assert.Equal("NewName", device.Name);
        Assert.Equal("10.0.0.2", device.IpAddress);
        Assert.Equal(DeviceType.Firewall, device.DeviceType);
        Assert.Equal("Palo Alto", device.Vendor);
    }

    [Fact]
    public void UpdateStatus_ToOnline_ShouldSetLastSeenUtc()
    {
        // Arrange
        var device = new Device(Guid.NewGuid(), Guid.NewGuid(), "Core-Router", "10.0.0.1", DeviceType.Router);

        // Act
        device.UpdateStatus(DeviceStatus.Online);

        // Assert
        Assert.Equal(DeviceStatus.Online, device.Status);
        Assert.NotNull(device.LastSeenUtc);
    }
}