using Nms.Domain.Enums;

namespace Nms.Application.ThreatDetection.Dtos;

public class ThreatDetectionRuleDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string RuleName { get; set; } = string.Empty;
    public ThreatType ThreatType { get; set; }
    public ThreatSeverity DefaultSeverity { get; set; }
    public int FailureThreshold { get; set; }
    public int TimeWindowMinutes { get; set; }
    public bool IsEnabled { get; set; }
    public string? Description { get; set; }
}