using MediatR;
using Nms.Domain.Enums;

namespace Nms.Application.Events.Commands.RecordEvent;

public record RecordEventCommand(
    EventCategory Category,
    EventSeverity Severity,
    string Source,
    string Message,
    Guid? DeviceId = null,
    string? CorrelationId = null,
    Guid? ParentEventId = null,
    string? MetadataJson = null) : IRequest<Guid>;