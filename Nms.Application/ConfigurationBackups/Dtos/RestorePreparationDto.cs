namespace Nms.Application.ConfigurationBackups.Dtos;

public record RestorePreparationDto
{
    public Guid BackupId { get; init; }
    public Guid DeviceId { get; init; }
    public int VersionNumber { get; init; }
    public bool IsEligibleForRestore { get; init; }
    public string ChecksumSha256 { get; init; } = string.Empty;
    public bool FileExistsOnDisk { get; init; }
    public string PreparationNotes { get; init; } = string.Empty;
    public DateTime PreparedAtUtc { get; init; }
}