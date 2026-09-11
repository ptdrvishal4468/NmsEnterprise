using MediatR;
using Nms.Application.Common.Models;
using Nms.Application.Ticketing.Dtos;
using Nms.Domain.Enums;

namespace Nms.Application.Ticketing.Queries.GetTicketsPaged;

public record GetTicketsPagedQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string? SearchTerm = null,
    TicketStatus? Status = null,
    TicketPriority? Priority = null,
    TicketingProviderType? ProviderType = null) : IRequest<PagedResult<TicketDto>>;