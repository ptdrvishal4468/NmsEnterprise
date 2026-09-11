using Nms.Domain.Enums;

namespace Nms.Application.Firmware.Dtos;

public record FirmwareUpgradePlanDto(
    Guid Id,
    Guid TenantId,
    Guid DeviceId,
    string DeviceName,
    string DeviceIpAddress,
    string? CurrentFirmwareVersion,
    string TargetVersion,
    DateTime PlannedDateUtc,
    UpgradePlanStatus Status,
    string? Notes,
    DateTime CreatedAtUtc,
    string? CreatedBy);