using Nms.Domain.Entities;
using Nms.Domain.Enums;

namespace Nms.Domain.Interfaces;

public interface ITopologyLinkRepository : IGenericRepository<TopologyLink, Guid>
{
    Task<IReadOnlyList<TopologyLink>> GetLinksAsync(
        TopologyLayerType? layerType = null,
        TopologyLinkStatus? status = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TopologyLink>> GetDeviceLinksAsync(
        Guid deviceId,
        TopologyLayerType? layerType = null,
        CancellationToken cancellationToken = default);

    Task<TopologyLink?> FindLinkAsync(
        Guid sourceDeviceId,
        Guid targetDeviceId,
        TopologyLayerType layerType,
        LinkDiscoveryProtocol protocol,
        Guid? sourceInterfaceId = null,
        Guid? targetInterfaceId = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TopologyLink>> GetStaleLinksAsync(
        DateTime olderThanUtc,
        CancellationToken cancellationToken = default);
}