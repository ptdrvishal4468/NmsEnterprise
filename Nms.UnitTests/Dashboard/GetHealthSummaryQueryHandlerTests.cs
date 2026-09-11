using System.Linq.Expressions;
using Moq;
using Nms.Application.Dashboard.Queries.GetHealthSummary;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;
using Xunit;

namespace Nms.UnitTests.Dashboard;

public class GetHealthSummaryQueryHandlerTests
{
    private readonly Mock<IDeviceRepository> _deviceRepoMock = new();
    private readonly Mock<IDeviceHealthHistoryRepository> _healthRepoMock = new();
    private readonly Guid _tenantId = Guid.NewGuid();

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Handle_ShouldComputeHealthSummary_WithHealthHistories()
    {
        var d1 = new Device(Guid.NewGuid(), _tenantId, "Core-1", "10.0.0.1", DeviceType.Switch);
        var d2 = new Device(Guid.NewGuid(), _tenantId, "Core-2", "10.0.0.2", DeviceType.Switch);
        var d3 = new Device(Guid.NewGuid(), _tenantId, "Edge-1", "10.0.0.3", DeviceType.Router);

        _deviceRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Device> { d1, d2, d3 });

        var h1 = new DeviceHealthHistory(Guid.NewGuid(), _tenantId, d1.Id, 95.0, DeviceStatus.Online, "Nominal", DateTime.UtcNow);
        var h2 = new DeviceHealthHistory(Guid.NewGuid(), _tenantId, d2.Id, 65.0, DeviceStatus.Degraded, "Interface errors", DateTime.UtcNow);
        var h3 = new DeviceHealthHistory(Guid.NewGuid(), _tenantId, d3.Id, 30.0, DeviceStatus.Unreachable, "ICMP packet loss", DateTime.UtcNow);

        _healthRepoMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<DeviceHealthHistory, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DeviceHealthHistory> { h1, h2, h3 });

        var handler = new GetHealthSummaryQueryHandler(_deviceRepoMock.Object, _healthRepoMock.Object);
        var result = await handler.Handle(new GetHealthSummaryQuery(TopDegradedLimit: 5), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(3, result.TotalMonitoredDevices);
        Assert.Equal(1, result.HealthyDevicesCount);
        Assert.Equal(1, result.WarningDevicesCount);
        Assert.Equal(1, result.CriticalDevicesCount);
        Assert.Equal(63.33, result.AverageHealthScore);
        Assert.Equal(2, result.TopDegradedDevices.Count);
        Assert.Equal(d3.Id, result.TopDegradedDevices[0].DeviceId); // Lowest health score first
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Handle_ShouldFallbackToDeviceStatus_WhenNoHealthHistoryExists()
    {
        var d1 = new Device(Guid.NewGuid(), _tenantId, "Core-1", "10.0.0.1", DeviceType.Switch);
        d1.UpdateStatus(DeviceStatus.Online);
        var d2 = new Device(Guid.NewGuid(), _tenantId, "Edge-1", "10.0.0.2", DeviceType.Router);
        d2.UpdateStatus(DeviceStatus.Degraded);

        _deviceRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Device> { d1, d2 });

        _healthRepoMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<DeviceHealthHistory, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DeviceHealthHistory>());

        var handler = new GetHealthSummaryQueryHandler(_deviceRepoMock.Object, _healthRepoMock.Object);
        var result = await handler.Handle(new GetHealthSummaryQuery(), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(2, result.TotalMonitoredDevices);
        Assert.Equal(1, result.HealthyDevicesCount);
        Assert.Equal(1, result.WarningDevicesCount);
        Assert.Equal(80.0, result.AverageHealthScore); // (100 + 60) / 2
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Handle_ShouldReturnEmptySummary_WhenNoDevicesExist()
    {
        _deviceRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Device>());

        var handler = new GetHealthSummaryQueryHandler(_deviceRepoMock.Object, _healthRepoMock.Object);
        var result = await handler.Handle(new GetHealthSummaryQuery(), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(0, result.TotalMonitoredDevices);
        Assert.Equal(100.0, result.AverageHealthScore);
        Assert.Empty(result.TopDegradedDevices);
    }
}