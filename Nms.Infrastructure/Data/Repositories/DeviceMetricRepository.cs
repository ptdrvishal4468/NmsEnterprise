using Microsoft.EntityFrameworkCore;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Infrastructure.Data.Repositories;

public class DeviceMetricRepository : GenericRepository<DeviceMetricRaw, long>, IDeviceMetricRepository
{
    private readonly NmsDbContext _dbContext;

    public DeviceMetricRepository(NmsDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddBulkAsync(IEnumerable<DeviceMetricRaw> metrics, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<DeviceMetricRaw>().AddRangeAsync(metrics, cancellationToken);
    }

    public async Task<IEnumerable<DeviceMetricRaw>> GetMetricsForDeviceAsync(
        Guid deviceId,
        DateTime fromUtc,
        DateTime toUtc,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<DeviceMetricRaw>()
            .AsNoTracking()
            .Where(m => m.DeviceId == deviceId && m.TimestampUtc >= fromUtc && m.TimestampUtc <= toUtc)
            .OrderByDescending(m => m.TimestampUtc)
            .ToListAsync(cancellationToken);
    }
}