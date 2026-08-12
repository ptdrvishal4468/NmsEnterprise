using Nms.Application.Common.Interfaces;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Application.Events.Services;

public class EventPublisher : IEventPublisher
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;

    public EventPublisher(IUnitOfWork unitOfWork, ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<Guid> PublishAsync(
        EventCategory category,
        EventSeverity severity,
        string source,
        string message,
        Guid? deviceId = null,
        string? correlationId = null,
        Guid? parentEventId = null,
        string? metadataJson = null,
        CancellationToken cancellationToken = default)
    {
        if (!_tenantContext.IsResolved)
        {
            throw new InvalidOperationException("Cannot record operational event without an active tenant context.");
        }

        var eventId = Guid.NewGuid();
        var deviceEvent = new DeviceEvent(
            id: eventId,
            tenantId: _tenantContext.TenantId,
            category: category,
            severity: severity,
            source: source,
            message: message,
            deviceId: deviceId,
            correlationId: correlationId,
            parentEventId: parentEventId,
            metadataJson: metadataJson);

        await _unitOfWork.Events.AddAsync(deviceEvent, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return eventId;
    }
}