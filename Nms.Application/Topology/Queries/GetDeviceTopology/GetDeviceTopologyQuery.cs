using MediatR;
using Nms.Application.Topology.Dtos;
using Nms.Domain.Enums;

namespace Nms.Application.Topology.Queries.GetDeviceTopology;

public record GetDeviceTopologyQuery(
    Guid DeviceId,
    int Depth = 2,
    TopologyLayerType? LayerType = null,
    TopologyLinkStatus? Status = null) : IRequest<TopologyGraphDto>;