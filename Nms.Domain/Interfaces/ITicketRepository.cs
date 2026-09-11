using Nms.Domain.Entities;
using Nms.Domain.Enums;

namespace Nms.Domain.Interfaces;

public interface ITicketRepository : IGenericRepository<Ticket, Guid>
{
    Task<Ticket?> GetByExternalIdAsync(string externalTicketId, TicketingProviderType providerType, CancellationToken cancellationToken = default);
    Task<Ticket?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Ticket>> GetPendingSyncTicketsAsync(CancellationToken cancellationToken = default);
}