using Nms.Domain.Enums;

namespace Nms.Application.Ticketing.Dtos;

public record TicketDto(
    Guid Id,
    Guid TenantId,
    string Title,
    string Description,
    TicketPriority Priority,
    TicketStatus Status,
    TicketingProviderType ProviderType,
    string? ExternalTicketId,
    string? ExternalTicketKey,
    string? ExternalStatus,
    string? ExternalUrl,
    DateTime? LastSyncedAtUtc,
    Guid? DeviceId,
    Guid? AlertId,
    Guid? CustomerId,
    string? MetadataJson,
    DateTime CreatedAtUtc,
    DateTime? LastModifiedAtUtc);