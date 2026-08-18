using Microsoft.EntityFrameworkCore;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Infrastructure.Data.Repositories;

public class ConfigurationDriftRepository : GenericRepository<ConfigurationDriftRecord, Guid>, IConfigurationDriftRepository
{
    public ConfigurationDriftRepository(NmsDbContext context) : base(context)
    {
    }

    public async Task<ConfigurationDriftRecord?> GetLatestByDeviceIdAsync(
        Guid tenantId,
        Guid deviceId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(d => d.Device)
            .Include(d => d.BaselineBackup)
            .Include(d => d.CurrentBackup)
            .Where(d => d.TenantId == tenantId && d.DeviceId == deviceId)
            .OrderByDescending(d => d.DetectedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<ConfigurationDriftRecord> Items, int TotalCount)> GetPagedAsync(
        Guid tenantId,
        int page,
        int pageSize,
        Guid? deviceId = null,
        bool? hasDrift = null,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .Include(d => d.Device)
            .Include(d => d.BaselineBackup)
            .Include(d => d.CurrentBackup)
            .AsNoTracking()
            .Where(d => d.TenantId == tenantId);

        if (deviceId.HasValue)
            query = query.Where(d => d.DeviceId == deviceId.Value);

        if (hasDrift.HasValue)
            query = query.Where(d => d.HasDrift == hasDrift.Value);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(d => d.DetectedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}