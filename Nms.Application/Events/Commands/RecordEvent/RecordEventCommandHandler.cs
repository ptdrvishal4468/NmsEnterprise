using MediatR;
using Nms.Application.Common.Interfaces;

namespace Nms.Application.Events.Commands.RecordEvent;

public class RecordEventCommandHandler : IRequestHandler<RecordEventCommand, Guid>
{
    private readonly IEventPublisher _eventPublisher;

    public RecordEventCommandHandler(IEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public async Task<Guid> Handle(RecordEventCommand request, CancellationToken cancellationToken)
    {
        return await _eventPublisher.PublishAsync(
            category: request.Category,
            severity: request.Severity,
            source: request.Source,
            message: request.Message,
            deviceId: request.DeviceId,
            correlationId: request.CorrelationId,
            parentEventId: request.ParentEventId,
            metadataJson: request.MetadataJson,
            cancellationToken: cancellationToken);
    }
}