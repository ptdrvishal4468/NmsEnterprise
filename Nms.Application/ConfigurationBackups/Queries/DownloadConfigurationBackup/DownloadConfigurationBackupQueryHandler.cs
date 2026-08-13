using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.ConfigurationBackups.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.ConfigurationBackups.Queries.DownloadConfigurationBackup;

public class DownloadConfigurationBackupQueryHandler : IRequestHandler<DownloadConfigurationBackupQuery, ConfigurationDownloadDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfigurationStorageService _storageService;

    public DownloadConfigurationBackupQueryHandler(
        IUnitOfWork unitOfWork,
        IConfigurationStorageService storageService)
    {
        _unitOfWork = unitOfWork;
        _storageService = storageService;
    }

    public async Task<ConfigurationDownloadDto> Handle(DownloadConfigurationBackupQuery request, CancellationToken cancellationToken)
    {
        var backup = await _unitOfWork.ConfigurationBackups.GetByIdAsync(request.BackupId, cancellationToken);
        if (backup == null || backup.TenantId != request.TenantId)
        {
            throw new KeyNotFoundException($"Configuration backup '{request.BackupId}' was not found for tenant '{request.TenantId}'.");
        }

        var rawContent = await _storageService.GetBackupFileContentAsync(backup.StoragePath, cancellationToken);

        var deviceName = string.IsNullOrWhiteSpace(backup.Device?.Name) ? "device" : backup.Device.Name.Replace(" ", "_");
        var fileName = $"{deviceName}_v{backup.VersionNumber}_{backup.TimestampUtc:yyyyMMdd_HHmmss}.config";

        return new ConfigurationDownloadDto
        {
            FileName = fileName,
            ContentType = "text/plain",
            RawConfigurationContent = rawContent,
            ChecksumSha256 = backup.ChecksumSha256
        };
    }
}