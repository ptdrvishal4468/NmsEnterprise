using MediatR;
using Nms.Application.Locations.Buildings.Dtos;

namespace Nms.Application.Locations.Buildings.Queries.GetBuildingById;

public record GetBuildingByIdQuery(Guid Id) : IRequest<BuildingDto?>;