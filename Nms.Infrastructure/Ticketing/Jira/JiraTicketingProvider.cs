using Nms.Application.Common.Interfaces;
using Nms.Domain.Entities;
using Nms.Domain.Enums;

namespace Nms.Infrastructure.Ticketing.Jira;

public class JiraTicketingProvider : ITicketingProvider
{
    private readonly IJiraClient _client;

    public TicketingProviderType ProviderType => TicketingProviderType.Jira;

    public JiraTicketingProvider(IJiraClient client)
    {
        _client = client;
    }

    public async Task<(string ExternalId, string ExternalKey, string? ExternalUrl)> CreateTicketAsync(Ticket ticket, CancellationToken cancellationToken = default)
    {
        var priority = ticket.Priority switch
        {
            TicketPriority.Critical => "Highest",
            TicketPriority.High => "High",
            TicketPriority.Medium => "Medium",
            _ => "Low"
        };

        var (issueId, issueKey, url) = await _client.CreateIssueAsync(ticket.Title, ticket.Description, "Incident", priority, cancellationToken);
        return (issueId, issueKey, url);
    }

    public async Task UpdateTicketAsync(Ticket ticket, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(ticket.ExternalTicketKey)) return;

        var transitionId = ticket.Status switch
        {
            TicketStatus.Closed => "31",
            TicketStatus.Resolved => "21",
            TicketStatus.InProgress => "11",
            _ => "1"
        };

        await _client.UpdateIssueAsync(ticket.ExternalTicketKey, transitionId, "Updated from NMS", cancellationToken);
    }

    public async Task<string?> GetTicketStatusAsync(string externalTicketId, CancellationToken cancellationToken = default)
    {
        return await _client.GetIssueStatusAsync(externalTicketId, cancellationToken);
    }
}