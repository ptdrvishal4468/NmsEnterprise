using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Topology.Dtos;

namespace Nms.Application.Topology.Queries.GetTopologyGraph;

public class GetTopologyGraphQueryHandler : IRequestHandler<GetTopologyGraphQuery, TopologyGraphDto>
{
    private readonly ITopologyGraphBuilder _graphBuilder;

    public GetTopologyGraphQueryHandler(ITopologyGraphBuilder graphBuilder)
    {
        _graphBuilder = graphBuilder;
    }

    public async Task<TopologyGraphDto> Handle(GetTopologyGraphQuery request, CancellationToken cancellationToken)
    {
        return await _graphBuilder.BuildGraphAsync(
            request.LayerType,
            request.Status,
            rootDeviceId: null,
            maxDepth: null,
            cancellationToken);
    }
}