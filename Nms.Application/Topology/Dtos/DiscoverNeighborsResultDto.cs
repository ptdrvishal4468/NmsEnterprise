namespace Nms.Application.Topology.Dtos;

public record DiscoverNeighborsResultDto(
    int DevicesScanned,
    int DiscoveredLinksCount,
    int NewLinksCount,
    int UpdatedLinksCount,
    IReadOnlyList<string> DiscoveryLogs);