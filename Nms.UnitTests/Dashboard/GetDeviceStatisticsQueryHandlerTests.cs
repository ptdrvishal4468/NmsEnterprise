using Moq;
using Nms.Application.Dashboard.Queries.GetDeviceStatistics;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;
using Xunit;

namespace Nms.UnitTests.Dashboard;

public class GetDeviceStatisticsQueryHandlerTests
{
    private readonly Mock<IDeviceRepository> _deviceRepoMock = new();
    private readonly Guid _tenantId = Guid.NewGuid();

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Handle_ShouldCalculateDistributionsCorrectly_WithVariousDevices()
    {
        var d1 = new Device(Guid.NewGuid(), _tenantId, "SW-1", "10.0.0.1", DeviceType.Switch, vendor: "Cisco", site: "HQ-Floor1");
        d1.UpdateStatus(DeviceStatus.Online);

        var d2 = new Device(Guid.NewGuid(), _tenantId, "SW-2", "10.0.0.2", DeviceType.Switch, vendor: "Cisco", site: "HQ-Floor2");
        d2.UpdateStatus(DeviceStatus.Online);

        var d3 = new Device(Guid.NewGuid(), _tenantId, "RT-1", "10.0.0.3", DeviceType.Router, vendor: "Juniper", site: null);
        d3.UpdateStatus(DeviceStatus.Degraded);

        var d4 = new Device(Guid.NewGuid(), _tenantId, "SRV-1", "10.0.0.4", DeviceType.LinuxServer, vendor: null, site: "HQ-Floor1");
        d4.UpdateStatus(DeviceStatus.Offline);

        _deviceRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Device> { d1, d2, d3, d4 });

        var handler = new GetDeviceStatisticsQueryHandler(_deviceRepoMock.Object);
        var result = await handler.Handle(new GetDeviceStatisticsQuery(), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(4, result.TotalDevices);

        // Status
        Assert.Equal(2, result.StatusDistribution[DeviceStatus.Online.ToString()]);
        Assert.Equal(1, result.StatusDistribution[DeviceStatus.Degraded.ToString()]);
        Assert.Equal(1, result.StatusDistribution[DeviceStatus.Offline.ToString()]);

        // Type
        Assert.Equal(2, result.TypeDistribution[DeviceType.Switch.ToString()]);
        Assert.Equal(1, result.TypeDistribution[DeviceType.Router.ToString()]);
        Assert.Equal(1, result.TypeDistribution[DeviceType.LinuxServer.ToString()]);

        // Vendor
        Assert.Equal(2, result.VendorDistribution["Cisco"]);
        Assert.Equal(1, result.VendorDistribution["Juniper"]);
        Assert.Equal(1, result.VendorDistribution["Unassigned"]);

        // Site
        Assert.Equal(2, result.SiteDistribution["HQ-Floor1"]);
        Assert.Equal(1, result.SiteDistribution["HQ-Floor2"]);
        Assert.Equal(1, result.SiteDistribution["Unassigned"]);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Handle_ShouldReturnEmptyDistributions_WhenNoDevicesExist()
    {
        _deviceRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Device>());

        var handler = new GetDeviceStatisticsQueryHandler(_deviceRepoMock.Object);
        var result = await handler.Handle(new GetDeviceStatisticsQuery(), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(0, result.TotalDevices);
        Assert.Empty(result.StatusDistribution);
        Assert.Empty(result.TypeDistribution);
        Assert.Empty(result.VendorDistribution);
        Assert.Empty(result.SiteDistribution);
    }
}