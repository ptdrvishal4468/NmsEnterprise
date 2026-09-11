using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Topology.Dtos;

namespace Nms.Application.Topology.Commands.DiscoverNeighbors;

public class DiscoverNeighborsCommandHandler : IRequestHandler<DiscoverNeighborsCommand, DiscoverNeighborsResultDto>
{
    private readonly INeighborDiscoveryEngine _neighborDiscoveryEngine;

    public DiscoverNeighborsCommandHandler(INeighborDiscoveryEngine neighborDiscoveryEngine)
    {
        _neighborDiscoveryEngine = neighborDiscoveryEngine;
    }

    public async Task<DiscoverNeighborsResultDto> Handle(DiscoverNeighborsCommand request, CancellationToken cancellationToken)
    {
        return await _neighborDiscoveryEngine.DiscoverNeighborsAsync(request.SpecificDeviceId, cancellationToken);
    }
}