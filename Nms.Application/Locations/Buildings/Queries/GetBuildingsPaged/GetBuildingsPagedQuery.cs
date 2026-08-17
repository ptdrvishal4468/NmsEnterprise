using MediatR;
using Nms.Application.Common.Models;
using Nms.Application.Locations.Buildings.Dtos;

namespace Nms.Application.Locations.Buildings.Queries.GetBuildingsPaged;

public record GetBuildingsPagedQuery(
    Guid? SiteId = null,
    int PageIndex = 1,
    int PageSize = 20,
    string? SearchTerm = null,
    bool? IsActive = null) : IRequest<PagedResult<BuildingDto>>;