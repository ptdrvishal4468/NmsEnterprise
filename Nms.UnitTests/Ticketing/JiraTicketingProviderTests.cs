using Moq;
using Nms.Application.Common.Interfaces;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Infrastructure.Ticketing.Jira;
using Xunit;

namespace Nms.UnitTests.Ticketing;

public class JiraTicketingProviderTests
{
    private readonly Mock<IJiraClient> _clientMock = new();

    [Fact]
    public async Task CreateTicketAsync_ShouldMapPriorityAndCreateJiraIssue()
    {
        var ticket = new Ticket
        {
            Title = "Power Supply Failure",
            Description = "PSU-2 offline",
            Priority = TicketPriority.High
        };

        _clientMock.Setup(c => c.CreateIssueAsync(ticket.Title, ticket.Description, "Incident", "High", It.IsAny<CancellationToken>()))
            .ReturnsAsync(("10001", "NMS-10001", "https://jira/NMS-10001"));

        var provider = new JiraTicketingProvider(_clientMock.Object);
        var (issueId, issueKey, url) = await provider.CreateTicketAsync(ticket);

        Assert.Equal("10001", issueId);
        Assert.Equal("NMS-10001", issueKey);
        _clientMock.Verify(c => c.CreateIssueAsync(ticket.Title, ticket.Description, "Incident", "High", It.IsAny<CancellationToken>()), Times.Once);
    }
}