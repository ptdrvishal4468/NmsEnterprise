using Nms.Application.Topology.Dtos;

namespace Nms.Application.Common.Interfaces;

public interface INeighborDiscoveryEngine
{
    Task<DiscoverNeighborsResultDto> DiscoverNeighborsAsync(
        Guid? specificDeviceId = null,
        CancellationToken cancellationToken = default);
}