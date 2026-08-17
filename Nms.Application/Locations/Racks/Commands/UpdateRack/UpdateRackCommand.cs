using MediatR;
using Nms.Application.Locations.Racks.Dtos;

namespace Nms.Application.Locations.Racks.Commands.UpdateRack;

public record UpdateRackCommand(Guid Id, UpdateRackDto Dto) : IRequest<RackDto>;