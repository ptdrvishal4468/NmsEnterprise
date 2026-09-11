using MediatR;
using Nms.Application.Locations.Buildings.Dtos;

namespace Nms.Application.Locations.Buildings.Queries.GetBuildingsBySite;

public record GetBuildingsBySiteQuery(Guid SiteId) : IRequest<IReadOnlyList<BuildingDto>>;