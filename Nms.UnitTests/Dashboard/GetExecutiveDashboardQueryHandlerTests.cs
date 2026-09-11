using System.Linq.Expressions;
using Moq;
using Nms.Application.Common.Interfaces;
using Nms.Application.Dashboard.Queries.GetExecutiveDashboard;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;
using Xunit;

namespace Nms.UnitTests.Dashboard;

public class GetExecutiveDashboardQueryHandlerTests
{
    private readonly Mock<IDeviceRepository> _deviceRepoMock = new();
    private readonly Mock<IAlertRepository> _alertRepoMock = new();
    private readonly Mock<IDeviceHealthHistoryRepository> _healthHistoryRepoMock = new();
    private readonly Mock<ITopologyLinkRepository> _topologyRepoMock = new();
    private readonly Mock<IEventRepository> _eventRepoMock = new();
    private readonly Mock<ITenantContext> _tenantContextMock = new();
    private readonly Guid _tenantId = Guid.NewGuid();

    public GetExecutiveDashboardQueryHandlerTests()
    {
        _tenantContextMock.Setup(t => t.TenantId).Returns(_tenantId);
        _tenantContextMock.Setup(t => t.IsResolved).Returns(true);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Handle_ShouldAggregateExecutiveMetricsCorrectly()
    {
        var device1 = new Device(Guid.NewGuid(), _tenantId, "Core-Router", "10.0.0.1", DeviceType.Router);
        device1.UpdateStatus(DeviceStatus.Online);

        var device2 = new Device(Guid.NewGuid(), _tenantId, "Edge-Switch", "10.0.0.2", DeviceType.Switch);
        device2.UpdateStatus(DeviceStatus.Offline);

        var device3 = new Device(Guid.NewGuid(), _tenantId, "Firewall-Primary", "10.0.0.3", DeviceType.Firewall);
        device3.UpdateStatus(DeviceStatus.Degraded);

        _deviceRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Device> { device1, device2, device3 });

        var ruleId = Guid.NewGuid();
        var alert1 = new Alert(_tenantId, ruleId, device1.Id, MetricType.CpuUsage, AlertSeverity.Critical, 95m, 80m, "High CPU");
        var alert2 = new Alert(_tenantId, ruleId, device2.Id, MetricType.MemoryUsage, AlertSeverity.Warning, 85m, 80m, "High RAM");

        _alertRepoMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Alert, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Alert> { alert1, alert2 });

        var history1 = new DeviceHealthHistory(Guid.NewGuid(), _tenantId, device1.Id, 95.0, DeviceStatus.Online, "Healthy", DateTime.UtcNow);
        var history2 = new DeviceHealthHistory(Guid.NewGuid(), _tenantId, device3.Id, 60.0, DeviceStatus.Degraded, "High load", DateTime.UtcNow);

        _healthHistoryRepoMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<DeviceHealthHistory, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DeviceHealthHistory> { history1, history2 });

        var link = new TopologyLink(Guid.NewGuid(), _tenantId, device1.Id, device2.Id, TopologyLayerType.Layer2, LinkDiscoveryProtocol.Lldp);
        _topologyRepoMock.Setup(r => r.GetLinksAsync(It.IsAny<TopologyLayerType?>(), TopologyLinkStatus.Active, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TopologyLink> { link });

        _eventRepoMock.Setup(r => r.GetEventsPagedAsync(
                _tenantId, 1, 1, null, null, null, null, It.IsAny<DateTime?>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<DeviceEvent>(), 42));

        var handler = new GetExecutiveDashboardQueryHandler(
            _deviceRepoMock.Object,
            _alertRepoMock.Object,
            _healthHistoryRepoMock.Object,
            _topologyRepoMock.Object,
            _eventRepoMock.Object,
            _tenantContextMock.Object);

        var result = await handler.Handle(new GetExecutiveDashboardQuery(), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(3, result.TotalDevices);
        Assert.Equal(1, result.OnlineDevices);
        Assert.Equal(1, result.OfflineDevices);
        Assert.Equal(1, result.DegradedDevices);
        Assert.Equal(2, result.TotalActiveAlerts);
        Assert.Equal(1, result.CriticalAlerts);
        Assert.Equal(1, result.WarningAlerts);
        Assert.Equal(77.5, result.AverageHealthScore); // (95 + 60) / 2
        Assert.Equal(1, result.ActiveTopologyLinks);
        Assert.Equal(42, result.RecentEventsCount);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Handle_ShouldReturnDefaultMetrics_WhenDataIsEmpty()
    {
        _deviceRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Device>());

        _alertRepoMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Alert, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Alert>());

        _healthHistoryRepoMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<DeviceHealthHistory, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DeviceHealthHistory>());

        _topologyRepoMock.Setup(r => r.GetLinksAsync(It.IsAny<TopologyLayerType?>(), TopologyLinkStatus.Active, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TopologyLink>());

        _eventRepoMock.Setup(r => r.GetEventsPagedAsync(
                _tenantId, 1, 1, null, null, null, null, It.IsAny<DateTime?>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<DeviceEvent>(), 0));

        var handler = new GetExecutiveDashboardQueryHandler(
            _deviceRepoMock.Object,
            _alertRepoMock.Object,
            _healthHistoryRepoMock.Object,
            _topologyRepoMock.Object,
            _eventRepoMock.Object,
            _tenantContextMock.Object);

        var result = await handler.Handle(new GetExecutiveDashboardQuery(), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(0, result.TotalDevices);
        Assert.Equal(0, result.TotalActiveAlerts);
        Assert.Equal(100.0, result.AverageHealthScore);
        Assert.Equal(0, result.ActiveTopologyLinks);
        Assert.Equal(0, result.RecentEventsCount);
    }
}