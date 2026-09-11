namespace Nms.Application.Locations.Buildings.Dtos;

public record UpdateBuildingDto(
    string Name,
    string Code,
    string? Description,
    string? Address,
    bool IsActive);