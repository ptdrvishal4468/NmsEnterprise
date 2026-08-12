using Nms.Domain.Enums;

namespace Nms.Application.Common.Interfaces;

public interface IEventPublisher
{
    Task<Guid> PublishAsync(
        EventCategory category,
        EventSeverity severity,
        string source,
        string message,
        Guid? deviceId = null,
        string? correlationId = null,
        Guid? parentEventId = null,
        string? metadataJson = null,
        CancellationToken cancellationToken = default);
}