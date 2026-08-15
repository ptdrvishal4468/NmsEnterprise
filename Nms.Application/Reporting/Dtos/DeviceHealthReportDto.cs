using Nms.Domain.Enums;

namespace Nms.Application.Reporting.Dtos;

public class DeviceHealthReportDto
{
    public Guid DeviceId { get; set; }
    public string DeviceName { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public DeviceType DeviceType { get; set; }
    public DeviceStatus Status { get; set; }
    public double HealthScore { get; set; }
    public string HealthReason { get; set; } = string.Empty;
    public DateTime LastEvaluatedUtc { get; set; }
}