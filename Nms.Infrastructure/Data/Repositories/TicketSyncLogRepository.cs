using Microsoft.EntityFrameworkCore;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Infrastructure.Data.Repositories;

public class TicketSyncLogRepository : GenericRepository<TicketSyncLog, Guid>, ITicketSyncLogRepository
{
    public TicketSyncLogRepository(NmsDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<TicketSyncLog>> GetByTicketIdAsync(Guid ticketId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(l => l.TicketId == ticketId)
            .OrderByDescending(l => l.TimestampUtc)
            .ToListAsync(cancellationToken);
    }
}