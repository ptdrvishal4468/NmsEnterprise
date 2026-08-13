using MediatR;
using Nms.Application.ConfigurationBackups.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.ConfigurationBackups.Queries.GetConfigurationBackupById;

public class GetConfigurationBackupByIdQueryHandler : IRequestHandler<GetConfigurationBackupByIdQuery, ConfigurationBackupDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetConfigurationBackupByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ConfigurationBackupDto> Handle(GetConfigurationBackupByIdQuery request, CancellationToken cancellationToken)
    {
        var backup = await _unitOfWork.ConfigurationBackups.GetByIdAsync(request.BackupId, cancellationToken);
        if (backup == null || backup.TenantId != request.TenantId)
        {
            throw new KeyNotFoundException($"Configuration backup '{request.BackupId}' was not found for tenant '{request.TenantId}'.");
        }

        return new ConfigurationBackupDto
        {
            Id = backup.Id,
            TenantId = backup.TenantId,
            DeviceId = backup.DeviceId,
            DeviceName = backup.Device?.Name ?? string.Empty,
            DeviceIpAddress = backup.Device?.IpAddress ?? string.Empty,
            VersionNumber = backup.VersionNumber,
            ChecksumSha256 = backup.ChecksumSha256,
            FileSizeBytes = backup.FileSizeBytes,
            Status = backup.Status,
            TriggerType = backup.TriggerType,
            FailureReason = backup.FailureReason,
            TimestampUtc = backup.TimestampUtc,
            IsEligibleForRestore = backup.IsEligibleForRestore,
            RestorePreparationNotes = backup.RestorePreparationNotes
        };
    }
}