using Nms.Domain.Enums;

namespace Nms.Application.ThreatDetection.Dtos;

public class ConfigurationDriftRecordDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid DeviceId { get; set; }
    public string DeviceName { get; set; } = string.Empty;
    public Guid BaselineBackupId { get; set; }
    public Guid CurrentBackupId { get; set; }
    public bool HasDrift { get; set; }
    public int AddedLinesCount { get; set; }
    public int RemovedLinesCount { get; set; }
    public int ModifiedLinesCount { get; set; }
    public string? DifferencesJson { get; set; }
    public DateTime DetectedAtUtc { get; set; }
    public ThreatSeverity Severity { get; set; }
    public bool IsAcknowledged { get; set; }
    public string? AcknowledgmentNotes { get; set; }
}