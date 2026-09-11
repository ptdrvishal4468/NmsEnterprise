using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.ConfigurationBackups.Dtos;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Application.ConfigurationBackups.Commands.PrepareRestore;

public class PrepareRestoreCommandHandler : IRequestHandler<PrepareRestoreCommand, RestorePreparationDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfigurationStorageService _storageService;

    public PrepareRestoreCommandHandler(IUnitOfWork unitOfWork, IConfigurationStorageService storageService)
    {
        _unitOfWork = unitOfWork;
        _storageService = storageService;
    }

    public async Task<RestorePreparationDto> Handle(PrepareRestoreCommand request, CancellationToken cancellationToken)
    {
        var backup = await _unitOfWork.ConfigurationBackups.GetByIdAsync(request.BackupId, cancellationToken);
        if (backup == null || backup.TenantId != request.TenantId)
        {
            throw new KeyNotFoundException($"Configuration backup '{request.BackupId}' was not found for tenant '{request.TenantId}'.");
        }

        if (backup.Status != BackupStatus.Success)
        {
            throw new InvalidOperationException($"Cannot prepare backup '{request.BackupId}' for restore because its status is '{backup.Status}'. Only successful backups are eligible.");
        }

        bool fileExists = false;
        string prepNotes = request.Notes ?? "Backup verified and prepared for restoration.";

        try
        {
            var content = await _storageService.GetBackupFileContentAsync(backup.StoragePath, cancellationToken);
            fileExists = !string.IsNullOrEmpty(content);
        }
        catch (Exception ex)
        {
            prepNotes = $"Restore preparation warning: File check failed - {ex.Message}";
        }

        backup.SetRestorePreparation(fileExists, prepNotes);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new RestorePreparationDto
        {
            BackupId = backup.Id,
            DeviceId = backup.DeviceId,
            VersionNumber = backup.VersionNumber,
            IsEligibleForRestore = backup.IsEligibleForRestore,
            ChecksumSha256 = backup.ChecksumSha256,
            FileExistsOnDisk = fileExists,
            PreparationNotes = prepNotes,
            PreparedAtUtc = DateTime.UtcNow
        };
    }
}