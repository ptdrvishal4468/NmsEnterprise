using MediatR;
using Nms.Application.Ticketing.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Ticketing.Commands.UpdateTicket;

public class UpdateTicketCommandHandler : IRequestHandler<UpdateTicketCommand, TicketDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTicketCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TicketDto?> Handle(UpdateTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _unitOfWork.Tickets.GetByIdAsync(request.Id, cancellationToken);
        if (ticket == null) return null;

        ticket.Title = request.Title;
        ticket.Description = request.Description;
        ticket.Priority = request.Priority;
        ticket.UpdateStatus(request.Status);
        ticket.MetadataJson = request.MetadataJson;

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