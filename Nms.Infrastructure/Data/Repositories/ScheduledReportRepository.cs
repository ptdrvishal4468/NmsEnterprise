using Microsoft.EntityFrameworkCore;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Infrastructure.Data.Repositories;

public class ScheduledReportRepository : GenericRepository<ScheduledReport, Guid>, IScheduledReportRepository
{
    public ScheduledReportRepository(NmsDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<ScheduledReport>> GetDueReportsAsync(DateTime currentTimeUtc, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(sr => sr.IsActive && sr.NextRunUtc != null && sr.NextRunUtc <= currentTimeUtc)
            .ToListAsync(cancellationToken);
    }
}