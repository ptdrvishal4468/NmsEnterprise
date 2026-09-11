using Moq;
using Nms.Application.Common.Interfaces;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;
using Nms.Infrastructure.Topology;
using Xunit;

namespace Nms.UnitTests.Topology;

public class NeighborDiscoveryEngineTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<INetworkInterfaceRepository> _interfaceRepoMock;
    private readonly Mock<ITenantContext> _tenantContextMock;
    private readonly Mock<IDeviceRepository> _deviceRepoMock;
    private readonly Mock<ITopologyLinkRepository> _topologyLinkRepoMock;
    private readonly NeighborDiscoveryEngine _engine;

    public NeighborDiscoveryEngineTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _interfaceRepoMock = new Mock<INetworkInterfaceRepository>();
        _tenantContextMock = new Mock<ITenantContext>();
        _deviceRepoMock = new Mock<IDeviceRepository>();
        _topologyLinkRepoMock = new Mock<ITopologyLinkRepository>();

        _unitOfWorkMock.Setup(u => u.Devices).Returns(_deviceRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.TopologyLinks).Returns(_topologyLinkRepoMock.Object);

        _tenantContextMock.Setup(t => t.TenantId).Returns(Guid.NewGuid());

        _engine = new NeighborDiscoveryEngine(
            _unitOfWorkMock.Object,
            _interfaceRepoMock.Object,
            _tenantContextMock.Object);
    }

    [Fact]
    public async Task DiscoverNeighborsAsync_ShouldDiscoverSubnetAdjacency_WhenDevicesAreInSameSubnet()
    {
        // Arrange
        var tenantId = _tenantContextMock.Object.TenantId;
        var dev1 = new Device(Guid.NewGuid(), tenantId, "Router-Core", "192.168.10.1", DeviceType.Router);
        var dev2 = new Device(Guid.NewGuid(), tenantId, "Switch-Dist", "192.168.10.2", DeviceType.Switch);

        _deviceRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Device> { dev1, dev2 });

        _interfaceRepoMock.Setup(r => r.GetByDeviceIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NetworkInterface>());

        _topologyLinkRepoMock.Setup(r => r.FindLinkAsync(
            It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<TopologyLayerType>(), It.IsAny<LinkDiscoveryProtocol>(),
            It.IsAny<Guid?>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TopologyLink?)null);

        // Act
        var result = await _engine.DiscoverNeighborsAsync(null, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.DevicesScanned);
        Assert.True(result.DiscoveredLinksCount > 0);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}