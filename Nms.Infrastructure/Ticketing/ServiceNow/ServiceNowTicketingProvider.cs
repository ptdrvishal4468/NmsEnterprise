using Nms.Application.Common.Interfaces;
using Nms.Domain.Entities;
using Nms.Domain.Enums;

namespace Nms.Infrastructure.Ticketing.ServiceNow;

public class ServiceNowTicketingProvider : ITicketingProvider
{
    private readonly IServiceNowClient _client;

    public TicketingProviderType ProviderType => TicketingProviderType.ServiceNow;

    public ServiceNowTicketingProvider(IServiceNowClient client)
    {
        _client = client;
    }

    public async Task<(string ExternalId, string ExternalKey, string? ExternalUrl)> CreateTicketAsync(Ticket ticket, CancellationToken cancellationToken = default)
    {
        var urgency = ticket.Priority switch
        {
            TicketPriority.Critical => "1",
            TicketPriority.High => "2",
            TicketPriority.Medium => "3",
            _ => "4"
        };

        var (sysId, number, url) = await _client.CreateIncidentAsync(ticket.Title, ticket.Description, urgency, cancellationToken);
        return (sysId, number, url);
    }

    public async Task UpdateTicketAsync(Ticket ticket, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(ticket.ExternalTicketId)) return;

        var state = ticket.Status switch
        {
            TicketStatus.Closed => "7",
            TicketStatus.Resolved => "6",
            TicketStatus.InProgress => "2",
            _ => "1"
        };

        await _client.UpdateIncidentAsync(ticket.ExternalTicketId, state, "Status updated from NMS", cancellationToken);
    }

    public async Task<string?> GetTicketStatusAsync(string externalTicketId, CancellationToken cancellationToken = default)
    {
        return await _client.GetIncidentStatusAsync(externalTicketId, cancellationToken);
    }
}