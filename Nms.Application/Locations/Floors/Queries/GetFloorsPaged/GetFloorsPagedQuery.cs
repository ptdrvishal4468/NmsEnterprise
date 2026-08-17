using MediatR;
using Nms.Application.Common.Models;
using Nms.Application.Locations.Floors.Dtos;

namespace Nms.Application.Locations.Floors.Queries.GetFloorsPaged;

public record GetFloorsPagedQuery(
    Guid? BuildingId = null,
    int PageIndex = 1,
    int PageSize = 20,
    string? SearchTerm = null,
    bool? IsActive = null) : IRequest<PagedResult<FloorDto>>;