using Nms.Domain.Enums;

namespace Nms.Application.ConfigurationBackups.Dtos;

public record ConfigurationBackupDto
{
    public Guid Id { get; init; }
    public Guid TenantId { get; init; }
    public Guid DeviceId { get; init; }
    public string DeviceName { get; init; } = string.Empty;
    public string DeviceIpAddress { get; init; } = string.Empty;

    public int VersionNumber { get; init; }
    public string ChecksumSha256 { get; init; } = string.Empty;
    public long FileSizeBytes { get; init; }

    public BackupStatus Status { get; init; }
    public BackupTriggerType TriggerType { get; init; }

    public string? FailureReason { get; init; }
    public DateTime TimestampUtc { get; init; }

    public bool IsEligibleForRestore { get; init; }
    public string? RestorePreparationNotes { get; init; }
}