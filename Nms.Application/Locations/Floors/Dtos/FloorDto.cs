namespace Nms.Application.Locations.Floors.Dtos;

public record FloorDto(
    Guid Id,
    Guid TenantId,
    Guid BuildingId,
    string Name,
    int FloorNumber,
    string? Description,
    bool IsActive,
    DateTime CreatedAtUtc,
    string? CreatedBy,
    DateTime? LastModifiedAtUtc,
    string? LastModifiedBy,
    int RoomCount);