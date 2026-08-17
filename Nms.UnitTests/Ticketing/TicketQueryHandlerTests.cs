using Moq;
using Nms.Application.Ticketing.Queries.GetTicketById;
using Nms.Application.Ticketing.Queries.GetTicketsPaged;
using Nms.Application.Ticketing.Queries.GetTicketSyncLogs;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;
using Xunit;

namespace Nms.UnitTests.Ticketing;

public class TicketQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ITicketRepository> _ticketRepoMock = new();
    private readonly Mock<ITicketSyncLogRepository> _syncLogRepoMock = new();

    public TicketQueryHandlerTests()
    {
        _unitOfWorkMock.Setup(u => u.Tickets).Returns(_ticketRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.TicketSyncLogs).Returns(_syncLogRepoMock.Object);
    }

    [Fact]
    public async Task GetTicketByIdQueryHandler_WhenFound_ShouldReturnDto()
    {
        var ticketId = Guid.NewGuid();
        var ticket = new Ticket(ticketId)
        {
            TenantId = Guid.NewGuid(),
            Title = "BGP Peer Down",
            Description = "Session dropped",
            Priority = TicketPriority.Critical,
            Status = TicketStatus.Open
        };

        _ticketRepoMock.Setup(r => r.GetWithDetailsAsync(ticketId, It.IsAny<CancellationToken>())).ReturnsAsync(ticket);

        var handler = new GetTicketByIdQueryHandler(_unitOfWorkMock.Object);
        var result = await handler.Handle(new GetTicketByIdQuery(ticketId), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("BGP Peer Down", result.Title);
    }

    [Fact]
    public async Task GetTicketsPagedQueryHandler_ShouldFilterAndPaginate()
    {
        var tickets = new List<Ticket>
        {
            new() { Title = "Switch 1 Error", Status = TicketStatus.Open, Priority = TicketPriority.High },
            new() { Title = "Switch 2 Error", Status = TicketStatus.Closed, Priority = TicketPriority.Low },
            new() { Title = "Firewall Alert", Status = TicketStatus.Open, Priority = TicketPriority.High }
        };

        _ticketRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(tickets);

        var handler = new GetTicketsPagedQueryHandler(_unitOfWorkMock.Object);
        var result = await handler.Handle(new GetTicketsPagedQuery(1, 10, SearchTerm: "Switch", Status: TicketStatus.Open), CancellationToken.None);

        Assert.Single(result.Items);
        Assert.Equal("Switch 1 Error", result.Items[0].Title);
        Assert.Equal(1, result.TotalCount);
    }

    [Fact]
    public async Task GetTicketSyncLogsQueryHandler_ShouldReturnLogsForTicket()
    {
        var ticketId = Guid.NewGuid();
        var logs = new List<TicketSyncLog>
        {
            new() { TicketId = ticketId, ProviderType = TicketingProviderType.ServiceNow, Status = TicketSyncStatus.Success }
        };

        _syncLogRepoMock.Setup(r => r.GetByTicketIdAsync(ticketId, It.IsAny<CancellationToken>())).ReturnsAsync(logs);

        var handler = new GetTicketSyncLogsQueryHandler(_unitOfWorkMock.Object);
        var result = await handler.Handle(new GetTicketSyncLogsQuery(ticketId), CancellationToken.None);

        Assert.Single(result);
        Assert.Equal(TicketSyncStatus.Success, result[0].Status);
    }
}