using Nms.Domain.Entities;

namespace Nms.Domain.Interfaces;

public interface IScheduledReportRepository : IGenericRepository<ScheduledReport, Guid>
{
    Task<IReadOnlyList<ScheduledReport>> GetDueReportsAsync(DateTime currentTimeUtc, CancellationToken cancellationToken = default);
}