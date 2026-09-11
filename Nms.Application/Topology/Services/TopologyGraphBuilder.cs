using Nms.Application.Common.Interfaces;
using Nms.Application.Topology.Dtos;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Application.Topology.Services;

public class TopologyGraphBuilder : ITopologyGraphBuilder
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INetworkInterfaceRepository _networkInterfaceRepository;

    public TopologyGraphBuilder(
        IUnitOfWork unitOfWork,
        INetworkInterfaceRepository networkInterfaceRepository)
    {
        _unitOfWork = unitOfWork;
        _networkInterfaceRepository = networkInterfaceRepository;
    }

    public async Task<TopologyGraphDto> BuildGraphAsync(
        TopologyLayerType? layerType = null,
        TopologyLinkStatus? status = null,
        Guid? rootDeviceId = null,
        int? maxDepth = null,
        CancellationToken cancellationToken = default)
    {
        // 1. Retrieve all matching topology links
        var allLinks = await _unitOfWork.TopologyLinks.GetLinksAsync(layerType, status, cancellationToken);

        // 2. Retrieve device inventory
        var allDevices = await _unitOfWork.Devices.GetAllAsync(cancellationToken);
        var deviceDict = allDevices.ToDictionary(d => d.Id);

        // 3. Subgraph traversal if rootDeviceId is specified
        IReadOnlyList<TopologyLink> filteredLinks;
        HashSet<Guid> includedDeviceIds;

        if (rootDeviceId.HasValue && rootDeviceId.Value != Guid.Empty && deviceDict.ContainsKey(rootDeviceId.Value))
        {
            var (reachableDeviceIds, scopedLinks) = TraverseSubgraph(rootDeviceId.Value, allLinks, maxDepth ?? 2);
            includedDeviceIds = reachableDeviceIds;
            filteredLinks = scopedLinks;
        }
        else
        {
            filteredLinks = allLinks;
            includedDeviceIds = allDevices.Select(d => d.Id).ToHashSet();
        }

        // 4. Build Node DTOs
        var nodes = new List<TopologyNodeDto>();
        foreach (var devId in includedDeviceIds)
        {
            if (deviceDict.TryGetValue(devId, out var device))
            {
                var interfaces = await _networkInterfaceRepository.GetByDeviceIdAsync(devId, cancellationToken);
                nodes.Add(new TopologyNodeDto(
                    device.Id,
                    device.Name,
                    device.IpAddress,
                    device.Hostname,
                    device.Vendor,
                    device.Model,
                    device.DeviceType,
                    device.Status,
                    device.Site,
                    device.Location,
                    interfaces.Count));
            }
        }

        // 5. Build Edge DTOs
        var edges = new List<TopologyEdgeDto>();
        var interfaceCache = new Dictionary<Guid, NetworkInterface>();

        foreach (var link in filteredLinks)
        {
            if (!deviceDict.ContainsKey(link.SourceDeviceId) || !deviceDict.ContainsKey(link.TargetDeviceId))
                continue;

            string sourceDevName = deviceDict[link.SourceDeviceId].Name;
            string targetDevName = deviceDict[link.TargetDeviceId].Name;
            string? sourceIfName = null;
            string? targetIfName = null;

            if (link.SourceInterfaceId.HasValue)
            {
                if (!interfaceCache.TryGetValue(link.SourceInterfaceId.Value, out var srcIf))
                {
                    srcIf = await _networkInterfaceRepository.GetByIdAsync(link.SourceInterfaceId.Value, cancellationToken);
                    if (srcIf != null)
                    {
                        interfaceCache[link.SourceInterfaceId.Value] = srcIf;
                    }
                }
                sourceIfName = srcIf?.Name;
            }

            if (link.TargetInterfaceId.HasValue)
            {
                if (!interfaceCache.TryGetValue(link.TargetInterfaceId.Value, out var tgtIf))
                {
                    tgtIf = await _networkInterfaceRepository.GetByIdAsync(link.TargetInterfaceId.Value, cancellationToken);
                    if (tgtIf != null)
                    {
                        interfaceCache[link.TargetInterfaceId.Value] = tgtIf;
                    }
                }
                targetIfName = tgtIf?.Name;
            }

            edges.Add(new TopologyEdgeDto(
                link.Id,
                link.SourceDeviceId,
                sourceDevName,
                link.SourceInterfaceId,
                sourceIfName,
                link.TargetDeviceId,
                targetDevName,
                link.TargetInterfaceId,
                targetIfName,
                link.LayerType,
                link.Protocol,
                link.Status,
                link.SpeedBps,
                link.LastDiscoveredUtc,
                link.MetadataJson));
        }

        int l2Count = edges.Count(e => e.LayerType == TopologyLayerType.Layer2 || e.LayerType == TopologyLayerType.Both);
        int l3Count = edges.Count(e => e.LayerType == TopologyLayerType.Layer3 || e.LayerType == TopologyLayerType.Both);

        return new TopologyGraphDto(
            nodes,
            edges,
            nodes.Count,
            edges.Count,
            l2Count,
            l3Count);
    }

    private static (HashSet<Guid> ReachableDeviceIds, List<TopologyLink> ScopedLinks) TraverseSubgraph(
        Guid rootDeviceId,
        IReadOnlyList<TopologyLink> allLinks,
        int maxDepth)
    {
        var visited = new HashSet<Guid> { rootDeviceId };
        var queue = new Queue<(Guid DeviceId, int CurrentDepth)>();
        queue.Enqueue((rootDeviceId, 0));

        var scopedLinks = new List<TopologyLink>();
        var linkSet = new HashSet<Guid>();

        var adjacency = new Dictionary<Guid, List<(Guid NeighborId, TopologyLink Link)>>();
        foreach (var link in allLinks)
        {
            if (!adjacency.ContainsKey(link.SourceDeviceId))
                adjacency[link.SourceDeviceId] = new();
            if (!adjacency.ContainsKey(link.TargetDeviceId))
                adjacency[link.TargetDeviceId] = new();

            adjacency[link.SourceDeviceId].Add((link.TargetDeviceId, link));
            adjacency[link.TargetDeviceId].Add((link.SourceDeviceId, link));
        }

        while (queue.Count > 0)
        {
            var (currentDevice, currentDepth) = queue.Dequeue();
            if (currentDepth >= maxDepth)
                continue;

            if (adjacency.TryGetValue(currentDevice, out var neighbors))
            {
                foreach (var (neighborId, link) in neighbors)
                {
                    if (linkSet.Add(link.Id))
                    {
                        scopedLinks.Add(link);
                    }

                    if (visited.Add(neighborId))
                    {
                        queue.Enqueue((neighborId, currentDepth + 1));
                    }
                }
            }
        }

        return (visited, scopedLinks);
    }
}