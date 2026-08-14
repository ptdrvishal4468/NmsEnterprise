namespace Nms.Application.Dashboard.Dtos;

public sealed record TenantDashboardDto
{
    public Guid TenantId { get; init; }
    public string TenantName { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public int TotalDevices { get; init; }
    public int OnlineDevices { get; init; }
    public int OfflineDevices { get; init; }
    public int ActiveAlerts { get; init; }
    public double AverageHealthScore { get; init; }
    public int TotalTopologyLinks { get; init; }
    public DateTime GeneratedAtUtc { get; init; } = DateTime.UtcNow;
}