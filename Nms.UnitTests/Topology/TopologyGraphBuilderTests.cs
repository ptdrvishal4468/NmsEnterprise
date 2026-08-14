using Moq;
using Nms.Application.Topology.Services;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;
using Xunit;

namespace Nms.UnitTests.Topology;

public class TopologyGraphBuilderTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<INetworkInterfaceRepository> _interfaceRepoMock;
    private readonly Mock<IDeviceRepository> _deviceRepoMock;
    private readonly Mock<ITopologyLinkRepository> _topologyLinkRepoMock;
    private readonly TopologyGraphBuilder _builder;

    public TopologyGraphBuilderTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _interfaceRepoMock = new Mock<INetworkInterfaceRepository>();
        _deviceRepoMock = new Mock<IDeviceRepository>();
        _topologyLinkRepoMock = new Mock<ITopologyLinkRepository>();

        _unitOfWorkMock.Setup(u => u.Devices).Returns(_deviceRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.TopologyLinks).Returns(_topologyLinkRepoMock.Object);

        _builder = new TopologyGraphBuilder(_unitOfWorkMock.Object, _interfaceRepoMock.Object);
    }

    [Fact]
    public async Task BuildGraphAsync_ShouldReturnCompleteGraph_WhenNoRootDeviceSpecified()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var dev1 = new Device(Guid.NewGuid(), tenantId, "Router-Core", "192.168.1.1", DeviceType.Router);
        var dev2 = new Device(Guid.NewGuid(), tenantId, "Switch-Dist", "192.168.1.2", DeviceType.Switch);
        var link = new TopologyLink(Guid.NewGuid(), tenantId, dev1.Id, dev2.Id, TopologyLayerType.Layer2, LinkDiscoveryProtocol.Lldp);

        _deviceRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Device> { dev1, dev2 });

        _topologyLinkRepoMock.Setup(r => r.GetLinksAsync(null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TopologyLink> { link });

        _interfaceRepoMock.Setup(r => r.GetByDeviceIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NetworkInterface>());

        // Act
        var result = await _builder.BuildGraphAsync(null, null, null, null, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalNodes);
        Assert.Equal(1, result.TotalEdges);
        Assert.Equal(1, result.Layer2EdgesCount);
        Assert.Equal(0, result.Layer3EdgesCount);
    }

    [Fact]
    public async Task BuildGraphAsync_ShouldFilterByLayerType_WhenSpecified()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var dev1 = new Device(Guid.NewGuid(), tenantId, "Router-1", "10.0.0.1", DeviceType.Router);
        var dev2 = new Device(Guid.NewGuid(), tenantId, "Router-2", "10.0.0.2", DeviceType.Router);
        var linkL3 = new TopologyLink(Guid.NewGuid(), tenantId, dev1.Id, dev2.Id, TopologyLayerType.Layer3, LinkDiscoveryProtocol.SubnetScan);

        _deviceRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Device> { dev1, dev2 });

        _topologyLinkRepoMock.Setup(r => r.GetLinksAsync(TopologyLayerType.Layer3, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TopologyLink> { linkL3 });

        _interfaceRepoMock.Setup(r => r.GetByDeviceIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NetworkInterface>());

        // Act
        var result = await _builder.BuildGraphAsync(TopologyLayerType.Layer3, null, null, null, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalNodes);
        Assert.Equal(1, result.TotalEdges);
        Assert.Equal(1, result.Layer3EdgesCount);
    }
}