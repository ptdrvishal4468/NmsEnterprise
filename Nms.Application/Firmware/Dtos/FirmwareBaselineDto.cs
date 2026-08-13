namespace Nms.Application.Firmware.Dtos;

public record FirmwareBaselineDto(
    Guid Id,
    Guid TenantId,
    string Vendor,
    string Model,
    string TargetVersion,
    bool IsActive,
    string? Notes,
    DateTime CreatedAtUtc,
    string? CreatedBy);