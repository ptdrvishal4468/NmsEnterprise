using Nms.Domain.Entities;
using Nms.Domain.Enums;

namespace Nms.Application.Common.Interfaces;

public interface IConfigurationBackupEngine
{
    Task<ConfigurationBackup> ExecuteBackupAsync(
        Guid tenantId,
        Guid deviceId,
        BackupTriggerType triggerType,
        CancellationToken cancellationToken = default);
}