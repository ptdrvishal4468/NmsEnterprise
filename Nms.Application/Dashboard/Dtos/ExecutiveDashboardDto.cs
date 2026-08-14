namespace Nms.Application.Dashboard.Dtos;

public sealed record ExecutiveDashboardDto
{
    public int TotalDevices { get; init; }
    public int OnlineDevices { get; init; }
    public int OfflineDevices { get; init; }
    public int DegradedDevices { get; init; }
    public int UnreachableDevices { get; init; }
    public int UnknownDevices { get; init; }
    public int TotalActiveAlerts { get; init; }
    public int CriticalAlerts { get; init; }
    public int WarningAlerts { get; init; }
    public double AverageHealthScore { get; init; }
    public int ActiveTopologyLinks { get; init; }
    public int RecentEventsCount { get; init; }
    public DateTime GeneratedAtUtc { get; init; } = DateTime.UtcNow;
}