using Nms.Domain.Entities;

namespace Nms.Domain.Interfaces;

public interface ITicketSyncLogRepository : IGenericRepository<TicketSyncLog, Guid>
{
    Task<IReadOnlyList<TicketSyncLog>> GetByTicketIdAsync(Guid ticketId, CancellationToken cancellationToken = default);
}