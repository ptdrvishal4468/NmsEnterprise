namespace Nms.Application.Locations.Rooms.Dtos;

public record UpdateRoomDto(
    string Name,
    string Code,
    string? RoomType,
    string? Description,
    bool IsActive);