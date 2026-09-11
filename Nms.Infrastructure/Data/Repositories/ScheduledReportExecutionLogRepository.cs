using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Infrastructure.Data.Repositories;

public class ScheduledReportExecutionLogRepository : GenericRepository<ScheduledReportExecutionLog, Guid>, IScheduledReportExecutionLogRepository
{
    public ScheduledReportExecutionLogRepository(NmsDbContext context) : base(context)
    {
    }
}