using Nms.Domain.Enums;

namespace Nms.Application.Reporting.Dtos;

public class AlertReportDto
{
    public Guid AlertId { get; set; }
    public Guid DeviceId { get; set; }
    public string DeviceName { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public MetricType MetricType { get; set; }
    public AlertSeverity Severity { get; set; }
    public AlertState State { get; set; }
    public decimal MetricValue { get; set; }
    public decimal ThresholdValue { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime TriggeredAtUtc { get; set; }
    public DateTime? ResolvedAtUtc { get; set; }
    public string? AcknowledgedBy { get; set; }
    public string? SuppressedBy { get; set; }
}