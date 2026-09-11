using Nms.Domain.Enums;

namespace Nms.Application.Topology.Dtos;

public record TopologyLinkDto(
    Guid Id,
    Guid SourceDeviceId,
    string? SourceDeviceName,
    Guid? SourceInterfaceId,
    string? SourceInterfaceName,
    Guid TargetDeviceId,
    string? TargetDeviceName,
    Guid? TargetInterfaceId,
    string? TargetInterfaceName,
    TopologyLayerType LayerType,
    LinkDiscoveryProtocol Protocol,
    TopologyLinkStatus Status,
    long SpeedBps,
    DateTime LastDiscoveredUtc,
    string? MetadataJson,
    DateTime CreatedAtUtc,
    string? CreatedBy);