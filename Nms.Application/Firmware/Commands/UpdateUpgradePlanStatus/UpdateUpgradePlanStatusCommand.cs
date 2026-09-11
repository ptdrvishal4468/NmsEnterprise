using MediatR;
using Nms.Application.Firmware.Dtos;
using Nms.Domain.Enums;

namespace Nms.Application.Firmware.Commands.UpdateUpgradePlanStatus;

public record UpdateUpgradePlanStatusCommand(
    Guid PlanId,
    UpgradePlanStatus Status) : IRequest<FirmwareUpgradePlanDto>;