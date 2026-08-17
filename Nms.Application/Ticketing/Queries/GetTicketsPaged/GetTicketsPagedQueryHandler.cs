using MediatR;
using Nms.Application.Common.Models;
using Nms.Application.Ticketing.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Ticketing.Queries.GetTicketsPaged;

public class GetTicketsPagedQueryHandler : IRequestHandler<GetTicketsPagedQuery, PagedResult<TicketDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTicketsPagedQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<TicketDto>> Handle(GetTicketsPagedQuery request, CancellationToken cancellationToken)
    {
        var allTickets = await _unitOfWork.Tickets.GetAllAsync(cancellationToken);
        var query = allTickets.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var search = request.SearchTerm.Trim().ToLower();
            query = query.Where(t => t.Title.ToLower().Contains(search) ||
                                     (t.ExternalTicketKey != null && t.ExternalTicketKey.ToLower().Contains(search)));
        }

        if (request.Status.HasValue)
            query = query.Where(t => t.Status == request.Status.Value);

        if (request.Priority.HasValue)
            query = query.Where(t => t.Priority == request.Priority.Value);

        if (request.ProviderType.HasValue)
            query = query.Where(t => t.ProviderType == request.ProviderType.Value);

        var totalCount = query.Count();
        var items = query
            .OrderByDescending(t => t.CreatedAtUtc)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(ticket => new TicketDto(
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
                ticket.LastModifiedAtUtc))
            .ToList();

        return new PagedResult<TicketDto>(items, totalCount, request.PageNumber, request.PageSize);
    }
}