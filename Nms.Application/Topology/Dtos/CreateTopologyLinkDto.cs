using Nms.Domain.Enums;

namespace Nms.Application.Topology.Dtos;

public record CreateTopologyLinkDto(
    Guid SourceDeviceId,
    Guid TargetDeviceId,
    TopologyLayerType LayerType,
    LinkDiscoveryProtocol Protocol = LinkDiscoveryProtocol.Manual,
    Guid? SourceInterfaceId = null,
    Guid? TargetInterfaceId = null,
    long SpeedBps = 0,
    string? MetadataJson = null);