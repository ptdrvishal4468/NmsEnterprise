using Nms.Domain.Enums;

namespace Nms.Application.ThreatDetection.Dtos;

public class CreateThreatRuleDto
{
    public string RuleName { get; set; } = string.Empty;
    public ThreatType ThreatType { get; set; }
    public ThreatSeverity DefaultSeverity { get; set; }
    public int FailureThreshold { get; set; } = 5;
    public int TimeWindowMinutes { get; set; } = 15;
    public bool IsEnabled { get; set; } = true;
    public string? Description { get; set; }
}