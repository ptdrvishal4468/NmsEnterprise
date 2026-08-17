namespace Nms.Application.Locations.Racks.Dtos;

public record CreateRackDto(
    Guid RoomId,
    string Name,
    string Identifier,
    int HeightInUnits = 42,
    int? WidthInInches = 19,
    int? DepthInMm = null,
    decimal? MaxPowerWatts = null,
    decimal? MaxWeightKg = null,
    string? Notes = null);