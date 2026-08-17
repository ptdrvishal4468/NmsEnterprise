using MediatR;
using Nms.Application.Common.Models;
using Nms.Application.Locations.Rooms.Dtos;

namespace Nms.Application.Locations.Rooms.Queries.GetRoomsPaged;

public record GetRoomsPagedQuery(
    Guid? FloorId = null,
    int PageIndex = 1,
    int PageSize = 20,
    string? SearchTerm = null,
    bool? IsActive = null) : IRequest<PagedResult<RoomDto>>;