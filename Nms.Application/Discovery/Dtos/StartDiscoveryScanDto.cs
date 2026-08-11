namespace Nms.Application.Discovery.Dtos;

public record StartDiscoveryScanDto(
    string Name,
    string IpRange,
    string? SnmpCommunity = "public",
    int SnmpPort = 161);