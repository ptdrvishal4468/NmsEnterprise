using Nms.Domain.Entities;
using Nms.Domain.Enums;

namespace Nms.Domain.Interfaces;

public interface IConfigurationBackupRepository : IGenericRepository<ConfigurationBackup, Guid>
{
    Task<int> GetNextVersionNumberAsync(
        Guid tenantId,
        Guid deviceId,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<ConfigurationBackup> Items, int TotalCount)> GetBackupsPagedAsync(
        Guid tenantId,
        Guid? deviceId,
        BackupStatus? status,
        BackupTriggerType? triggerType,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<ConfigurationBackup?> GetLatestBackupAsync(
        Guid tenantId,
        Guid deviceId,
        CancellationToken cancellationToken = default);
}