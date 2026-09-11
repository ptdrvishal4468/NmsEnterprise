using System.Linq.Expressions;
using Moq;
using Nms.Application.Common.Interfaces;
using Nms.Application.Dashboard.Queries.GetTenantDashboard;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;
using Xunit;

namespace Nms.UnitTests.Dashboard;

public class GetTenantDashboardQueryHandlerTests
{
    private readonly Mock<ITenantRepository> _tenantRepoMock = new();
    private readonly Mock<IDeviceRepository> _deviceRepoMock = new();
    private readonly Mock<IAlertRepository> _alertRepoMock = new();
    private readonly Mock<IDeviceHealthHistoryRepository> _healthRepoMock = new();
    private readonly Mock<ITopologyLinkRepository> _topologyRepoMock = new();
    private readonly Mock<ITenantContext> _tenantContextMock = new();
    private readonly Guid _tenantId = Guid.NewGuid();

    public GetTenantDashboardQueryHandlerTests()
    {
        _tenantContextMock.Setup(t => t.TenantId).Returns(_tenantId);
        _tenantContextMock.Setup(t => t.IsResolved).Returns(true);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Handle_ShouldAggregateTenantDashboardCorrectly()
    {
        var tenant = new Tenant(_tenantId, "Acme Corp");
        _tenantRepoMock.Setup(r => r.GetByIdAsync(_tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tenant);

        var d1 = new Device(Guid.NewGuid(), _tenantId, "Core-1", "10.0.0.1", DeviceType.Switch);
        d1.UpdateStatus(DeviceStatus.Online);
        var d2 = new Device(Guid.NewGuid(), _tenantId, "Core-2", "10.0.0.2", DeviceType.Switch);
        d2.UpdateStatus(DeviceStatus.Offline);

        _deviceRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Device> { d1, d2 });

        var alert = new Alert(_tenantId, Guid.NewGuid(), d2.Id, MetricType.DiskUsage, AlertSeverity.Critical, 95m, 90m, "Disk full");
        _alertRepoMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Alert, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Alert> { alert });

        var history = new DeviceHealthHistory(Guid.NewGuid(), _tenantId, d1.Id, 100.0, DeviceStatus.Online, "Healthy", DateTime.UtcNow);
        _healthRepoMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<DeviceHealthHistory, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DeviceHealthHistory> { history });

        var link = new TopologyLink(Guid.NewGuid(), _tenantId, d1.Id, d2.Id, TopologyLayerType.Layer2, LinkDiscoveryProtocol.Lldp);
        _topologyRepoMock.Setup(r => r.GetLinksAsync(null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TopologyLink> { link });

        var handler = new GetTenantDashboardQueryHandler(
            _tenantRepoMock.Object,
            _deviceRepoMock.Object,
            _alertRepoMock.Object,
            _healthRepoMock.Object,
            _topologyRepoMock.Object,
            _tenantContextMock.Object);

        var result = await handler.Handle(new GetTenantDashboardQuery(), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(_tenantId, result.TenantId);
        Assert.Equal("Acme Corp", result.TenantName);
        Assert.True(result.IsActive);
        Assert.Equal(2, result.TotalDevices);
        Assert.Equal(1, result.OnlineDevices);
        Assert.Equal(1, result.OfflineDevices);
        Assert.Equal(1, result.ActiveAlerts);
        Assert.Equal(100.0, result.AverageHealthScore);
        Assert.Equal(1, result.TotalTopologyLinks);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Handle_ShouldThrowKeyNotFoundException_WhenTenantDoesNotExist()
    {
        _tenantRepoMock.Setup(r => r.GetByIdAsync(_tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tenant?)null);

        var handler = new GetTenantDashboardQueryHandler(
            _tenantRepoMock.Object,
            _deviceRepoMock.Object,
            _alertRepoMock.Object,
            _healthRepoMock.Object,
            _topologyRepoMock.Object,
            _tenantContextMock.Object);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new GetTenantDashboardQuery(), CancellationToken.None));
    }
}