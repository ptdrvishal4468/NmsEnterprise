using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Ticketing.Dtos;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Application.Ticketing.Commands.CloseTicket;

public class CloseTicketCommandHandler : IRequestHandler<CloseTicketCommand, TicketDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITicketingProviderFactory _providerFactory;

    public CloseTicketCommandHandler(IUnitOfWork unitOfWork, ITicketingProviderFactory providerFactory)
    {
        _unitOfWork = unitOfWork;
        _providerFactory = providerFactory;
    }

    public async Task<TicketDto?> Handle(CloseTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _unitOfWork.Tickets.GetByIdAsync(request.Id, cancellationToken);
        if (ticket == null) return null;

        ticket.UpdateStatus(TicketStatus.Closed);

        if (!string.IsNullOrEmpty(ticket.ExternalTicketId))
        {
            var provider = _providerFactory.GetProvider(ticket.ProviderType);
            try
            {
                await provider.UpdateTicketAsync(ticket, cancellationToken);
            }
            catch
            {
                // Fail-safe; local close preserved
            }
        }

        _unitOfWork.Tickets.Update(ticket);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new TicketDto(
            ticket.Id,
            ticket.TenantId,
            ticket.Title,
            ticket.Description,
            ticket.Priority,
            ticket.Status,
            ticket.ProviderType,
            ticket.ExternalTicketId,
            ticket.ExternalTicketKey,
            ticket.ExternalStatus,
            ticket.ExternalUrl,
            ticket.LastSyncedAtUtc,
            ticket.DeviceId,
            ticket.AlertId,
            ticket.CustomerId,
            ticket.MetadataJson,
            ticket.CreatedAtUtc,
            ticket.LastModifiedAtUtc);
    }
}