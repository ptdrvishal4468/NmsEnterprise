using Moq;
using Nms.Application.Common.Interfaces;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Infrastructure.Ticketing.ServiceNow;
using Xunit;

namespace Nms.UnitTests.Ticketing;

public class ServiceNowTicketingProviderTests
{
    private readonly Mock<IServiceNowClient> _clientMock = new();

    [Fact]
    public async Task CreateTicketAsync_ShouldMapPriorityToUrgencyAndCallClient()
    {
        var ticket = new Ticket
        {
            Title = "High CPU Utilization",
            Description = "CPU at 98%",
            Priority = TicketPriority.Critical
        };

        _clientMock.Setup(c => c.CreateIncidentAsync(ticket.Title, ticket.Description, "1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(("sys_123", "INC00123", "https://now/123"));

        var provider = new ServiceNowTicketingProvider(_clientMock.Object);
        var (sysId, number, url) = await provider.CreateTicketAsync(ticket);

        Assert.Equal("sys_123", sysId);
        Assert.Equal("INC00123", number);
        _clientMock.Verify(c => c.CreateIncidentAsync(ticket.Title, ticket.Description, "1", It.IsAny<CancellationToken>()), Times.Once);
    }
}