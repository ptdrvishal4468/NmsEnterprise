using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Ticketing.Dtos;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Application.Ticketing.Commands.SyncTicket;

public class SyncTicketCommandHandler : IRequestHandler<SyncTicketCommand, SyncTicketResultDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;
    private readonly ITicketingProviderFactory _providerFactory;

    public SyncTicketCommandHandler(
        IUnitOfWork unitOfWork,
        ITenantContext tenantContext,
        ITicketingProviderFactory providerFactory)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
        _providerFactory = providerFactory;
    }

    public async Task<SyncTicketResultDto?> Handle(SyncTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _unitOfWork.Tickets.GetByIdAsync(request.Id, cancellationToken);
        if (ticket == null) return null;

        var provider = _providerFactory.GetProvider(ticket.ProviderType);

        try
        {
            if (string.IsNullOrEmpty(ticket.ExternalTicketId))
            {
                var (externalId, externalKey, externalUrl) = await provider.CreateTicketAsync(ticket, cancellationToken);
                ticket.SetExternalDetails(externalId, externalKey, "Synchronized", externalUrl);
            }
            else
            {
                var externalStatus = await provider.GetTicketStatusAsync(ticket.ExternalTicketId, cancellationToken);
                ticket.ExternalStatus = externalStatus;
                ticket.LastSyncedAtUtc = DateTime.UtcNow;
            }

            var log = new TicketSyncLog
            {
                TenantId = _tenantContext.TenantId,
                TicketId = ticket.Id,
                ProviderType = ticket.ProviderType,
                Direction = TicketSyncDirection.Inbound,
                Status = TicketSyncStatus.Success,
                ResponsePayload = $"ExternalStatus: {ticket.ExternalStatus}"
            };

            await _unitOfWork.TicketSyncLogs.AddAsync(log, cancellationToken);
            _unitOfWork.Tickets.Update(ticket);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SyncTicketResultDto(ticket.Id, true, ticket.ExternalTicketId, ticket.ExternalStatus, null, DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            var failedLog = new TicketSyncLog
            {
                TenantId = _tenantContext.TenantId,
                TicketId = ticket.Id,
                ProviderType = ticket.ProviderType,
                Direction = TicketSyncDirection.Inbound,
                Status = TicketSyncStatus.Failed,
                ErrorMessage = ex.Message
            };

            await _unitOfWork.TicketSyncLogs.AddAsync(failedLog, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SyncTicketResultDto(ticket.Id, false, ticket.ExternalTicketId, ticket.ExternalStatus, ex.Message, DateTime.UtcNow);
        }
    }
}