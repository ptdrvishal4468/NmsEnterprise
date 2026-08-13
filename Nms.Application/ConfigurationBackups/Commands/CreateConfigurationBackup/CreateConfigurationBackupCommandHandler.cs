using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.ConfigurationBackups.Dtos;

namespace Nms.Application.ConfigurationBackups.Commands.CreateConfigurationBackup;

public class CreateConfigurationBackupCommandHandler : IRequestHandler<CreateConfigurationBackupCommand, ConfigurationBackupDto>
{
    private readonly IConfigurationBackupEngine _backupEngine;

    public CreateConfigurationBackupCommandHandler(IConfigurationBackupEngine backupEngine)
    {
        _backupEngine = backupEngine;
    }

    public async Task<ConfigurationBackupDto> Handle(CreateConfigurationBackupCommand request, CancellationToken cancellationToken)
    {
        var backup = await _backupEngine.ExecuteBackupAsync(
            request.TenantId,
            request.DeviceId,
            request.TriggerType,
            cancellationToken);

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