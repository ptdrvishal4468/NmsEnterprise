using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Application.Syslog.Commands.IngestSyslogMessage;

public class IngestSyslogMessageCommandHandler : IRequestHandler<IngestSyslogMessageCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEventPublisher _eventPublisher;

    public IngestSyslogMessageCommandHandler(
        IUnitOfWork unitOfWork,
        IEventPublisher eventPublisher)
    {
        _unitOfWork = unitOfWork;
        _eventPublisher = eventPublisher;
    }

    public async Task<Guid> Handle(IngestSyslogMessageCommand request, CancellationToken cancellationToken)
    {
        // Try resolving device by IP address
        var device = await _unitOfWork.Devices.GetByIpAddressAsync(request.SourceIpAddress, cancellationToken);

        var syslogMessage = new SyslogMessage(Guid.NewGuid())
        {
            TenantId = request.TenantId,
            DeviceId = device?.Id,
            Facility = request.Facility,
            Severity = request.Severity,
            FacilityName = request.Facility.ToString(),
            SeverityName = request.Severity.ToString(),
            TimestampUtc = request.TimestampUtc,
            Hostname = request.Hostname,
            AppTag = request.AppTag,
            ProcessId = request.ProcessId,
            MessageId = request.MessageId,
            Message = request.Message,
            RawMessage = request.RawMessage,
            SourceIpAddress = request.SourceIpAddress,
            IsMalformed = request.IsMalformed
        };

        await _unitOfWork.Syslogs.AddAsync(syslogMessage, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Bridge Critical/Error/Emergency logs into Phase 32 Event Management
        if (request.Severity <= SyslogSeverity.Error)
        {
            var eventSeverity = request.Severity switch
            {
                SyslogSeverity.Emergency => EventSeverity.Critical,
                SyslogSeverity.Alert => EventSeverity.Critical,
                SyslogSeverity.Critical => EventSeverity.Critical,
                SyslogSeverity.Error => EventSeverity.Error,
                _ => EventSeverity.Warning
            };

            await _eventPublisher.PublishAsync(
                category: EventCategory.System,
                severity: eventSeverity,
                source: $"Syslog:{request.AppTag ?? request.SourceIpAddress}",
                message: request.Message,
                deviceId: device?.Id,
                correlationId: request.MessageId,
                cancellationToken: cancellationToken);
        }

        return syslogMessage.Id;
    }
}