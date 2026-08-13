using Nms.Domain.Enums;

namespace Nms.Application.Firmware.Dtos;

public record DeviceFirmwareInventoryDto(
    Guid DeviceId,
    string DeviceName,
    string IpAddress,
    DeviceType DeviceType,
    string? Vendor,
    string? Model,
    string? CurrentFirmwareVersion,
    string? TargetBaselineVersion,
    FirmwareComplianceStatus ComplianceStatus,
    DateTime? LastSeenUtc);