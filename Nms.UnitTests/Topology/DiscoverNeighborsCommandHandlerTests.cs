using Moq;
using Nms.Application.Common.Interfaces;
using Nms.Application.Topology.Commands.DiscoverNeighbors;
using Nms.Application.Topology.Dtos;
using Xunit;

namespace Nms.UnitTests.Topology;

public class DiscoverNeighborsCommandHandlerTests
{
    private readonly Mock<INeighborDiscoveryEngine> _engineMock;
    private readonly DiscoverNeighborsCommandHandler _handler;

    public DiscoverNeighborsCommandHandlerTests()
    {
        _engineMock = new Mock<INeighborDiscoveryEngine>();
        _handler = new DiscoverNeighborsCommandHandler(_engineMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldDelegateToNeighborDiscoveryEngine_AndReturnResult()
    {
        // Arrange
        var deviceId = Guid.NewGuid();
        var expectedResult = new DiscoverNeighborsResultDto(
            DevicesScanned: 1,
            DiscoveredLinksCount: 2,
            NewLinksCount: 1,
            UpdatedLinksCount: 1,
            DiscoveryLogs: new List<string> { "Discovery scan completed successfully." });

        _engineMock
            .Setup(e => e.DiscoverNeighborsAsync(deviceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var command = new DiscoverNeighborsCommand(deviceId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResult.DevicesScanned, result.DevicesScanned);
        Assert.Equal(expectedResult.DiscoveredLinksCount, result.DiscoveredLinksCount);
        Assert.Equal(expectedResult.NewLinksCount, result.NewLinksCount);
        Assert.Equal(expectedResult.UpdatedLinksCount, result.UpdatedLinksCount);
        _engineMock.Verify(e => e.DiscoverNeighborsAsync(deviceId, It.IsAny<CancellationToken>()), Times.Once);
    }
}