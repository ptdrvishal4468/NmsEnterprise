namespace Nms.Application.Locations.Buildings.Dtos;

public record CreateBuildingDto(
    Guid SiteId,
    string Name,
    string Code,
    string? Description,
    string? Address);