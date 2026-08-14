using MediatR;
using Nms.Application.Topology.Dtos;
using Nms.Domain.Enums;

namespace Nms.Application.Topology.Commands.CreateTopologyLink;

public record CreateTopologyLinkCommand(
    Guid SourceDeviceId,
    Guid TargetDeviceId,
    TopologyLayerType LayerType,
    LinkDiscoveryProtocol Protocol = LinkDiscoveryProtocol.Manual,
    Guid? SourceInterfaceId = null,
    Guid? TargetInterfaceId = null,
    long SpeedBps = 0,
    string? MetadataJson = null) : IRequest<TopologyLinkDto>;