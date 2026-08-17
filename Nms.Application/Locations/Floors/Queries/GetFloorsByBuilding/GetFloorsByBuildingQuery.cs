using MediatR;
using Nms.Application.Locations.Floors.Dtos;

namespace Nms.Application.Locations.Floors.Queries.GetFloorsByBuilding;

public record GetFloorsByBuildingQuery(Guid BuildingId) : IRequest<IReadOnlyList<FloorDto>>;