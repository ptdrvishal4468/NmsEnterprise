using Nms.Domain.Entities;
using Nms.Domain.Enums;

namespace Nms.Application.Common.Interfaces;

public interface ITicketingProvider
{
    TicketingProviderType ProviderType { get; }
    Task<(string ExternalId, string ExternalKey, string? ExternalUrl)> CreateTicketAsync(Ticket ticket, CancellationToken cancellationToken = default);
    Task UpdateTicketAsync(Ticket ticket, CancellationToken cancellationToken = default);
    Task<string?> GetTicketStatusAsync(string externalTicketId, CancellationToken cancellationToken = default);
}