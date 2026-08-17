namespace Nms.Application.Locations.Rooms.Dtos;

public record RoomDto(
    Guid Id,
    Guid TenantId,
    Guid FloorId,
    string Name,
    string Code,
    string? RoomType,
    string? Description,
    bool IsActive,
    DateTime CreatedAtUtc,
    string? CreatedBy,
    DateTime? LastModifiedAtUtc,
    string? LastModifiedBy,
    int RackCount);