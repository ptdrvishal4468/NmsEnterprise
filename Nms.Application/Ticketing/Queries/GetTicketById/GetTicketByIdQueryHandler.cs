using MediatR;
using Nms.Application.Ticketing.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Ticketing.Queries.GetTicketById;

public class GetTicketByIdQueryHandler : IRequestHandler<GetTicketByIdQuery, TicketDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTicketByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TicketDto?> Handle(GetTicketByIdQuery request, CancellationToken cancellationToken)
    {
        var ticket = await _unitOfWork.Tickets.GetWithDetailsAsync(request.Id, cancellationToken);
        if (ticket == null) return null;

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