using Nms.Domain.Common;
using Nms.Domain.Enums;

namespace Nms.Domain.Entities;

public class ConfigurationRestoreLog : AuditableEntity<Guid>, IMustHaveTenant
{
    public Guid TenantId { get; set; }

    public Guid DeviceId { get; private set; }
    public Device? Device { get; private set; }

    public Guid TargetBackupId { get; private set; }
    public ConfigurationBackup? TargetBackup { get; private set; }

    public Guid? PreRestoreBackupId { get; private set; }
    public ConfigurationBackup? PreRestoreBackup { get; private set; }

    public RestoreStatus Status { get; private set; } = RestoreStatus.Pending;
    public string InitiatedBy { get; private set; } = string.Empty;

    public DateTime StartTimeUtc { get; private set; }
    public DateTime? EndTimeUtc { get; private set; }

    public string? FailureReason { get; private set; }
    public string? RollbackReason { get; private set; }
    public string? AuditNotes { get; private set; }

    // Private constructor for EF Core
    private ConfigurationRestoreLog() { }

    public ConfigurationRestoreLog(
        Guid id,
        Guid tenantId,
        Guid deviceId,
        Guid targetBackupId,
        string initiatedBy) : base(id)
    {
        if (tenantId == Guid.Empty)
            throw new ArgumentException("Tenant ID is required.", nameof(tenantId));

        if (deviceId == Guid.Empty)
            throw new ArgumentException("Device ID is required.", nameof(deviceId));

        if (targetBackupId == Guid.Empty)
            throw new ArgumentException("Target Backup ID is required.", nameof(targetBackupId));

        if (string.IsNullOrWhiteSpace(initiatedBy))
            throw new ArgumentException("Initiator identity is required.", nameof(initiatedBy));

        TenantId = tenantId;
        DeviceId = deviceId;
        TargetBackupId = targetBackupId;
        InitiatedBy = initiatedBy.Trim();
        Status = RestoreStatus.Pending;
        StartTimeUtc = DateTime.UtcNow;
    }

    public void SetPreRestoreBackup(Guid preRestoreBackupId)
    {
        if (preRestoreBackupId == Guid.Empty)
            throw new ArgumentException("Pre-restore backup ID cannot be empty.", nameof(preRestoreBackupId));

        PreRestoreBackupId = preRestoreBackupId;
    }

    public void MarkInProgress()
    {
        Status = RestoreStatus.InProgress;
    }

    public void MarkSuccess(string? notes = null)
    {
        Status = RestoreStatus.Success;
        EndTimeUtc = DateTime.UtcNow;
        FailureReason = null;
        AuditNotes = notes;
    }

    public void MarkFailed(string failureReason, string? notes = null)
    {
        Status = RestoreStatus.Failed;
        EndTimeUtc = DateTime.UtcNow;
        FailureReason = failureReason;
        AuditNotes = notes;
    }

    public void MarkRolledBack(string rollbackReason, string? notes = null)
    {
        Status = RestoreStatus.RolledBack;
        EndTimeUtc = DateTime.UtcNow;
        RollbackReason = rollbackReason;
        AuditNotes = notes;
    }

    public void MarkRollbackFailed(string failureReason, string rollbackReason, string? notes = null)
    {
        Status = RestoreStatus.RollbackFailed;
        EndTimeUtc = DateTime.UtcNow;
        FailureReason = failureReason;
        RollbackReason = rollbackReason;
        AuditNotes = notes;
    }
}