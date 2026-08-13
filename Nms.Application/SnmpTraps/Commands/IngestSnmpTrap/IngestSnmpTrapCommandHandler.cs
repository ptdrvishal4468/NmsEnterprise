using MediatR;
using Microsoft.Extensions.Logging;
using Nms.Application.Common.Interfaces;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Application.SnmpTraps.Commands.IngestSnmpTrap;

public sealed class IngestSnmpTrapCommandHandler : IRequestHandler<IngestSnmpTrapCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<IngestSnmpTrapCommandHandler> _logger;

    public IngestSnmpTrapCommandHandler(
        IUnitOfWork unitOfWork,
        IEventPublisher eventPublisher,
        ILogger<IngestSnmpTrapCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public async Task<Guid> Handle(IngestSnmpTrapCommand request, CancellationToken cancellationToken)
    {
        var device = await _unitOfWork.Devices.GetByIpAddressAsync(request.SourceIpAddress, cancellationToken);
        var tenantId = device?.TenantId ?? Guid.Empty;

        var entity = new SnmpTrapMessage(
            tenantId: tenantId,
            snmpVersion: request.Version,
            enterpriseOid: request.EnterpriseOid,
            sourceIpAddress: request.SourceIpAddress,
            severity: request.Severity,
            varbindsJson: request.VarbindsJson,
            timestampUtc: request.TimestampUtc,
            deviceId: device?.Id,
            community: request.Community,
            genericTrap: request.GenericTrap,
            specificTrap: request.SpecificTrap,
            trapOid: request.TrapOid,
            agentAddress: request.AgentAddress,
            rawPayloadHex: request.RawPayloadHex,
            isMalformed: request.IsMalformed);

        await _unitOfWork.SnmpTraps.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (request.Severity >= TrapSeverity.Error && tenantId != Guid.Empty)
        {
            var eventSeverity = request.Severity switch
            {
                TrapSeverity.Critical => EventSeverity.Critical,
                TrapSeverity.Error => EventSeverity.Error,
                _ => EventSeverity.Warning
            };

            await _eventPublisher.PublishAsync(
                category: EventCategory.System,
                severity: eventSeverity,
                source: $"SNMP Trap Receiver ({request.SourceIpAddress})",
                message: $"SNMP Trap received [OID: {request.EnterpriseOid}]: {request.VarbindsJson}",
                deviceId: device?.Id,
                cancellationToken: cancellationToken);
        }

        return entity.Id;
    }
}