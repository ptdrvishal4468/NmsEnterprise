using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Common.Models;
using Nms.Application.Firmware.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Firmware.Queries.GetUpgradePlansPaged;

public class GetUpgradePlansPagedQueryHandler : IRequestHandler<GetUpgradePlansPagedQuery, PagedResult<FirmwareUpgradePlanDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;

    public GetUpgradePlansPagedQueryHandler(IUnitOfWork unitOfWork, ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<PagedResult<FirmwareUpgradePlanDto>> Handle(GetUpgradePlansPagedQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;

        var allPlans = await _unitOfWork.FirmwareUpgradePlans.GetAllAsync(cancellationToken);
        var query = allPlans.Where(p => p.TenantId == tenantId);

        if (request.DeviceId.HasValue)
        {
            query = query.Where(p => p.DeviceId == request.DeviceId.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(p => p.Status == request.Status.Value);
        }

        var planList = query.OrderByDescending(p => p.PlannedDateUtc).ToList();
        var totalCount = planList.Count;

        var pagedItems = planList
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var dtoList = new List<FirmwareUpgradePlanDto>();
        foreach (var plan in pagedItems)
        {
            var device = await _unitOfWork.Devices.GetByIdAsync(plan.DeviceId, cancellationToken);
            dtoList.Add(new FirmwareUpgradePlanDto(
                plan.Id,
                plan.TenantId,
                plan.DeviceId,
                device?.Name ?? string.Empty,
                device?.IpAddress ?? string.Empty,
                device?.FirmwareVersion,
                plan.TargetVersion,
                plan.PlannedDateUtc,
                plan.Status,
                plan.Notes,
                plan.CreatedAtUtc,
                plan.CreatedBy));
        }

        return new PagedResult<FirmwareUpgradePlanDto>(dtoList, totalCount, request.PageNumber, request.PageSize);
    }
}