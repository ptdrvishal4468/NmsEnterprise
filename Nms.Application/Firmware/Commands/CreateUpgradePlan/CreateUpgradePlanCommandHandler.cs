using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Firmware.Dtos;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Application.Firmware.Commands.CreateUpgradePlan;

public class CreateUpgradePlanCommandHandler : IRequestHandler<CreateUpgradePlanCommand, FirmwareUpgradePlanDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;

    public CreateUpgradePlanCommandHandler(IUnitOfWork unitOfWork, ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<FirmwareUpgradePlanDto> Handle(CreateUpgradePlanCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;

        var device = await _unitOfWork.Devices.GetByIdAsync(request.DeviceId, cancellationToken);
        if (device == null || device.TenantId != tenantId)
        {
            throw new KeyNotFoundException($"Device with ID '{request.DeviceId}' was not found.");
        }

        var plan = new FirmwareUpgradePlan(
            Guid.NewGuid(),
            tenantId,
            request.DeviceId,
            request.TargetVersion,
            request.PlannedDateUtc,
            request.Notes);

        await _unitOfWork.FirmwareUpgradePlans.AddAsync(plan, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new FirmwareUpgradePlanDto(
            plan.Id,
            plan.TenantId,
            plan.DeviceId,
            device.Name,
            device.IpAddress,
            device.FirmwareVersion,
            plan.TargetVersion,
            plan.PlannedDateUtc,
            plan.Status,
            plan.Notes,
            plan.CreatedAtUtc,
            plan.CreatedBy);
    }
}