using Nms.Domain.Common;
using Nms.Domain.Enums;

namespace Nms.Domain.Entities;

public class ConfigurationDriftRecord : AuditableEntity<Guid>, IMustHaveTenant
{
    public ConfigurationDriftRecord() : base(Guid.NewGuid()) { }
    public ConfigurationDriftRecord(Guid id) : base(id) { }

    public Guid TenantId { get; set; }
    public Guid DeviceId { get; set; }
    public Device Device { get; set; } = null!;
    public Guid BaselineBackupId { get; set; }
    public ConfigurationBackup BaselineBackup { get; set; } = null!;
    public Guid CurrentBackupId { get; set; }
    public ConfigurationBackup CurrentBackup { get; set; } = null!;
    public bool HasDrift { get; set; }
    public int AddedLinesCount { get; set; }
    public int RemovedLinesCount { get; set; }
    public int ModifiedLinesCount { get; set; }
    public string? DifferencesJson { get; set; }
    public DateTime DetectedAtUtc { get; set; } = DateTime.UtcNow;
    public ThreatSeverity Severity { get; set; } = ThreatSeverity.Medium;
    public bool IsAcknowledged { get; set; }
    public string? AcknowledgmentNotes { get; set; }
}