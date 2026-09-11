using Microsoft.EntityFrameworkCore;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Infrastructure.Data.Repositories;

public class BackupScheduleRepository : GenericRepository<BackupSchedule, Guid>, IBackupScheduleRepository
{
    public BackupScheduleRepository(NmsDbContext context) : base(context) { }

    public async Task<(IReadOnlyList<BackupSchedule> Items, int TotalCount)> GetSchedulesPagedAsync(
        Guid tenantId,
        Guid? deviceId,
        bool? isEnabled,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = Context.BackupSchedules
            .Include(s => s.Device)
            .Where(s => s.TenantId == tenantId);

        if (deviceId.HasValue)
            query = query.Where(s => s.DeviceId == deviceId.Value);

        if (isEnabled.HasValue)
            query = query.Where(s => s.IsEnabled == isEnabled.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(s => s.CreatedAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IReadOnlyList<BackupSchedule>> GetDueSchedulesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await Context.BackupSchedules
            .Include(s => s.Device)
            .Where(s => s.IsEnabled && s.NextRunUtc <= now)
            .ToListAsync(cancellationToken);
    }
}