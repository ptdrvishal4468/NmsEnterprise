using System.Security.Cryptography;
using System.Text;
using Nms.Application.Common.Interfaces;

namespace Nms.Infrastructure.ConfigurationBackups;

public class LocalConfigurationStorageService : IConfigurationStorageService
{
    private readonly string _baseStorageDirectory;

    public LocalConfigurationStorageService()
    {
        _baseStorageDirectory = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "Backups");
        if (!Directory.Exists(_baseStorageDirectory))
        {
            Directory.CreateDirectory(_baseStorageDirectory);
        }
    }

    public async Task<(string StoragePath, string ChecksumSha256, long FileSizeBytes)> SaveBackupFileAsync(
        Guid tenantId,
        Guid deviceId,
        Guid backupId,
        string rawConfiguration,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(rawConfiguration))
            rawConfiguration = string.Empty;

        var tenantDir = Path.Combine(_baseStorageDirectory, tenantId.ToString());
        var deviceDir = Path.Combine(tenantDir, deviceId.ToString());

        if (!Directory.Exists(deviceDir))
        {
            Directory.CreateDirectory(deviceDir);
        }

        var fileName = $"{backupId}.config";
        var fullPath = Path.Combine(deviceDir, fileName);

        var fileBytes = Encoding.UTF8.GetBytes(rawConfiguration);
        await File.WriteAllBytesAsync(fullPath, fileBytes, cancellationToken);

        // Calculate SHA256 Checksum
        var hashBytes = SHA256.HashData(fileBytes);
        var checksumSha256 = BitConverter.ToString(hashBytes).Replace("-", string.Empty).ToLowerInvariant();

        // Relative path for storage in DB
        var relativePath = Path.Combine("App_Data", "Backups", tenantId.ToString(), deviceId.ToString(), fileName);

        return (relativePath, checksumSha256, fileBytes.Length);
    }

    public async Task<string> GetBackupFileContentAsync(string storagePath, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(storagePath))
            throw new FileNotFoundException("Configuration backup file path is not specified.");

        // Prevent path traversal
        var normalizedPath = storagePath.Replace("..", string.Empty);
        var fullPath = Path.IsPathRooted(normalizedPath)
            ? normalizedPath
            : Path.Combine(Directory.GetCurrentDirectory(), normalizedPath);

        if (!File.Exists(fullPath))
            throw new FileNotFoundException("Configuration backup file was not found on disk.", fullPath);

        return await File.ReadAllTextAsync(fullPath, Encoding.UTF8, cancellationToken);
    }

    public Task DeleteBackupFileAsync(string storagePath, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(storagePath))
            return Task.CompletedTask;

        var normalizedPath = storagePath.Replace("..", string.Empty);
        var fullPath = Path.IsPathRooted(normalizedPath)
            ? normalizedPath
            : Path.Combine(Directory.GetCurrentDirectory(), normalizedPath);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }
}