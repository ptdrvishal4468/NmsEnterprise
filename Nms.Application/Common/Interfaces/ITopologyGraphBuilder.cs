using Nms.Application.Topology.Dtos;
using Nms.Domain.Enums;

namespace Nms.Application.Common.Interfaces;

public interface ITopologyGraphBuilder
{
    Task<TopologyGraphDto> BuildGraphAsync(
        TopologyLayerType? layerType = null,
        TopologyLinkStatus? status = null,
        Guid? rootDeviceId = null,
        int? maxDepth = null,
        CancellationToken cancellationToken = default);
}