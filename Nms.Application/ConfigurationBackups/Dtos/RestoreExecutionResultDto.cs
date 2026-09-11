using Nms.Domain.Enums;

namespace Nms.Application.ConfigurationBackups.Dtos;

public class RestoreExecutionResultDto
{
    public Guid RestoreLogId { get; set; }
    public Guid TenantId { get; set; }
    public Guid DeviceId { get; set; }
    public Guid TargetBackupId { get; set; }
    public Guid? PreRestoreBackupId { get; set; }
    public RestoreStatus Status { get; set; }
    public bool IsSuccess => Status == RestoreStatus.Success;
    public string InitiatedBy { get; set; } = string.Empty;
    public DateTime StartTimeUtc { get; set; }
    public DateTime? EndTimeUtc { get; set; }
    public string? FailureReason { get; set; }
    public string? RollbackReason { get; set; }
    public string? AuditNotes { get; set; }
}