using MediatR;
using Nms.Application.Ticketing.Dtos;
using Nms.Domain.Enums;

namespace Nms.Application.Ticketing.Commands.CreateTicket;

public record CreateTicketCommand(
    string Title,
    string Description,
    TicketPriority Priority,
    TicketingProviderType ProviderType,
    Guid? DeviceId = null,
    Guid? AlertId = null,
    Guid? CustomerId = null,
    string? MetadataJson = null,
    bool DispatchToProvider = true) : IRequest<TicketDto>;