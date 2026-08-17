using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Xunit;

namespace Nms.UnitTests.Ticketing;

public class TicketDomainTests
{
    [Fact]
    public void Ticket_Initialization_ShouldSetDefaultStateAndOpenStatus()
    {
        var tenantId = Guid.NewGuid();
        var ticket = new Ticket
        {
            TenantId = tenantId,
            Title = "Switch Core-01 Link Down",
            Description = "Port Gi0/1 down",
            Priority = TicketPriority.High,
            ProviderType = TicketingProviderType.ServiceNow
        };

        Assert.NotEqual(Guid.Empty, ticket.Id);
        Assert.Equal(tenantId, ticket.TenantId);
        Assert.Equal(TicketStatus.Open, ticket.Status);
        Assert.Equal(TicketPriority.High, ticket.Priority);
        Assert.Null(ticket.ExternalTicketId);
        Assert.Empty(ticket.SyncLogs);
    }

    [Fact]
    public void UpdateStatus_ShouldTransitionTicketStatus()
    {
        var ticket = new Ticket { Status = TicketStatus.Open };

        ticket.UpdateStatus(TicketStatus.InProgress);
        Assert.Equal(TicketStatus.InProgress, ticket.Status);

        ticket.UpdateStatus(TicketStatus.Resolved);
        Assert.Equal(TicketStatus.Resolved, ticket.Status);

        ticket.UpdateStatus(TicketStatus.Closed);
        Assert.Equal(TicketStatus.Closed, ticket.Status);
    }

    [Fact]
    public void SetExternalDetails_ShouldPopulateExternalFieldsAndSyncTimestamp()
    {
        var ticket = new Ticket();
        var externalId = "sys_123456";
        var externalKey = "INC001234";
        var externalStatus = "Active";
        var externalUrl = "https://servicenow.local/inc/INC001234";

        ticket.SetExternalDetails(externalId, externalKey, externalStatus, externalUrl);

        Assert.Equal(externalId, ticket.ExternalTicketId);
        Assert.Equal(externalKey, ticket.ExternalTicketKey);
        Assert.Equal(externalStatus, ticket.ExternalStatus);
        Assert.Equal(externalUrl, ticket.ExternalUrl);
        Assert.NotNull(ticket.LastSyncedAtUtc);
    }
}