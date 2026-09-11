using Nms.Domain.Enums;

namespace Nms.Application.Ticketing.Dtos;

public record CreateTicketDto(
    string Title,
    string Description,
    TicketPriority Priority,
    TicketingProviderType ProviderType,
    Guid? DeviceId = null,
    Guid? AlertId = null,
    Guid? CustomerId = null,
    string? MetadataJson = null,
    bool DispatchToProvider = true);