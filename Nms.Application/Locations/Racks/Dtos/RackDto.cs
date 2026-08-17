namespace Nms.Application.Locations.Racks.Dtos;

public record RackDto(
    Guid Id,
    Guid TenantId,
    Guid RoomId,
    string Name,
    string Identifier,
    int HeightInUnits,
    int? WidthInInches,
    int? DepthInMm,
    decimal? MaxPowerWatts,
    decimal? MaxWeightKg,
    string? Notes,
    bool IsActive,
    DateTime CreatedAtUtc,
    string? CreatedBy,
    DateTime? LastModifiedAtUtc,
    string? LastModifiedBy);