using System.Net;
using Nms.Application.Common.Interfaces;
using Nms.Application.Topology.Dtos;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Infrastructure.Topology;

public class NeighborDiscoveryEngine : INeighborDiscoveryEngine
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INetworkInterfaceRepository _networkInterfaceRepository;
    private readonly ITenantContext _tenantContext;

    public NeighborDiscoveryEngine(
        IUnitOfWork unitOfWork,
        INetworkInterfaceRepository networkInterfaceRepository,
        ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _networkInterfaceRepository = networkInterfaceRepository;
        _tenantContext = tenantContext;
    }

    public async Task<DiscoverNeighborsResultDto> DiscoverNeighborsAsync(
        Guid? specificDeviceId = null,
        CancellationToken cancellationToken = default)
    {
        var logs = new List<string>();
        int newLinksCount = 0;
        int updatedLinksCount = 0;
        int discoveredLinksCount = 0;

        // 1. Fetch devices for discovery
        IReadOnlyList<Device> devices;
        if (specificDeviceId.HasValue && specificDeviceId.Value != Guid.Empty)
        {
            var dev = await _unitOfWork.Devices.GetByIdAsync(specificDeviceId.Value, cancellationToken);
            devices = dev != null ? new List<Device> { dev } : new List<Device>();
        }
        else
        {
            devices = await _unitOfWork.Devices.GetAllAsync(cancellationToken);
        }

        if (devices.Count == 0)
        {
            logs.Add("No devices found for neighbor discovery.");
            return new DiscoverNeighborsResultDto(0, 0, 0, 0, logs);
        }

        logs.Add($"Starting neighbor discovery across {devices.Count} device(s).");

        // 2. Fetch all tenant devices and interfaces for correlation
        var allTenantDevices = await _unitOfWork.Devices.GetAllAsync(cancellationToken);
        var deviceInterfaces = new Dictionary<Guid, IReadOnlyList<NetworkInterface>>();

        foreach (var dev in allTenantDevices)
        {
            var ifaces = await _networkInterfaceRepository.GetByDeviceIdAsync(dev.Id, cancellationToken);
            deviceInterfaces[dev.Id] = ifaces;
        }

        var scannedDeviceSet = devices.Select(d => d.Id).ToHashSet();

        // 3. Correlate L2 and L3 Adjacencies
        for (int i = 0; i < allTenantDevices.Count; i++)
        {
            var devA = allTenantDevices[i];
            if (!scannedDeviceSet.Contains(devA.Id))
                continue;

            for (int j = 0; j < allTenantDevices.Count; j++)
            {
                if (i == j)
                    continue;

                var devB = allTenantDevices[j];

                // A. Layer 2 Correlation: Interface MAC matching or Switch Port Uplinks
                if (deviceInterfaces.TryGetValue(devA.Id, out var ifacesA) &&
                    deviceInterfaces.TryGetValue(devB.Id, out var ifacesB))
                {
                    foreach (var ifA in ifacesA)
                    {
                        foreach (var ifB in ifacesB)
                        {
                            // If MAC addresses correlate or direct switch-to-device connection is indicated
                            if (!string.IsNullOrWhiteSpace(ifA.MacAddress) &&
                                !string.IsNullOrWhiteSpace(ifB.MacAddress) &&
                                string.Equals(ifA.MacAddress, ifB.MacAddress, StringComparison.OrdinalIgnoreCase))
                            {
                                var (isNew, link) = await UpsertLinkAsync(
                                    devA.Id,
                                    devB.Id,
                                    ifA.Id,
                                    ifB.Id,
                                    TopologyLayerType.Layer2,
                                    LinkDiscoveryProtocol.Lldp,
                                    Math.Max(ifA.SpeedBps, ifB.SpeedBps),
                                    "{\"discoveryMethod\":\"L2-MacMatching\"}",
                                    cancellationToken);

                                discoveredLinksCount++;
                                if (isNew) newLinksCount++; else updatedLinksCount++;
                                logs.Add($"Discovered L2 link between {devA.Name} ({ifA.Name}) and {devB.Name} ({ifB.Name}).");
                            }
                        }
                    }
                }

                // B. Layer 3 Correlation: Shared IP Subnet Adjacency
                if (IsSameSubnet(devA.IpAddress, devB.IpAddress))
                {
                    // If at least one device is a Router, Switch, or Firewall, record L3 link
                    bool isInfrastructureLink = devA.DeviceType == DeviceType.Router ||
                                                devA.DeviceType == DeviceType.Switch ||
                                                devA.DeviceType == DeviceType.Firewall ||
                                                devB.DeviceType == DeviceType.Router ||
                                                devB.DeviceType == DeviceType.Switch ||
                                                devB.DeviceType == DeviceType.Firewall;

                    if (isInfrastructureLink)
                    {
                        var (isNew, link) = await UpsertLinkAsync(
                            devA.Id,
                            devB.Id,
                            null,
                            null,
                            TopologyLayerType.Layer3,
                            LinkDiscoveryProtocol.SubnetScan,
                            1_000_000_000L, // Default 1 Gbps logical L3 link
                            "{\"discoveryMethod\":\"L3-SubnetScan\",\"sharedSubnet\":\"24\"}",
                            cancellationToken);

                        discoveredLinksCount++;
                        if (isNew) newLinksCount++; else updatedLinksCount++;
                        logs.Add($"Discovered L3 subnet adjacency between {devA.Name} ({devA.IpAddress}) and {devB.Name} ({devB.IpAddress}).");
                    }
                }
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        logs.Add($"Discovery complete: {discoveredLinksCount} total links evaluated ({newLinksCount} new, {updatedLinksCount} updated).");

        return new DiscoverNeighborsResultDto(
            devices.Count,
            discoveredLinksCount,
            newLinksCount,
            updatedLinksCount,
            logs);
    }

    private async Task<(bool IsNew, TopologyLink Link)> UpsertLinkAsync(
        Guid sourceDeviceId,
        Guid targetDeviceId,
        Guid? sourceInterfaceId,
        Guid? targetInterfaceId,
        TopologyLayerType layerType,
        LinkDiscoveryProtocol protocol,
        long speedBps,
        string metadataJson,
        CancellationToken cancellationToken)
    {
        var existingLink = await _unitOfWork.TopologyLinks.FindLinkAsync(
            sourceDeviceId,
            targetDeviceId,
            layerType,
            protocol,
            sourceInterfaceId,
            targetInterfaceId,
            cancellationToken);

        if (existingLink != null)
        {
            existingLink.TouchDiscovery(DateTime.UtcNow, speedBps, metadataJson);
            return (false, existingLink);
        }

        var newLink = new TopologyLink(
            Guid.NewGuid(),
            _tenantContext.TenantId,
            sourceDeviceId,
            targetDeviceId,
            layerType,
            protocol,
            sourceInterfaceId,
            targetInterfaceId,
            speedBps,
            TopologyLinkStatus.Active,
            DateTime.UtcNow,
            metadataJson);

        await _unitOfWork.TopologyLinks.AddAsync(newLink, cancellationToken);
        return (true, newLink);
    }

    private static bool IsSameSubnet(string ip1, string ip2)
    {
        if (!IPAddress.TryParse(ip1, out var addr1) || !IPAddress.TryParse(ip2, out var addr2))
            return false;

        var bytes1 = addr1.GetAddressBytes();
        var bytes2 = addr2.GetAddressBytes();

        if (bytes1.Length != 4 || bytes2.Length != 4)
            return false;

        // /24 subnet match (first 3 octets)
        return bytes1[0] == bytes2[0] &&
               bytes1[1] == bytes2[1] &&
               bytes1[2] == bytes2[2];
    }
}