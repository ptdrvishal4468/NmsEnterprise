using MediatR;
using Nms.Application.Locations.Floors.Dtos;

namespace Nms.Application.Locations.Floors.Queries.GetFloorById;

public record GetFloorByIdQuery(Guid Id) : IRequest<FloorDto?>;