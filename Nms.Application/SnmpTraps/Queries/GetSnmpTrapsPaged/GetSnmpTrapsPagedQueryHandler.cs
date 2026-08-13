using MediatR;
using Nms.Application.Common.Models;
using Nms.Application.SnmpTraps.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.SnmpTraps.Queries.GetSnmpTrapsPaged;

public sealed class GetSnmpTrapsPagedQueryHandler : IRequestHandler<GetSnmpTrapsPagedQuery, PagedResult<SnmpTrapMessageDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetSnmpTrapsPagedQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<SnmpTrapMessageDto>> Handle(GetSnmpTrapsPagedQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _unitOfWork.SnmpTraps.GetSnmpTrapsPagedAsync(
            request.TenantId,
            request.DeviceId,
            request.Severity,
            request.SourceIpAddress,
            request.EnterpriseOid,
            request.FromUtc,
            request.ToUtc,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var dtos = items.Select(x => new SnmpTrapMessageDto(
            x.Id,
            x.TenantId,
            x.DeviceId,
            x.SnmpVersion,
            x.Community,
            x.EnterpriseOid,
            x.GenericTrap,
            x.SpecificTrap,
            x.TrapOid,
            x.AgentAddress,
            x.SourceIpAddress,
            x.Severity,
            x.VarbindsJson,
            x.RawPayloadHex,
            x.IsMalformed,
            x.TimestampUtc,
            x.CreatedAtUtc)).ToList();

        return new PagedResult<SnmpTrapMessageDto>(dtos, totalCount, request.PageNumber, request.PageSize);
    }
}