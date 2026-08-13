using MediatR;
using Nms.Application.SnmpTraps.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.SnmpTraps.Queries.GetSnmpTrapById;

public sealed class GetSnmpTrapByIdQueryHandler : IRequestHandler<GetSnmpTrapByIdQuery, SnmpTrapMessageDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetSnmpTrapByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<SnmpTrapMessageDto?> Handle(GetSnmpTrapByIdQuery request, CancellationToken cancellationToken)
    {
        var trap = await _unitOfWork.SnmpTraps.GetByIdAsync(request.Id, cancellationToken);

        if (trap == null || trap.TenantId != request.TenantId)
        {
            return null;
        }

        return new SnmpTrapMessageDto(
            trap.Id,
            trap.TenantId,
            trap.DeviceId,
            trap.SnmpVersion,
            trap.Community,
            trap.EnterpriseOid,
            trap.GenericTrap,
            trap.SpecificTrap,
            trap.TrapOid,
            trap.AgentAddress,
            trap.SourceIpAddress,
            trap.Severity,
            trap.VarbindsJson,
            trap.RawPayloadHex,
            trap.IsMalformed,
            trap.TimestampUtc,
            trap.CreatedAtUtc);
    }
}