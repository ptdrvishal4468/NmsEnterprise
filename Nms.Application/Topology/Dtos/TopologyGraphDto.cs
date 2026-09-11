namespace Nms.Application.Topology.Dtos;

public record TopologyGraphDto(
    IReadOnlyList<TopologyNodeDto> Nodes,
    IReadOnlyList<TopologyEdgeDto> Edges,
    int TotalNodes,
    int TotalEdges,
    int Layer2EdgesCount,
    int Layer3EdgesCount);