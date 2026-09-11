namespace Nms.Application.Locations.Rooms.Dtos;

public record CreateRoomDto(
    Guid FloorId,
    string Name,
    string Code,
    string? RoomType,
    string? Description);