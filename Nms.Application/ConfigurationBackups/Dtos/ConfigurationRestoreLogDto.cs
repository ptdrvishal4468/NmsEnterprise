using Nms.Domain.Enums;

namespace Nms.Application.ConfigurationBackups.Dtos;

public class ConfigurationRestoreLogDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid DeviceId { get; set; }
    public string DeviceName { get; set; } = string.Empty;
    public Guid TargetBackupId { get; set; }
    public int TargetVersionNumber { get; set; }
    public Guid? PreRestoreBackupId { get; set; }
    public RestoreStatus Status { get; set; }
    public string InitiatedBy { get; set; } = string.Empty;
    public DateTime StartTimeUtc { get; set; }
    public DateTime? EndTimeUtc { get; set; }
    public string? FailureReason { get; set; }
    public string? RollbackReason { get; set; }
    public string? AuditNotes { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}