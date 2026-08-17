using MediatR;

namespace Nms.Application.Locations.Racks.Commands.DeleteRack;

public record DeleteRackCommand(Guid Id) : IRequest<bool>;