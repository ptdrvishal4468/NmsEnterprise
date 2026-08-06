using Nms.Domain.Enums;

namespace Nms.Application.Reachability.Dtos;

public sealed record ReachabilityHistoryDto(
    Guid Id,
    Guid DeviceId,
    ReachabilityStatus Status,
    double MinLatencyMs,
    double MaxLatencyMs,
    double AvgLatencyMs,
    double CurrentLatencyMs,
    int PacketsSent,
    int PacketsReceived,
    double PacketLossPercentage,
    DateTime TimestampUtc);