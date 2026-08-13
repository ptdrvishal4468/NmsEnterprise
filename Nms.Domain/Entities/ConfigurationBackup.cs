using Nms.Domain.Common;
using Nms.Domain.Enums;

namespace Nms.Domain.Entities;

public class ConfigurationBackup : AuditableEntity<Guid>, IMustHaveTenant
{
    public Guid TenantId { get; set; }
    public Guid DeviceId { get; private set; }
    public Device? Device { get; private set; }

    public int VersionNumber { get; private set; }
    public string StoragePath { get; private set; } = string.Empty;
    public string ChecksumSha256 { get; private set; } = string.Empty;
    public long FileSizeBytes { get; private set; }

    public BackupStatus Status { get; private set; } = BackupStatus.Pending;
    public BackupTriggerType TriggerType { get; private set; } = BackupTriggerType.Manual;

    public string? FailureReason { get; private set; }
    public DateTime TimestampUtc { get; private set; }

    // Restore Preparation Metadata (Phase 35 Scope)
    public bool IsEligibleForRestore { get; private set; }
    public string? RestorePreparationNotes { get; private set; }

    // Private constructor for EF Core
    private ConfigurationBackup() { }

    public ConfigurationBackup(
        Guid id,
        Guid tenantId,
        Guid deviceId,
        int versionNumber,
        BackupTriggerType triggerType) : base(id)
    {
        if (tenantId == Guid.Empty)
            throw new ArgumentException("Tenant ID is required.", nameof(tenantId));

        if (deviceId == Guid.Empty)
            throw new ArgumentException("Device ID is required.", nameof(deviceId));

        if (versionNumber <= 0)
            throw new ArgumentException("Version number must be positive.", nameof(versionNumber));

        TenantId = tenantId;
        DeviceId = deviceId;
        VersionNumber = versionNumber;
        TriggerType = triggerType;
        Status = BackupStatus.Pending;
        TimestampUtc = DateTime.UtcNow;
        IsEligibleForRestore = false;
    }

    public void MarkInProgress()
    {
        Status = BackupStatus.InProgress;
    }

    public void MarkSuccess(string storagePath, string checksumSha256, long fileSizeBytes)
    {
        if (string.IsNullOrWhiteSpace(storagePath))
            throw new ArgumentException("Storage path is required.", nameof(storagePath));

        if (string.IsNullOrWhiteSpace(checksumSha256))
            throw new ArgumentException("SHA256 checksum is required.", nameof(checksumSha256));

        Status = BackupStatus.Success;
        StoragePath = storagePath;
        ChecksumSha256 = checksumSha256;
        FileSizeBytes = fileSizeBytes;
        FailureReason = null;
        TimestampUtc = DateTime.UtcNow;
    }

    public void MarkFailed(string failureReason)
    {
        Status = BackupStatus.Failed;
        FailureReason = failureReason;
        TimestampUtc = DateTime.UtcNow;
        IsEligibleForRestore = false;
    }

    public void SetRestorePreparation(bool isEligible, string? notes)
    {
        if (Status != BackupStatus.Success && isEligible)
            throw new InvalidOperationException("Only successful backups can be marked eligible for restore.");

        IsEligibleForRestore = isEligible;
        RestorePreparationNotes = notes;
    }
}