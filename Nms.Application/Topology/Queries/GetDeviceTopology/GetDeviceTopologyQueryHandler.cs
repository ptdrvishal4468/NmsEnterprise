using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Topology.Dtos;

namespace Nms.Application.Topology.Queries.GetDeviceTopology;

public class GetDeviceTopologyQueryHandler : IRequestHandler<GetDeviceTopologyQuery, TopologyGraphDto>
{
    private readonly ITopologyGraphBuilder _graphBuilder;

    public GetDeviceTopologyQueryHandler(ITopologyGraphBuilder graphBuilder)
    {
        _graphBuilder = graphBuilder;
    }

    public async Task<TopologyGraphDto> Handle(GetDeviceTopologyQuery request, CancellationToken cancellationToken)
    {
        return await _graphBuilder.BuildGraphAsync(
            request.LayerType,
            request.Status,
            rootDeviceId: request.DeviceId,
            maxDepth: request.Depth,
            cancellationToken);
    }
}