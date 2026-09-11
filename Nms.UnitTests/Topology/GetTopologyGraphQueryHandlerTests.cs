using Moq;
using Nms.Application.Common.Interfaces;
using Nms.Application.Topology.Dtos;
using Nms.Application.Topology.Queries.GetTopologyGraph;
using Nms.Domain.Enums;
using Xunit;

namespace Nms.UnitTests.Topology;

public class GetTopologyGraphQueryHandlerTests
{
    private readonly Mock<ITopologyGraphBuilder> _builderMock;
    private readonly GetTopologyGraphQueryHandler _handler;

    public GetTopologyGraphQueryHandlerTests()
    {
        _builderMock = new Mock<ITopologyGraphBuilder>();
        _handler = new GetTopologyGraphQueryHandler(_builderMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldDelegateToTopologyGraphBuilder_AndReturnGraph()
    {
        // Arrange
        var expectedGraph = new TopologyGraphDto(
            Nodes: new List<TopologyNodeDto>(),
            Edges: new List<TopologyEdgeDto>(),
            TotalNodes: 0,
            TotalEdges: 0,
            Layer2EdgesCount: 0,
            Layer3EdgesCount: 0);

        _builderMock
            .Setup(b => b.BuildGraphAsync(
                TopologyLayerType.Layer2,
                TopologyLinkStatus.Active,
                null,
                null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedGraph);

        var query = new GetTopologyGraphQuery(TopologyLayerType.Layer2, TopologyLinkStatus.Active);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedGraph.TotalNodes, result.TotalNodes);
        Assert.Equal(expectedGraph.TotalEdges, result.TotalEdges);
        _builderMock.Verify(b => b.BuildGraphAsync(
            TopologyLayerType.Layer2,
            TopologyLinkStatus.Active,
            null,
            null,
            It.IsAny<CancellationToken>()), Times.Once);
    }
}