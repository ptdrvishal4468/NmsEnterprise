using Nms.Domain.Common;
using Nms.Domain.Enums;

namespace Nms.Domain.Entities;

public class TopologyLink : AuditableEntity<Guid>, IMustHaveTenant
{
    public Guid TenantId { get; set; }

    public Guid SourceDeviceId { get; private set; }
    public Device? SourceDevice { get; private set; }

    public Guid? SourceInterfaceId { get; private set; }
    public NetworkInterface? SourceInterface { get; private set; }

    public Guid TargetDeviceId { get; private set; }
    public Device? TargetDevice { get; private set; }

    public Guid? TargetInterfaceId { get; private set; }
    public NetworkInterface? TargetInterface { get; private set; }

    public TopologyLayerType LayerType { get; private set; } = TopologyLayerType.Layer2;
    public LinkDiscoveryProtocol Protocol { get; private set; } = LinkDiscoveryProtocol.Manual;
    public TopologyLinkStatus Status { get; private set; } = TopologyLinkStatus.Active;
    public long SpeedBps { get; private set; }
    public DateTime LastDiscoveredUtc { get; private set; }
    public string? MetadataJson { get; private set; }

    // EF Core private constructor
    private TopologyLink() { }

    public TopologyLink(
        Guid id,
        Guid tenantId,
        Guid sourceDeviceId,
        Guid targetDeviceId,
        TopologyLayerType layerType,
        LinkDiscoveryProtocol protocol,
        Guid? sourceInterfaceId = null,
        Guid? targetInterfaceId = null,
        long speedBps = 0,
        TopologyLinkStatus status = TopologyLinkStatus.Active,
        DateTime? lastDiscoveredUtc = null,
        string? metadataJson = null) : base(id)
    {
        if (sourceDeviceId == Guid.Empty)
            throw new ArgumentException("Source device ID is required.", nameof(sourceDeviceId));

        if (targetDeviceId == Guid.Empty)
            throw new ArgumentException("Target device ID is required.", nameof(targetDeviceId));

        TenantId = tenantId;
        SourceDeviceId = sourceDeviceId;
        TargetDeviceId = targetDeviceId;
        SourceInterfaceId = sourceInterfaceId;
        TargetInterfaceId = targetInterfaceId;
        LayerType = layerType;
        Protocol = protocol;
        SpeedBps = speedBps >= 0 ? speedBps : 0;
        Status = status;
        LastDiscoveredUtc = lastDiscoveredUtc ?? DateTime.UtcNow;
        MetadataJson = metadataJson;
    }

    public void UpdateStatus(TopologyLinkStatus newStatus)
    {
        Status = newStatus;
    }

    public void TouchDiscovery(DateTime? discoveredUtc = null, long? speedBps = null, string? metadataJson = null)
    {
        LastDiscoveredUtc = discoveredUtc ?? DateTime.UtcNow;
        Status = TopologyLinkStatus.Active;
        if (speedBps.HasValue && speedBps.Value >= 0)
        {
            SpeedBps = speedBps.Value;
        }
        if (metadataJson != null)
        {
            MetadataJson = metadataJson;
        }
    }

    public void UpdateLinkDetails(
        Guid? sourceInterfaceId,
        Guid? targetInterfaceId,
        TopologyLayerType layerType,
        LinkDiscoveryProtocol protocol,
        long speedBps,
        string? metadataJson)
    {
        SourceInterfaceId = sourceInterfaceId;
        TargetInterfaceId = targetInterfaceId;
        LayerType = layerType;
        Protocol = protocol;
        SpeedBps = speedBps >= 0 ? speedBps : 0;
        MetadataJson = metadataJson;
        LastDiscoveredUtc = DateTime.UtcNow;
    }
}