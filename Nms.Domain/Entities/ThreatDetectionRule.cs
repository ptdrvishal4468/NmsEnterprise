using Nms.Domain.Common;
using Nms.Domain.Enums;

namespace Nms.Domain.Entities;

public class ThreatDetectionRule : AuditableEntity<Guid>, IMustHaveTenant
{
    public ThreatDetectionRule() : base(Guid.NewGuid()) { }
    public ThreatDetectionRule(Guid id) : base(id) { }

    public Guid TenantId { get; set; }
    public string RuleName { get; set; } = string.Empty;
    public ThreatType ThreatType { get; set; }
    public ThreatSeverity DefaultSeverity { get; set; }
    public int FailureThreshold { get; set; } = 5;
    public int TimeWindowMinutes { get; set; } = 15;
    public bool IsEnabled { get; set; } = true;
    public string? Description { get; set; }
}