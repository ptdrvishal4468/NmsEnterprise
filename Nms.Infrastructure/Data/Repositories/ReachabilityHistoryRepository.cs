using Microsoft.EntityFrameworkCore;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Infrastructure.Data.Repositories;

public class ReachabilityHistoryRepository : GenericRepository<DeviceReachabilityHistory, Guid>, IReachabilityHistoryRepository
{
    public ReachabilityHistoryRepository(NmsDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<DeviceReachabilityHistory>> GetByDeviceIdAsync(
        Guid deviceId,
        int take,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(x => x.DeviceId == deviceId)
            .OrderByDescending(x => x.TimestampUtc)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<DeviceReachabilityHistory?> GetLatestByDeviceIdAsync(
        Guid deviceId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(x => x.DeviceId == deviceId)
            .OrderByDescending(x => x.TimestampUtc)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<DeviceReachabilityHistory> Items, int TotalCount)> GetPagedByDeviceIdAsync(
        Guid deviceId,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .AsNoTracking()
            .Where(x => x.DeviceId == deviceId)
            .OrderByDescending(x => x.TimestampUtc);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}