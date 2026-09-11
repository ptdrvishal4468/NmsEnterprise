using MediatR;
using Nms.Application.Locations.Racks.Dtos;

namespace Nms.Application.Locations.Racks.Queries.GetRackById;

public record GetRackByIdQuery(Guid Id) : IRequest<RackDto?>;