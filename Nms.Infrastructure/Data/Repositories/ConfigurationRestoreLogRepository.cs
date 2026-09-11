using Microsoft.EntityFrameworkCore;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Infrastructure.Data.Repositories;

public class ConfigurationRestoreLogRepository : GenericRepository<ConfigurationRestoreLog, Guid>, IConfigurationRestoreLogRepository
{
    public ConfigurationRestoreLogRepository(NmsDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<ConfigurationRestoreLog>> GetByDeviceIdAsync(
        Guid tenantId,
        Guid deviceId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(x => x.Device)
            .Include(x => x.TargetBackup)
            .Include(x => x.PreRestoreBackup)
            .Where(x => x.TenantId == tenantId && x.DeviceId == deviceId)
            .OrderByDescending(x => x.StartTimeUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasActiveRestoreInProgressAsync(
        Guid tenantId,
        Guid deviceId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AnyAsync(x => x.TenantId == tenantId &&
                           x.DeviceId == deviceId &&
                           (x.Status == RestoreStatus.Pending || x.Status == RestoreStatus.InProgress),
                      cancellationToken);
    }
}