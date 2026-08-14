using Moq;
using Nms.Application.Common.Interfaces;
using Nms.Application.Topology.Commands.CreateTopologyLink;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;
using Xunit;

namespace Nms.UnitTests.Topology;

public class CreateTopologyLinkCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ITenantContext> _tenantContextMock;
    private readonly Mock<INetworkInterfaceRepository> _interfaceRepoMock;
    private readonly Mock<IDeviceRepository> _deviceRepoMock;
    private readonly Mock<ITopologyLinkRepository> _topologyLinkRepoMock;
    private readonly CreateTopologyLinkCommandHandler _handler;

    public CreateTopologyLinkCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _tenantContextMock = new Mock<ITenantContext>();
        _interfaceRepoMock = new Mock<INetworkInterfaceRepository>();
        _deviceRepoMock = new Mock<IDeviceRepository>();
        _topologyLinkRepoMock = new Mock<ITopologyLinkRepository>();

        _unitOfWorkMock.Setup(u => u.Devices).Returns(_deviceRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.TopologyLinks).Returns(_topologyLinkRepoMock.Object);

        _tenantContextMock.Setup(t => t.TenantId).Returns(Guid.NewGuid());

        _handler = new CreateTopologyLinkCommandHandler(
            _unitOfWorkMock.Object,
            _tenantContextMock.Object,
            _interfaceRepoMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateNewLink_WhenValidRequestProvided()
    {
        // Arrange
        var tenantId = _tenantContextMock.Object.TenantId;
        var dev1 = new Device(Guid.NewGuid(), tenantId, "Device-A", "10.1.1.1", DeviceType.Switch);
        var dev2 = new Device(Guid.NewGuid(), tenantId, "Device-B", "10.1.1.2", DeviceType.Switch);

        _deviceRepoMock.Setup(r => r.GetByIdAsync(dev1.Id, It.IsAny<CancellationToken>())).ReturnsAsync(dev1);
        _deviceRepoMock.Setup(r => r.GetByIdAsync(dev2.Id, It.IsAny<CancellationToken>())).ReturnsAsync(dev2);

        _topologyLinkRepoMock.Setup(r => r.FindLinkAsync(
            dev1.Id, dev2.Id, TopologyLayerType.Layer2, LinkDiscoveryProtocol.Manual, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TopologyLink?)null);

        var command = new CreateTopologyLinkCommand(
            dev1.Id,
            dev2.Id,
            TopologyLayerType.Layer2,
            LinkDiscoveryProtocol.Manual,
            null,
            null,
            1_000_000_000L);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dev1.Id, result.SourceDeviceId);
        Assert.Equal(dev2.Id, result.TargetDeviceId);
        Assert.Equal(TopologyLayerType.Layer2, result.LayerType);
        _topologyLinkRepoMock.Verify(r => r.AddAsync(It.IsAny<TopologyLink>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowKeyNotFoundException_WhenSourceDeviceNotFound()
    {
        // Arrange
        var sourceId = Guid.NewGuid();
        var targetId = Guid.NewGuid();

        _deviceRepoMock.Setup(r => r.GetByIdAsync(sourceId, It.IsAny<CancellationToken>())).ReturnsAsync((Device?)null);

        var command = new CreateTopologyLinkCommand(sourceId, targetId, TopologyLayerType.Layer2);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }
}