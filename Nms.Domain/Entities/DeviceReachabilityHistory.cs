using Nms.Domain.Common;
using Nms.Domain.Enums;

namespace Nms.Domain.Entities;

public class DeviceReachabilityHistory : BaseEntity<Guid>, IMustHaveTenant
{
    public Guid DeviceId { get; private set; }
    public Guid TenantId { get; set; }
    public ReachabilityStatus Status { get; private set; }
    public double MinLatencyMs { get; private set; }
    public double MaxLatencyMs { get; private set; }
    public double AvgLatencyMs { get; private set; }
    public double CurrentLatencyMs { get; private set; }
    public int PacketsSent { get; private set; }
    public int PacketsReceived { get; private set; }
    public double PacketLossPercentage { get; private set; }
    public DateTime TimestampUtc { get; private set; }

    // EF Core private constructor
    private DeviceReachabilityHistory() { }

    public DeviceReachabilityHistory(
        Guid id,
        Guid deviceId,
        Guid tenantId,
        ReachabilityStatus status,
        double minLatencyMs,
        double maxLatencyMs,
        double avgLatencyMs,
        double currentLatencyMs,
        int packetsSent,
        int packetsReceived,
        double packetLossPercentage,
        DateTime timestampUtc) : base(id)
    {
        if (deviceId == Guid.Empty)
            throw new ArgumentException("DeviceId is required.", nameof(deviceId));

        if (tenantId == Guid.Empty)
            throw new ArgumentException("TenantId is required.", nameof(tenantId));

        DeviceId = deviceId;
        TenantId = tenantId;
        Status = status;
        MinLatencyMs = minLatencyMs;
        MaxLatencyMs = maxLatencyMs;
        AvgLatencyMs = avgLatencyMs;
        CurrentLatencyMs = currentLatencyMs;
        PacketsSent = packetsSent;
        PacketsReceived = packetsReceived;
        PacketLossPercentage = packetLossPercentage;
        TimestampUtc = timestampUtc;
    }
}