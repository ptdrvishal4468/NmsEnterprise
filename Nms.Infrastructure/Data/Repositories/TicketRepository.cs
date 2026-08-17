using Microsoft.EntityFrameworkCore;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Infrastructure.Data.Repositories;

public class TicketRepository : GenericRepository<Ticket, Guid>, ITicketRepository
{
    public TicketRepository(NmsDbContext context) : base(context)
    {
    }

    public async Task<Ticket?> GetByExternalIdAsync(
        string externalTicketId,
        TicketingProviderType providerType,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(t => t.ExternalTicketId == externalTicketId && t.ProviderType == providerType, cancellationToken);
    }

    public async Task<Ticket?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(t => t.Device)
            .Include(t => t.Alert)
            .Include(t => t.Customer)
            .Include(t => t.SyncLogs)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Ticket>> GetPendingSyncTicketsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(t => t.ExternalTicketId == null && t.Status != TicketStatus.Closed)
            .ToListAsync(cancellationToken);
    }
}