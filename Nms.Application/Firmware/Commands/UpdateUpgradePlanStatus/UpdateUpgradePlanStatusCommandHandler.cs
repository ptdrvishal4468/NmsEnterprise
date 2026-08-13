using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Firmware.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Firmware.Commands.UpdateUpgradePlanStatus;

public class UpdateUpgradePlanStatusCommandHandler : IRequestHandler<UpdateUpgradePlanStatusCommand, FirmwareUpgradePlanDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;

    public UpdateUpgradePlanStatusCommandHandler(IUnitOfWork unitOfWork, ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<FirmwareUpgradePlanDto> Handle(UpdateUpgradePlanStatusCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;

        var plan = await _unitOfWork.FirmwareUpgradePlans.GetByIdAsync(request.PlanId, cancellationToken);
        if (plan == null || plan.TenantId != tenantId)
        {
            throw new KeyNotFoundException($"Upgrade plan with ID '{request.PlanId}' was not found.");
        }

        plan.UpdateStatus(request.Status);
        _unitOfWork.FirmwareUpgradePlans.Update(plan);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var device = await _unitOfWork.Devices.GetByIdAsync(plan.DeviceId, cancellationToken);

        return new FirmwareUpgradePlanDto(
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
            plan.CreatedBy);
    }
}