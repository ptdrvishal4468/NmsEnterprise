namespace Nms.Application.Locations.Buildings.Dtos;

public record BuildingDto(
    Guid Id,
    Guid TenantId,
    Guid SiteId,
    string Name,
    string Code,
    string? Description,
    string? Address,
    bool IsActive,
    DateTime CreatedAtUtc,
    string? CreatedBy,
    DateTime? LastModifiedAtUtc,
    string? LastModifiedBy,
    int FloorCount);