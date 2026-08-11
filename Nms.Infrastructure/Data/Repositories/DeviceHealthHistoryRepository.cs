using Microsoft.EntityFrameworkCore;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Infrastructure.Data.Repositories;

public class DeviceHealthHistoryRepository : GenericRepository<DeviceHealthHistory, Guid>, IDeviceHealthHistoryRepository
{
    public DeviceHealthHistoryRepository(NmsDbContext context) : base(context)
    {
    }

    public async Task<(IReadOnlyList<DeviceHealthHistory> Items, int TotalCount)> GetPagedByDeviceIdAsync(
        Guid deviceId,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .AsNoTracking()
            .Where(h => h.DeviceId == deviceId)
            .OrderByDescending(h => h.TimestampUtc);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}