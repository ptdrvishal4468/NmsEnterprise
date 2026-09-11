using Nms.Domain.Common;
using Nms.Domain.Enums;

namespace Nms.Domain.Entities;

public class ThreatIndicator : AuditableEntity<Guid>, IMustHaveTenant
{
    public ThreatIndicator() : base(Guid.NewGuid()) { }
    public ThreatIndicator(Guid id) : base(id) { }

    public Guid TenantId { get; set; }
    public ThreatType ThreatType { get; set; }
    public ThreatSeverity Severity { get; set; }
    public ThreatStatus Status { get; set; } = ThreatStatus.Active;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string SourceIp { get; set; } = string.Empty;
    public Guid? TargetDeviceId { get; set; }
    public Device? TargetDevice { get; set; }
    public string TargetUser { get; set; } = string.Empty;
    public int AttemptCount { get; set; } = 1;
    public DateTime FirstDetectedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime LastDetectedAtUtc { get; set; } = DateTime.UtcNow;
    public string? ResolutionNotes { get; set; }
    public string? IndicatorMetadataJson { get; set; }
}