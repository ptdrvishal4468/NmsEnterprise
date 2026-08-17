namespace Nms.Application.Locations.Floors.Dtos;

public record UpdateFloorDto(
    string Name,
    int FloorNumber,
    string? Description,
    bool IsActive);