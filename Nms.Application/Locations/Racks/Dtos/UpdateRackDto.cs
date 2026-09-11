namespace Nms.Application.Locations.Racks.Dtos;

public record UpdateRackDto(
    string Name,
    string Identifier,
    int HeightInUnits,
    int? WidthInInches,
    int? DepthInMm,
    decimal? MaxPowerWatts,
    decimal? MaxWeightKg,
    string? Notes,
    bool IsActive);