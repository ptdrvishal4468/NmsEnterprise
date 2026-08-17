using MediatR;
using Nms.Application.Common.Models;
using Nms.Application.Locations.Racks.Dtos;

namespace Nms.Application.Locations.Racks.Queries.GetRacksPaged;

public record GetRacksPagedQuery(
    Guid? RoomId = null,
    int PageIndex = 1,
    int PageSize = 20,
    string? SearchTerm = null,
    bool? IsActive = null) : IRequest<PagedResult<RackDto>>;