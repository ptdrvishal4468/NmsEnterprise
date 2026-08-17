namespace Nms.Application.Locations.Floors.Dtos;

public record CreateFloorDto(
    Guid BuildingId,
    string Name,
    int FloorNumber,
    string? Description);