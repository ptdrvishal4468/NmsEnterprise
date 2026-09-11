namespace Nms.Application.Common.Interfaces;

public interface IConfigurationStorageService
{
    Task<(string StoragePath, string ChecksumSha256, long FileSizeBytes)> SaveBackupFileAsync(
        Guid tenantId,
        Guid deviceId,
        Guid backupId,
        string rawConfiguration,
        CancellationToken cancellationToken);

    Task<string> GetBackupFileContentAsync(
        string storagePath,
        CancellationToken cancellationToken);

    Task DeleteBackupFileAsync(
        string storagePath,
        CancellationToken cancellationToken);
}