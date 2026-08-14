using Nms.Domain.Enums;

namespace Nms.Application.Topology.Dtos;

public record TopologyNodeDto(
    Guid Id,
    string Name,
    string IpAddress,
    string? Hostname,
    string? Vendor,
    string? Model,
    DeviceType DeviceType,
    DeviceStatus Status,
    string? Site,
    string? Location,
    int InterfaceCount);