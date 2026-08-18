using Nms.Domain.Enums;

namespace Nms.Application.ThreatDetection.Dtos;

public class ThreatIndicatorDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public ThreatType ThreatType { get; set; }
    public ThreatSeverity Severity { get; set; }
    public ThreatStatus Status { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string SourceIp { get; set; } = string.Empty;
    public Guid? TargetDeviceId { get; set; }
    public string? TargetDeviceName { get; set; }
    public string TargetUser { get; set; } = string.Empty;
    public int AttemptCount { get; set; }
    public DateTime FirstDetectedAtUtc { get; set; }
    public DateTime LastDetectedAtUtc { get; set; }
    public string? ResolutionNotes { get; set; }
    public string? IndicatorMetadataJson { get; set; }
}