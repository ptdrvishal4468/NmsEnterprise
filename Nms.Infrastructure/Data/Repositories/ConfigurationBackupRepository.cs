using Microsoft.EntityFrameworkCore;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Infrastructure.Data.Repositories;

public class ConfigurationBackupRepository : GenericRepository<ConfigurationBackup, Guid>, IConfigurationBackupRepository
{
    public ConfigurationBackupRepository(NmsDbContext context) : base(context) { }

    public async Task<int> GetNextVersionNumberAsync(Guid tenantId, Guid deviceId, CancellationToken cancellationToken = default)
    {
        var maxVersion = await Context.ConfigurationBackups
            .Where(b => b.TenantId == tenantId && b.DeviceId == deviceId)
            .MaxAsync(b => (int?)b.VersionNumber, cancellationToken);

        return (maxVersion ?? 0) + 1;
    }

    public async Task<(IReadOnlyList<ConfigurationBackup> Items, int TotalCount)> GetBackupsPagedAsync(
        Guid tenantId,
        Guid? deviceId,
        BackupStatus? status,
        BackupTriggerType? triggerType,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = Context.ConfigurationBackups
            .Include(b => b.Device)
            .Where(b => b.TenantId == tenantId);

        if (deviceId.HasValue)
            query = query.Where(b => b.DeviceId == deviceId.Value);

        if (status.HasValue)
            query = query.Where(b => b.Status == status.Value);

        if (triggerType.HasValue)
            query = query.Where(b => b.TriggerType == triggerType.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(b => b.TimestampUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<ConfigurationBackup?> GetLatestBackupAsync(Guid tenantId, Guid deviceId, CancellationToken cancellationToken = default)
    {
        return await Context.ConfigurationBackups
            .Include(b => b.Device)
            .Where(b => b.TenantId == tenantId && b.DeviceId == deviceId && b.Status == BackupStatus.Success)
            .OrderByDescending(b => b.VersionNumber)
            .FirstOrDefaultAsync(cancellationToken);
    }
}