using Moq;
using Nms.Application.Common.Interfaces;
using Nms.Application.Ticketing.Commands.CloseTicket;
using Nms.Application.Ticketing.Commands.CreateTicket;
using Nms.Application.Ticketing.Commands.SyncTicket;
using Nms.Application.Ticketing.Commands.UpdateTicket;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;
using Xunit;

namespace Nms.UnitTests.Ticketing;

public class TicketCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ITicketRepository> _ticketRepoMock = new();
    private readonly Mock<ITicketSyncLogRepository> _syncLogRepoMock = new();
    private readonly Mock<ITenantContext> _tenantContextMock = new();
    private readonly Mock<ITicketingProviderFactory> _providerFactoryMock = new();
    private readonly Mock<ITicketingProvider> _providerMock = new();
    private readonly Guid _tenantId = Guid.NewGuid();

    public TicketCommandHandlerTests()
    {
        _tenantContextMock.Setup(t => t.TenantId).Returns(_tenantId);
        _unitOfWorkMock.Setup(u => u.Tickets).Returns(_ticketRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.TicketSyncLogs).Returns(_syncLogRepoMock.Object);
        _providerFactoryMock.Setup(f => f.GetProvider(It.IsAny<TicketingProviderType>())).Returns(_providerMock.Object);
    }

    [Fact]
    public async Task CreateTicketCommandHandler_ShouldCreateTicketAndDispatchToProvider()
    {
        var command = new CreateTicketCommand(
            "Link flap on Router-01",
            "High error rate detected",
            TicketPriority.High,
            TicketingProviderType.ServiceNow,
            DispatchToProvider: true);

        _providerMock.Setup(p => p.CreateTicketAsync(It.IsAny<Ticket>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(("sys_999", "INC00999", "https://servicenow.local/INC00999"));

        var handler = new CreateTicketCommandHandler(_unitOfWorkMock.Object, _tenantContextMock.Object, _providerFactoryMock.Object);
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("INC00999", result.ExternalTicketKey);
        Assert.Equal(_tenantId, result.TenantId);
        _ticketRepoMock.Verify(r => r.AddAsync(It.IsAny<Ticket>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateTicketCommandHandler_WhenTicketExists_ShouldUpdateProperties()
    {
        var ticketId = Guid.NewGuid();
        var existingTicket = new Ticket(ticketId)
        {
            TenantId = _tenantId,
            Title = "Old Title",
            Status = TicketStatus.Open
        };

        _ticketRepoMock.Setup(r => r.GetByIdAsync(ticketId, It.IsAny<CancellationToken>())).ReturnsAsync(existingTicket);

        var command = new UpdateTicketCommand(ticketId, "New Title", "New Desc", TicketPriority.Critical, TicketStatus.InProgress);
        var handler = new UpdateTicketCommandHandler(_unitOfWorkMock.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("New Title", result.Title);
        Assert.Equal(TicketStatus.InProgress, result.Status);
        _ticketRepoMock.Verify(r => r.Update(existingTicket), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CloseTicketCommandHandler_ShouldSetStatusClosed()
    {
        var ticketId = Guid.NewGuid();
        var existingTicket = new Ticket(ticketId)
        {
            TenantId = _tenantId,
            Status = TicketStatus.InProgress,
            ExternalTicketId = "ext_1"
        };

        _ticketRepoMock.Setup(r => r.GetByIdAsync(ticketId, It.IsAny<CancellationToken>())).ReturnsAsync(existingTicket);

        var command = new CloseTicketCommand(ticketId);
        var handler = new CloseTicketCommandHandler(_unitOfWorkMock.Object, _providerFactoryMock.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(TicketStatus.Closed, result.Status);
        _providerMock.Verify(p => p.UpdateTicketAsync(existingTicket, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SyncTicketCommandHandler_ShouldSynchronizeStatusAndRecordLog()
    {
        var ticketId = Guid.NewGuid();
        var ticket = new Ticket(ticketId)
        {
            TenantId = _tenantId,
            ExternalTicketId = "sys_123",
            ProviderType = TicketingProviderType.ServiceNow
        };

        _ticketRepoMock.Setup(r => r.GetByIdAsync(ticketId, It.IsAny<CancellationToken>())).ReturnsAsync(ticket);
        _providerMock.Setup(p => p.GetTicketStatusAsync("sys_123", It.IsAny<CancellationToken>())).ReturnsAsync("Resolved");

        var command = new SyncTicketCommand(ticketId);
        var handler = new SyncTicketCommandHandler(_unitOfWorkMock.Object, _tenantContextMock.Object, _providerFactoryMock.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal("Resolved", result.ExternalStatus);
        _syncLogRepoMock.Verify(l => l.AddAsync(It.IsAny<TicketSyncLog>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}