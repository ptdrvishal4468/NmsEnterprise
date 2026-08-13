using MediatR;
using Nms.Application.Firmware.Dtos;

namespace Nms.Application.Firmware.Commands.CreateUpgradePlan;

public record CreateUpgradePlanCommand(
    Guid DeviceId,
    string TargetVersion,
    DateTime PlannedDateUtc,
    string? Notes = null) : IRequest<FirmwareUpgradePlanDto>;