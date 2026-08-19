using Microsoft.EntityFrameworkCore;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Infrastructure.Data.Repositories;

public class DeviceMetricRepository : GenericRepository<DeviceMetricRaw, long>, IDeviceMetricRepository
{
    public DeviceMetricRepository(NmsDbContext dbContext) : base(dbContext) { }

    public async Task AddBulkAsync(IEnumerable<DeviceMetricRaw> metrics, CancellationToken cancellationToken = default)
    {
        await DbSet.AddRangeAsync(metrics, cancellationToken);
    }

    public async Task<IEnumerable<DeviceMetricRaw>> GetMetricsForDeviceAsync(
        Guid deviceId,
        DateTime fromUtc,
        DateTime toUtc,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(m => m.DeviceId == deviceId && m.TimestampUtc >= fromUtc && m.TimestampUtc <= toUtc)
            .OrderByDescending(m => m.TimestampUtc)
            .ToListAsync(cancellationToken);
    }
}