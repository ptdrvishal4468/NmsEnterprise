using Nms.Domain.Enums;

namespace Nms.Domain.Models;

public sealed class PingResult
{
    public ReachabilityStatus Status { get; }
    public double MinLatencyMs { get; }
    public double MaxLatencyMs { get; }
    public double AvgLatencyMs { get; }
    public double CurrentLatencyMs { get; }
    public int PacketsSent { get; }
    public int PacketsReceived { get; }
    public double PacketLossPercentage { get; }

    public PingResult(
        ReachabilityStatus status,
        double minLatencyMs,
        double maxLatencyMs,
        double avgLatencyMs,
        double currentLatencyMs,
        int packetsSent,
        int packetsReceived,
        double packetLossPercentage)
    {
        Status = status;
        MinLatencyMs = minLatencyMs;
        MaxLatencyMs = maxLatencyMs;
        AvgLatencyMs = avgLatencyMs;
        CurrentLatencyMs = currentLatencyMs;
        PacketsSent = packetsSent;
        PacketsReceived = packetsReceived;
        PacketLossPercentage = packetLossPercentage;
    }

    public static PingResult CreateFailure(ReachabilityStatus status, int packetsSent)
    {
        return new PingResult(
            status: status,
            minLatencyMs: 0,
            maxLatencyMs: 0,
            avgLatencyMs: 0,
            currentLatencyMs: 0,
            packetsSent: packetsSent,
            packetsReceived: 0,
            packetLossPercentage: 100.0);
    }
}