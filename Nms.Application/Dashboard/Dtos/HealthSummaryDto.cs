namespace Nms.Application.Dashboard.Dtos;

public sealed record DegradedDeviceHealthDto(
    Guid DeviceId,
    string DeviceName,
    string IpAddress,
    double HealthScore,
    string Status,
    string Reason,
    DateTime TimestampUtc);

public sealed record HealthSummaryDto
{
    public int TotalMonitoredDevices { get; init; }
    public double AverageHealthScore { get; init; }
    public int HealthyDevicesCount { get; init; }   // Score >= 80.0
    public int WarningDevicesCount { get; init; }   // Score >= 50.0 and < 80.0
    public int CriticalDevicesCount { get; init; }  // Score < 50.0
    public IReadOnlyList<DegradedDeviceHealthDto> TopDegradedDevices { get; init; } = Array.Empty<DegradedDeviceHealthDto>();
    public DateTime GeneratedAtUtc { get; init; } = DateTime.UtcNow;
}