using MediatR;
using Nms.Application.Topology.Dtos;
using Nms.Domain.Enums;

namespace Nms.Application.Topology.Queries.GetTopologyGraph;

public record GetTopologyGraphQuery(
    TopologyLayerType? LayerType = null,
    TopologyLinkStatus? Status = null) : IRequest<TopologyGraphDto>;